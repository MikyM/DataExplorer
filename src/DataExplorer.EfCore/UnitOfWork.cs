using System.Collections.Concurrent;
using AutoMapper;
using DataExplorer.Abstractions.DataContexts;
using DataExplorer.Abstractions.Repositories;
using DataExplorer.Abstractions.UnitOfWork;
using DataExplorer.EfCore.Abstractions;
using DataExplorer.EfCore.DataContexts;
using DataExplorer.EfCore.Gridify;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Options;
using ISpecificationEvaluator = DataExplorer.EfCore.Specifications.Evaluators.ISpecificationEvaluator;

namespace DataExplorer.EfCore;

/// <inheritdoc cref="IUnitOfWork"/>
[PublicAPI]
public sealed class UnitOfWork<TContext> : IUnitOfWork<TContext> where TContext : IEfDbContext
{
    /// <summary>
    /// Inner <see cref="ISpecificationEvaluator"/>.
    /// </summary>
    public ISpecificationEvaluator SpecificationEvaluator { get; }
    
    /// <summary>
    /// Configuration.
    /// </summary>
    private readonly IOptions<DataExplorerEfCoreConfiguration> _options;

    // To detect redundant calls
    private bool _disposed;
    // ReSharper disable once InconsistentNaming

    /// <summary>
    /// Repositories.
    /// </summary>
    private ConcurrentDictionary<RepositoryEntryKey, Lazy<RepositoryEntry>>? _repositories;
    
    /// <summary>
    /// Repository cache data.
    /// </summary>
    // ReSharper disable once StaticMemberInGenericType
    private static ConcurrentDictionary<Type, RepoCacheData> _repoCacheData = new();

    /// <summary>
    /// Inner <see cref="IDbContextTransaction"/>.
    /// </summary>
    private IDbContextTransaction? _transaction;

    /// <summary>
    /// Mapper instance.
    /// </summary>
    public IMapper Mapper { get; }
    
    /// <summary>
    /// GridifyMapperProvider instance.
    /// </summary>
    public IGridifyMapperProvider GridifyMapperProvider { get; }

    /// <summary>
    /// Data cache.
    /// </summary>
    private readonly IEfDataExplorerTypeCache _cache;

    /// <summary>
    /// Creates a new instance of <see cref="UnitOfWork{TContext}"/>.
    /// </summary>
    /// <param name="context"><see cref="DbContext"/> to be used.</param>
    /// <param name="specificationEvaluator">Specification evaluator to be used.</param>
    /// <param name="mapper">Mapper.</param>
    /// <param name="options">Options.</param>
    /// <param name="gridifyMapperProvider">Gridify mapper provider.</param>
    /// <param name="efDataExplorerTypeCache">Unit of Work cache.</param>
    public UnitOfWork(TContext context, ISpecificationEvaluator specificationEvaluator, IMapper mapper,
        IOptions<DataExplorerEfCoreConfiguration> options, IGridifyMapperProvider gridifyMapperProvider, IEfDataExplorerTypeCache efDataExplorerTypeCache)
    {
        Context = context;
        SpecificationEvaluator = specificationEvaluator;
        _options = options;
        GridifyMapperProvider = gridifyMapperProvider;
        Mapper = mapper;
        _cache = efDataExplorerTypeCache;
    }

    /// <inheritdoc />
    public TContext Context { get; }

    /// <inheritdoc />
    public async Task UseTransactionAsync(CancellationToken cancellationToken = default)
        => _transaction ??= await Context.Database.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);

    /// <inheritdoc cref="IUnitOfWork.GetRepositoryFor{TRepository}" />
    public IRepository<TEntity> GetRepositoryFor<TEntity>() where TEntity : EfEntity<long>
    {
        var entityType = typeof(TEntity);
        var repositoryType = _cache.CachedCrudRepos.GetValueOrDefault(entityType);

        if (repositoryType is null)
            throw new InvalidOperationException("Couldn't find proper type in cache.");

        if (_repoCacheData.TryGetValue(repositoryType, out var cachedData))
            return LazilyGetOrCreateRepository<IRepository<TEntity>>(cachedData);
        
        var repoCacheData = CreateAndCacheRepoData(repositoryType, typeof(IRepository<TEntity>), entityType, typeof(long), true);
        
        return LazilyGetOrCreateRepository<IRepository<TEntity>>(repoCacheData);
    }

    /// <inheritdoc cref="IUnitOfWork.GetReadOnlyRepositoryFor{TRepository}" />
    public IReadOnlyRepository<TEntity> GetReadOnlyRepositoryFor<TEntity>() where TEntity : EfEntity<long>
    {
        var entityType = typeof(TEntity);
        var repositoryType = _cache.CachedReadOnlyRepos.GetValueOrDefault(entityType);

        if (repositoryType is null)
            throw new InvalidOperationException("Couldn't find proper type in cache.");

        if (_repoCacheData.TryGetValue(repositoryType, out var cachedData))
            return LazilyGetOrCreateRepository<IReadOnlyRepository<TEntity>>(cachedData);

        var repoCacheData = CreateAndCacheRepoData(repositoryType, typeof(IReadOnlyRepository<TEntity>), entityType, typeof(long), false);

        return LazilyGetOrCreateRepository<IRepository<TEntity>>(repoCacheData);
    }

    /// <inheritdoc cref="IUnitOfWork.GetRepositoryFor{TRepository,TId}" />
    public IRepository<TEntity, TId> GetRepositoryFor<TEntity, TId>() where TEntity : EfEntity<TId>
        where TId : IComparable, IEquatable<TId>, IComparable<TId>
    {
        var entityType = typeof(TEntity);
        var repositoryType = _cache.CachedCrudGenericIdRepos.GetValueOrDefault(entityType);

        if (repositoryType is null)
            throw new InvalidOperationException("Couldn't find proper type in cache.");

        if (_repoCacheData.TryGetValue(repositoryType, out var cachedData))
            return LazilyGetOrCreateRepository<IRepository<TEntity, TId>>(cachedData);

        var repoCacheData = CreateAndCacheRepoData(repositoryType, typeof(IRepository<TEntity, TId>), entityType, typeof(TId), true);

        return LazilyGetOrCreateRepository<IRepository<TEntity, TId>>(repoCacheData);
    }

    /// <inheritdoc cref="IUnitOfWork.GetReadOnlyRepositoryFor{TRepository,TId}" />
    public IReadOnlyRepository<TEntity, TId> GetReadOnlyRepositoryFor<TEntity, TId>() where TEntity : EfEntity<TId>
        where TId : IComparable, IEquatable<TId>, IComparable<TId>
    {
        var entityType = typeof(TEntity);
        var repositoryType = _cache.CachedReadOnlyGenericIdRepos.GetValueOrDefault(entityType);

        if (repositoryType is null)
            throw new InvalidOperationException("Couldn't find proper type in cache.");
        
        if (_repoCacheData.TryGetValue(repositoryType, out var cachedData))
            return LazilyGetOrCreateRepository<IReadOnlyRepository<TEntity, TId>>(cachedData);

        var repoCacheData = CreateAndCacheRepoData(repositoryType, typeof(IReadOnlyRepository<TEntity, TId>), entityType, typeof(TId), false);
        
        return LazilyGetOrCreateRepository<IReadOnlyRepository<TEntity, TId>>(repoCacheData);
    }

    /// <inheritdoc cref="IUnitOfWork.GetRepository{TRepository}" />
    public TRepository GetRepository<TRepository>() where TRepository : class, IRepositoryBase
    {
        var repoInterfaceType = typeof(TRepository);
        if (!repoInterfaceType.IsInterface || !repoInterfaceType.IsGenericType)
            throw new NotSupportedException(
                "You can only retrieve types: IRepository<TEntity>, IRepository<TEntity,TId>, IReadOnlyRepository<TEntity> and IReadOnlyRepository<TEntity,TId>.");

        if (_repoCacheData.TryGetValue(repoInterfaceType, out var cachedData))
            return LazilyGetOrCreateRepository<TRepository>(cachedData);
        
        var entityType = repoInterfaceType.GetGenericArguments().FirstOrDefault();
        if (entityType is null)
            throw new ArgumentException(
                "Couldn't retrieve entity type from generic arguments on given repository type.");

        var genericDefinition = repoInterfaceType.GetGenericTypeDefinition();
        
        if (!_cache.AllowedRepoTypes.Contains(genericDefinition))
            throw new NotSupportedException(
                "You can only retrieve types: IRepository<TEntity>, IRepository<TEntity,TId>, IReadOnlyRepository<TEntity> and IReadOnlyRepository<TEntity,TId>.");
        
        Type? repositoryType;
        bool isCrud;
        switch (repoInterfaceType.IsGenericType)
        {
            case true when genericDefinition == typeof(IRepository<,>):
                repositoryType = _cache.CachedCrudGenericIdRepos.GetValueOrDefault(entityType);
                isCrud = true;
                break;
            case true when genericDefinition == typeof(IReadOnlyRepository<,>):
                repositoryType = _cache.CachedReadOnlyGenericIdRepos.GetValueOrDefault(entityType);
                isCrud = false;
                break;
            case true when genericDefinition == typeof(IRepository<>):
                repositoryType = _cache.CachedCrudRepos.GetValueOrDefault(entityType);
                isCrud = true;
                break;
            case true when genericDefinition == typeof(IReadOnlyRepository<>):
                repositoryType = _cache.CachedReadOnlyRepos.GetValueOrDefault(entityType);
                isCrud = false;
                break;
            default:
                throw new NotSupportedException(
                    "You can only retrieve types: IRepository<TEntity>, IRepository<TEntity,TId>, IReadOnlyRepository<TEntity> and IReadOnlyRepository<TEntity,TId>.");
        };

        if (repositoryType is null)
            throw new InvalidOperationException("Couldn't find proper type in cache.");
        
        if (!_cache.EntityTypeIdTypeDictionary.TryGetValue(entityType, out var idType))
            throw new InvalidOperationException($"Couldn't find id type for type: {entityType.Name}.");

        var repoCacheData = CreateAndCacheRepoData(repositoryType, repoInterfaceType, entityType, idType, isCrud);

        return LazilyGetOrCreateRepository<TRepository>(repoCacheData);
    }
    
    private RepoCacheData CreateAndCacheRepoData(Type repoImplementationType, Type repoInterfaceType, Type entityType, Type entityIdType,
        bool isCrud)
    {
        var repoCacheData = new RepoCacheData(entityType, entityIdType, isCrud, repoImplementationType, repoInterfaceType);

        _ = _repoCacheData.TryAdd(repoInterfaceType, repoCacheData);

        return repoCacheData;
    }

    /// <summary>
    /// Lazily creates a new repository instance of a given type.
    /// </summary>
    /// <param name="repoCacheData">Repository cache data.</param>
    /// <typeparam name="TRepository">Type of the wanted repository</typeparam>
    /// <returns>Created repo instance.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    private TRepository LazilyGetOrCreateRepository<TRepository>(RepoCacheData repoCacheData) where TRepository : IRepositoryBase
    {
        _repositories ??= new ConcurrentDictionary<RepositoryEntryKey, Lazy<RepositoryEntry>>();

        var repositoryTypeName = repoCacheData.RepoImplementationType.FullName ?? repoCacheData.RepoImplementationType.Name;
        var entityTypeName = repoCacheData.EntityType.FullName ?? repoCacheData.EntityType.Name;

        var key = new RepositoryEntryKey(entityTypeName, repoCacheData.IsCrud);

        var repositoryEntry = _repositories.GetOrAdd(key, new Lazy<RepositoryEntry>(() =>
        {
            var lazyRepo = new Lazy<IRepositoryBase>(() =>
            {
                var instance =
                    InstanceFactory.CreateInstance(repoCacheData.RepoImplementationType, Context,
                        SpecificationEvaluator, Mapper, GridifyMapperProvider);

                if (instance is null)
                    throw new InvalidOperationException($"Couldn't create an instance of {repositoryTypeName}");

                return (TRepository)instance;
            }, LazyThreadSafetyMode.ExecutionAndPublication);

            return new RepositoryEntry(repoCacheData, lazyRepo);
        }, LazyThreadSafetyMode.ExecutionAndPublication));

        return (TRepository)repositoryEntry.Value.LazyRepo.Value;
    }
    
    /// <inheritdoc />
    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction is not null) 
            await _transaction.RollbackAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    IDataContextBase IUnitOfWorkBase.Context => Context;

    /// <inheritdoc />
    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        if (_options.Value.OnBeforeSaveChangesActions is not null &&
            _options.Value.OnBeforeSaveChangesActions.TryGetValue(typeof(TContext).Name, out var action))
            await action.Invoke(this);
        
        _ = await Context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        
        if (_transaction is not null) 
            await _transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task CommitAsync(string userId, CancellationToken cancellationToken = default)
    {
        if (_options.Value.OnBeforeSaveChangesActions is not null &&
            _options.Value.OnBeforeSaveChangesActions.TryGetValue(typeof(TContext).Name, out var action))
            await action.Invoke(this);

        if (Context is AuditableEfDbContext auditableDbContext)
            _ = await auditableDbContext.SaveChangesAsync(userId, cancellationToken).ConfigureAwait(false);
        else
            _ = await Context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        
        if (_transaction is not null) 
            await _transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
    }
    
    /// <inheritdoc />
    public async Task<int> CommitWithCountAsync(CancellationToken cancellationToken = default)
    {
        if (_options.Value.OnBeforeSaveChangesActions is not null &&
            _options.Value.OnBeforeSaveChangesActions.TryGetValue(typeof(TContext).Name, out var action))
            await action.Invoke(this);

        int result = await Context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        if (_transaction is not null) 
            await _transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
        return result;
    }

    /// <inheritdoc />
    public async Task<int> CommitWithCountAsync(string userId, CancellationToken cancellationToken = default)
    {
        if (_options.Value.OnBeforeSaveChangesActions is not null &&
            _options.Value.OnBeforeSaveChangesActions.TryGetValue(typeof(TContext).Name, out var action))
            await action.Invoke(this);

        int result;
        if (Context is AuditableEfDbContext auditableDbContext)
            result = await auditableDbContext.SaveChangesAsync(userId, cancellationToken).ConfigureAwait(false);
        else
            result = await Context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        if (_transaction is not null) 
            await _transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
        
        return result;
    }

    // Public implementation of Dispose pattern callable by consumers.
    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    // Protected implementation of Dispose pattern.
    private void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
        {
            Context.Dispose();
            _transaction?.Dispose();
        }

        _repositories = null;

        _disposed = true;
    }
}

/// <summary>
/// Repository cache data.
/// </summary>
internal record RepoCacheData
{
    internal Type EntityType { get; }
    internal Type EntityIdType { get; }
    internal bool IsCrud { get; }
    internal Type RepoImplementationType { get; }
    internal Type RepoInterfaceType { get; }

    public RepoCacheData(Type entityType, Type entityIdType, bool isCrud, Type repoImplementationType, Type repoInterfaceType)
    {
        EntityType = entityType;
        EntityIdType = entityIdType;
        IsCrud = isCrud;
        RepoImplementationType = repoImplementationType;
        RepoInterfaceType = repoInterfaceType;
    }
}

/// <summary>
/// Repository entry.
/// </summary>
internal record RepositoryEntry
{
    internal RepositoryEntry(RepoCacheData repoCacheData, Lazy<IRepositoryBase> lazyRepo)
    {
        RepoCacheData = repoCacheData;
        LazyRepo = lazyRepo;
    }
    internal RepoCacheData RepoCacheData { get; }
    internal Lazy<IRepositoryBase> LazyRepo { get; }
}

/// <summary>
/// Repository entry key.
/// </summary>
internal readonly struct RepositoryEntryKey : IEquatable<RepositoryEntryKey>
{
    internal RepositoryEntryKey(string entityTypeName, bool isCrud)
    {
        EntityTypeName = entityTypeName;
        IsCrud = isCrud;
    }

    private string EntityTypeName { get; }
    private bool IsCrud { get; }

    public bool Equals(RepositoryEntryKey other)
        => EntityTypeName == other.EntityTypeName && IsCrud == other.IsCrud;

    public override bool Equals(object? obj)
        => obj is RepositoryEntryKey other && Equals(other);

    public override int GetHashCode()
        => HashCode.Combine(EntityTypeName, IsCrud);
}
