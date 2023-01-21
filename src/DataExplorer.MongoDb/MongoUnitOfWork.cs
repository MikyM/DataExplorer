using System.Collections.Concurrent;
using AutoMapper;
using DataExplorer.Abstractions.DataContexts;
using DataExplorer.Abstractions.Repositories;
using DataExplorer.Abstractions.UnitOfWork;
using DataExplorer.MongoDb.Abstractions;
using DataExplorer.MongoDb.Abstractions.DataContexts;
using DataExplorer.MongoDb.DataContexts;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Entities;

namespace DataExplorer.MongoDb;

/// <inheritdoc cref="IMongoUnitOfWork{TContext}"/>
[PublicAPI]
public sealed class MongoUnitOfWork<TContext> : IMongoUnitOfWork<TContext> where TContext : IMongoDbContext
{
    /// <summary>
    /// Configuration.
    /// </summary>
    private readonly IOptions<DataExplorerMongoDbConfiguration> _options;

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
    /// Inner <see cref="IClientSessionHandle"/>.
    /// </summary>
    public IClientSessionHandle? Transaction { get; private set; }

    /// <summary>
    /// Mapper instance.
    /// </summary>
    public IMapper Mapper { get; }

    /// <summary>
    /// Data cache.
    /// </summary>
    private readonly IMongoDataExplorerTypeCache _cache;

    /// <summary>
    /// Creates a new instance of <see cref="MongoUnitOfWork{TContext}"/>.
    /// </summary>
    /// <param name="context"><see cref="MongoDbContext"/> to be used.</param>
    /// <param name="mapper">Mapper.</param>
    /// <param name="options">Options.</param>
    /// <param name="mongoDataExplorerTypeCache">Unit of Work cache.</param>
    public MongoUnitOfWork(TContext context, IMapper mapper,
        IOptions<DataExplorerMongoDbConfiguration> options, IMongoDataExplorerTypeCache mongoDataExplorerTypeCache)
    {
        Context = context;
        _options = options;
        Mapper = mapper;
        _cache = mongoDataExplorerTypeCache;
    }

    /// <inheritdoc />
    public TContext Context { get; }

    /// <inheritdoc />
    public Task UseTransactionAsync(CancellationToken cancellationToken = default)
        => Task.FromResult(Transaction ??= Context.Transaction());

    /// <inheritdoc cref="IMongoUnitOfWork.GetRepositoryFor{TRepository}" />
    public IMongoRepository<TEntity> GetRepositoryFor<TEntity>() where TEntity : MongoEntity<long>
    {
        var entityType = typeof(TEntity);
        var repositoryType = _cache.CachedCrudRepos.GetValueOrDefault(entityType);

        if (repositoryType is null)
            throw new InvalidOperationException("Couldn't find proper type in cache.");

        if (_repoCacheData.TryGetValue(repositoryType, out var cachedData))
            return LazilyGetOrCreateRepository<IMongoRepository<TEntity>>(cachedData);
        
        var repoCacheData = CreateAndCacheRepoData(repositoryType, typeof(IMongoRepository<TEntity>), entityType, typeof(long), true);
        
        return LazilyGetOrCreateRepository<IMongoRepository<TEntity>>(repoCacheData);
    }

    /// <inheritdoc cref="IMongoUnitOfWork.GetReadOnlyRepositoryFor{TRepository}" />
    public IMongoReadOnlyRepository<TEntity> GetReadOnlyRepositoryFor<TEntity>() where TEntity : MongoEntity<long>
    {
        var entityType = typeof(TEntity);
        var repositoryType = _cache.CachedReadOnlyRepos.GetValueOrDefault(entityType);

        if (repositoryType is null)
            throw new InvalidOperationException("Couldn't find proper type in cache.");

        if (_repoCacheData.TryGetValue(repositoryType, out var cachedData))
            return LazilyGetOrCreateRepository<IMongoReadOnlyRepository<TEntity>>(cachedData);

        var repoCacheData = CreateAndCacheRepoData(repositoryType, typeof(IMongoReadOnlyRepository<TEntity>), entityType, typeof(long), false);

        return LazilyGetOrCreateRepository<IMongoRepository<TEntity>>(repoCacheData);
    }

    /// <inheritdoc cref="IMongoUnitOfWork.GetRepositoryFor{TRepository,TId}" />
    public IMongoRepository<TEntity, TId> GetRepositoryFor<TEntity, TId>() where TEntity : MongoEntity<TId>
        where TId : IComparable, IEquatable<TId>, IComparable<TId>
    {
        var entityType = typeof(TEntity);
        var repositoryType = _cache.CachedCrudGenericIdRepos.GetValueOrDefault(entityType);

        if (repositoryType is null)
            throw new InvalidOperationException("Couldn't find proper type in cache.");

        if (_repoCacheData.TryGetValue(repositoryType, out var cachedData))
            return LazilyGetOrCreateRepository<IMongoRepository<TEntity, TId>>(cachedData);

        var repoCacheData = CreateAndCacheRepoData(repositoryType, typeof(IMongoRepository<TEntity, TId>), entityType, typeof(TId), true);

        return LazilyGetOrCreateRepository<IMongoRepository<TEntity, TId>>(repoCacheData);
    }

    /// <inheritdoc cref="IMongoUnitOfWork.GetReadOnlyRepositoryFor{TRepository,TId}" />
    public IMongoReadOnlyRepository<TEntity, TId> GetReadOnlyRepositoryFor<TEntity, TId>() where TEntity : MongoEntity<TId>
        where TId : IComparable, IEquatable<TId>, IComparable<TId>
    {
        var entityType = typeof(TEntity);
        var repositoryType = _cache.CachedReadOnlyGenericIdRepos.GetValueOrDefault(entityType);

        if (repositoryType is null)
            throw new InvalidOperationException("Couldn't find proper type in cache.");
        
        if (_repoCacheData.TryGetValue(repositoryType, out var cachedData))
            return LazilyGetOrCreateRepository<IMongoReadOnlyRepository<TEntity, TId>>(cachedData);

        var repoCacheData = CreateAndCacheRepoData(repositoryType, typeof(IMongoReadOnlyRepository<TEntity, TId>), entityType, typeof(TId), false);
        
        return LazilyGetOrCreateRepository<IMongoReadOnlyRepository<TEntity, TId>>(repoCacheData);
    }

    /// <inheritdoc cref="IMongoUnitOfWork.GetRepository{TRepository}" />
    public TRepository GetRepository<TRepository>() where TRepository : class, IRepositoryBase
    {
        var repoInterfaceType = typeof(TRepository);
        if (!repoInterfaceType.IsInterface || !repoInterfaceType.IsGenericType)
            throw new NotSupportedException(
                "You can only retrieve types: IMongoRepository<TEntity>, IMongoRepository<TEntity,TId>, IMongoReadOnlyRepository<TEntity> and IMongoReadOnlyRepository<TEntity,TId>.");

        if (_repoCacheData.TryGetValue(repoInterfaceType, out var cachedData))
            return LazilyGetOrCreateRepository<TRepository>(cachedData);
        
        var entityType = repoInterfaceType.GetGenericArguments().FirstOrDefault();
        if (entityType is null)
            throw new ArgumentException(
                "Couldn't retrieve entity type from generic arguments on given repository type.");

        var genericDefinition = repoInterfaceType.GetGenericTypeDefinition();
        
        if (!_cache.AllowedRepoTypes.Contains(genericDefinition))
            throw new NotSupportedException(
                "You can only retrieve types: IMongoRepository<TEntity>, IMongoRepository<TEntity,TId>, IMongoReadOnlyRepository<TEntity> and IMongoReadOnlyRepository<TEntity,TId>.");
        
        Type? repositoryType;
        bool isCrud;
        switch (repoInterfaceType.IsGenericType)
        {
            case true when genericDefinition == typeof(IMongoRepository<,>):
                repositoryType = _cache.CachedCrudGenericIdRepos.GetValueOrDefault(entityType);
                isCrud = true;
                break;
            case true when genericDefinition == typeof(IMongoReadOnlyRepository<,>):
                repositoryType = _cache.CachedReadOnlyGenericIdRepos.GetValueOrDefault(entityType);
                isCrud = false;
                break;
            case true when genericDefinition == typeof(IMongoRepository<>):
                repositoryType = _cache.CachedCrudRepos.GetValueOrDefault(entityType);
                isCrud = true;
                break;
            case true when genericDefinition == typeof(IMongoReadOnlyRepository<>):
                repositoryType = _cache.CachedReadOnlyRepos.GetValueOrDefault(entityType);
                isCrud = false;
                break;
            default:
                throw new NotSupportedException(
                    "You can only retrieve types: IMongoRepository<TEntity>, IMongoRepository<TEntity,TId>, IMongoReadOnlyRepository<TEntity> and IMongoReadOnlyRepository<TEntity,TId>.");
        }

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
                        /*SpecificationEvaluator,*/ Mapper);

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
        if (Transaction is not null) 
            await Transaction.AbortTransactionAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    IDataContextBase IUnitOfWorkBase.Context => Context;

    /// <inheritdoc />
    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        if (_options.Value.OnBeforeSaveChangesActions is not null &&
            _options.Value.OnBeforeSaveChangesActions.TryGetValue(typeof(TContext).Name, out var action))
            await action.Invoke(this);
        
        await Context.CommitAsync(cancellationToken).ConfigureAwait(false);
        
        if (Transaction is not null) 
            await Transaction.CommitTransactionAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task CommitAsync(string userId, CancellationToken cancellationToken = default)
    {
        if (_options.Value.OnBeforeSaveChangesActions is not null &&
            _options.Value.OnBeforeSaveChangesActions.TryGetValue(typeof(TContext).Name, out var action))
            await action.Invoke(this);
        
        await Context.CommitAsync(cancellationToken).ConfigureAwait(false);
        
        if (Transaction is not null) 
            await Transaction.CommitTransactionAsync(cancellationToken).ConfigureAwait(false);
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
            Transaction?.Dispose();
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
