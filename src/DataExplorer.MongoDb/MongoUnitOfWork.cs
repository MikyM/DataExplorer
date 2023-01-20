using System.Collections.Concurrent;
using AutoMapper;
using DataExplorer.Abstractions.DataContexts;
using DataExplorer.Abstractions.Repositories;
using DataExplorer.Abstractions.UnitOfWork;
using DataExplorer.MongoDb.Abstractions;
using DataExplorer.MongoDb.Abstractions.DataContexts;
using DataExplorer.MongoDb.DataContexts;
using MongoDB.Driver;
using MongoDB.Entities;

namespace DataExplorer.MongoDb;

/// <inheritdoc cref="IMongoUnitOfWork{TContext}"/>
[PublicAPI]
public sealed class MongoUnitOfWork<TContext> : IMongoUnitOfWork<TContext> where TContext : IMongoDbContext
{
    /*/// <summary>
    /// Inner <see cref="ISpecificationEvaluator"/>.
    /// </summary>
    private readonly ISpecificationEvaluator _specificationEvaluator;
    /// <summary>
    /// Configuration.
    /// </summary>
    private readonly IOptions<DataExplorerEfCoreConfiguration> _options;*/

    // To detect redundant calls
    private bool _disposed;
    // ReSharper disable once InconsistentNaming

    /// <summary>
    /// Repository cache.
    /// </summary>
    private ConcurrentDictionary<RepositoryEntryKey, Lazy<RepositoryEntry>>? _repositories;

    /// <summary>
    /// Inner <see cref="MongoDB.Entities.Transaction"/>.
    /// </summary>
    public IClientSessionHandle? Transaction { get; set; }

    /// <summary>
    /// Mapper instance.
    /// </summary>
    public IMapper Mapper { get; }

    /// <summary>
    /// Creates a new instance of <see cref="MongoUnitOfWork{TContext}"/>.
    /// </summary>
    /// <param name="context"><see cref="MongoDbContext"/> to be used.</param>
    /// <param name="specificationEvaluator">Specification evaluator to be used.</param>
    /// <param name="mapper">Mapper.</param>
    /// <param name="options">Options.</param>
    public MongoUnitOfWork(TContext context, /*ISpecificationEvaluator specificationEvaluator, */IMapper mapper /*IOptions<DataExplorerEfCoreConfiguration> options*/)
    {
        Context = context;
        /*_specificationEvaluator = specificationEvaluator;
        _options = options;*/
        Mapper = mapper;
    }

    /// <inheritdoc />
    public TContext Context { get; }

    /// <inheritdoc />
    public Task UseTransactionAsync(CancellationToken cancellationToken = default)
        => Task.FromResult(Transaction ??= Context.Transaction());

    /// <inheritdoc cref="IMongoUnitOfWork.GetRepositoryFor{TRepository}" />
    public IMongoRepository<TEntity> GetRepositoryFor<TEntity>() where TEntity : Entity<long>, IEntity
    {
        var entityType = typeof(TEntity);
        var repositoryType = UoFCache.CachedCrudRepos.GetValueOrDefault(entityType);

        if (repositoryType is null)
            throw new InvalidOperationException("Couldn't find proper type in cache.");

        return LazilyGetOrCreateRepository<IMongoRepository<TEntity>>(repositoryType, entityType, true);
    }

    /// <inheritdoc />
    IRepositoryBase IUnitOfWorkBase.GetRepositoryFor<TEntity, TId>()
        => throw new NotSupportedException("Using GetRepositoryFor methods from the IUnitOfWorkBase interface is not supported in MongoUnitOfWork");

    /// <inheritdoc cref="IMongoUnitOfWork.GetReadOnlyRepositoryFor{TRepository}" />
    public IMongoReadOnlyRepository<TEntity> GetReadOnlyRepositoryFor<TEntity>() where TEntity : Entity<long>, IEntity
    {
        var entityType = typeof(TEntity);
        var repositoryType = UoFCache.CachedReadOnlyRepos.GetValueOrDefault(entityType);

        if (repositoryType is null)
            throw new InvalidOperationException("Couldn't find proper type in cache.");

        return LazilyGetOrCreateRepository<IMongoReadOnlyRepository<TEntity>>(repositoryType, entityType, false);
    }

    /// <inheritdoc />
    IRepositoryBase IUnitOfWorkBase.GetRepositoryFor<TEntity>()
        => throw new NotSupportedException("Using GetRepositoryFor methods from the IUnitOfWorkBase interface is not supported in MongoUnitOfWork");

    /// <inheritdoc cref="IMongoUnitOfWork.GetRepositoryFor{TRepository,TId}" />
    public IMongoRepository<TEntity, TId> GetRepositoryFor<TEntity, TId>() where TEntity : Entity<TId>, IEntity
        where TId : IComparable, IEquatable<TId>, IComparable<TId>
    {
        var entityType = typeof(TEntity);
        var repositoryType = UoFCache.CachedCrudGenericIdRepos.GetValueOrDefault(entityType);

        if (repositoryType is null)
            throw new InvalidOperationException("Couldn't find proper type in cache.");

        return LazilyGetOrCreateRepository<IMongoRepository<TEntity, TId>>(repositoryType, entityType, true);
    }

    /// <inheritdoc cref="IMongoUnitOfWork.GetReadOnlyRepositoryFor{TRepository,TId}" />
    public IMongoReadOnlyRepository<TEntity, TId> GetReadOnlyRepositoryFor<TEntity, TId>() where TEntity : Entity<TId>, IEntity
        where TId : IComparable, IEquatable<TId>, IComparable<TId>
    {
        var entityType = typeof(TEntity);
        var repositoryType = UoFCache.CachedReadOnlyGenericIdRepos.GetValueOrDefault(entityType);

        if (repositoryType is null)
            throw new InvalidOperationException("Couldn't find proper type in cache.");

        return LazilyGetOrCreateRepository<IMongoReadOnlyRepository<TEntity, TId>>(repositoryType, entityType, false);
    }

    /// <inheritdoc cref="IMongoUnitOfWork.GetRepository{TRepository}" />
    public TRepository GetRepository<TRepository>() where TRepository : class, IRepositoryBase
    {
        var givenType = typeof(TRepository);
        if (!givenType.IsInterface || !givenType.IsGenericType ||
            !UoFCache.AllowedRepoTypes.Contains(givenType.GetGenericTypeDefinition()))
            throw new NotSupportedException(
                "You can only retrieve types: IMongoRepository<TEntity>, IMongoRepository<TEntity,TId>, IMongoReadOnlyRepository<TEntity> and IMongoReadOnlyRepository<TEntity,TId>.");

        var entityType = givenType.GetGenericArguments().FirstOrDefault();
        if (entityType is null)
            throw new ArgumentException(
                "Couldn't retrieve entity type from generic arguments on given repository type.");

        Type? repositoryType;
        bool isCrud;
        switch (givenType.IsGenericType)
        {
            case true when givenType.GetGenericTypeDefinition() == typeof(IMongoRepository<,>):
                repositoryType = UoFCache.CachedCrudGenericIdRepos.GetValueOrDefault(entityType);
                isCrud = true;
                break;
            case true when givenType.GetGenericTypeDefinition() == typeof(IMongoReadOnlyRepository<,>):
                repositoryType = UoFCache.CachedReadOnlyGenericIdRepos.GetValueOrDefault(entityType);
                isCrud = false;
                break;
            case true when givenType.GetGenericTypeDefinition() == typeof(IMongoRepository<>):
                repositoryType = UoFCache.CachedCrudRepos.GetValueOrDefault(entityType);
                isCrud = true;
                break;
            case true when givenType.GetGenericTypeDefinition() == typeof(IMongoReadOnlyRepository<>):
                repositoryType = UoFCache.CachedReadOnlyRepos.GetValueOrDefault(entityType);
                isCrud = false;
                break;
            default:
                throw new NotSupportedException(
                    "You can only retrieve types: IMongoRepository<TEntity>, IMongoRepository<TEntity,TId>, IMongoReadOnlyRepository<TEntity> and IMongoReadOnlyRepository<TEntity,TId>.");
        };

        if (repositoryType is null)
            throw new InvalidOperationException("Couldn't find proper type in cache.");

        return LazilyGetOrCreateRepository<TRepository>(repositoryType, entityType, isCrud);
    }

    /// <summary>
    /// Lazily creates a new repository instance of a given type.
    /// </summary>
    /// <param name="repositoryType">Repository closed generic type.</param>
    /// <param name="entityType">Entity type.</param>
    /// <param name="isCrud">Whether the repository is a crud repository.</param>
    /// <typeparam name="TRepository">Type of the wanted repository</typeparam>
    /// <returns>Created repo instance.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    private TRepository LazilyGetOrCreateRepository<TRepository>(Type repositoryType, Type entityType, bool isCrud) where TRepository : IRepositoryBase
    {
        _repositories ??= new ConcurrentDictionary<RepositoryEntryKey, Lazy<RepositoryEntry>>();

        var repositoryTypeName = repositoryType.FullName ?? repositoryType.Name;
        var entityTypeName = entityType.FullName ?? entityType.Name;

        var key = new RepositoryEntryKey(entityTypeName, isCrud);

        var repositoryEntry = _repositories.GetOrAdd(key, new Lazy<RepositoryEntry>(() =>
        {
            var lazyRepo = new Lazy<IRepositoryBase>(() =>
            {
                var instance =
                    InstanceFactory.CreateInstance(repositoryType, Context,
                        /*_specificationEvaluator,*/ Mapper);

                if (instance is null)
                    throw new InvalidOperationException($"Couldn't create an instance of {repositoryTypeName}");

                return (TRepository)instance;
            }, LazyThreadSafetyMode.ExecutionAndPublication);

            return new RepositoryEntry(entityType, repositoryType, lazyRepo, isCrud);
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
        /*if (_options.Value.OnBeforeSaveChangesActions is not null &&
            _options.Value.OnBeforeSaveChangesActions.TryGetValue(typeof(TContext).Name, out var action))
            await action.Invoke(this);*/
        
        await Context.CommitAsync(cancellationToken).ConfigureAwait(false);
        
        if (Transaction is not null) 
            await Transaction.CommitTransactionAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task CommitAsync(string userId, CancellationToken cancellationToken = default)
    {
        /*if (_options.Value.OnBeforeSaveChangesActions is not null &&
            _options.Value.OnBeforeSaveChangesActions.TryGetValue(typeof(TContext).Name, out var action))
            await action.Invoke(this);*/

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
/// Repository entry.
/// </summary>
internal class RepositoryEntry
{
    internal RepositoryEntry(Type entityType, Type repoClosedGenericType, Lazy<IRepositoryBase> lazyRepo, bool isCrud)
    {
        EntityType = entityType;
        RepoClosedGenericType = repoClosedGenericType;
        IsCrud = isCrud;
        LazyRepo = lazyRepo;
    }
    internal Type EntityType { get; }
    internal Type RepoClosedGenericType { get; }
    internal bool IsCrud { get; }
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
