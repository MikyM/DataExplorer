﻿using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using DataExplorer.Abstractions.Mapper;
using DataExplorer.Abstractions.Specifications.Evaluators;
using DataExplorer.EfCore.Specifications.Evaluators;
using Microsoft.EntityFrameworkCore.Storage;

namespace DataExplorer.EfCore;

/// <inheritdoc cref="IUnitOfWork"/>
[PublicAPI]
public sealed class UnitOfWork<TContext> : IUnitOfWork<TContext> where TContext : IEfDbContext
{
    /// <summary>
    /// Inner <see cref="IEfSpecificationEvaluator"/>.
    /// </summary>
    public IEfSpecificationEvaluator SpecificationEvaluator { get; }
    
    /// <inheritdoc />
    ISpecificationEvaluator IUnitOfWorkBase.SpecificationEvaluator => SpecificationEvaluator;
    
    /// <summary>
    /// Configuration.
    /// </summary>
    private readonly DataExplorerEfCoreConfiguration _options;

    // To detect redundant calls
    private bool _disposed;
    // ReSharper disable once InconsistentNaming

    /// <summary>
    /// Repositories.
    /// </summary>
    private ConcurrentDictionary<RepositoryEntryKey,RepositoryEntry>? _repositories;

    /// <summary>
    /// Inner <see cref="IDbContextTransaction"/>.
    /// </summary>
    public IDbContextTransaction? Transaction { get; private set; }

    /// <summary>
    /// Mapper instance.
    /// </summary>
    public IMapper Mapper { get; }

    /// <summary>
    /// Data cache.
    /// </summary>
    private readonly IEfDataExplorerTypeCache _cache;

    /// <summary>
    /// Instance factory.
    /// </summary>
    private readonly ICachedInstanceFactory _instanceFactory;

    /// <summary>
    /// Creates a new instance of <see cref="UnitOfWork{TContext}"/>.
    /// </summary>
    /// <param name="context"><see cref="DbContext"/> to be used.</param>
    /// <param name="specificationEvaluator">Specification evaluator to be used.</param>
    /// <param name="mapper">Mapper.</param>
    /// <param name="options">Options.</param>
    /// <param name="efDataExplorerTypeCache">Unit of Work cache.</param>
    /// <param name="instanceFactory">Instance factory.</param>
    public UnitOfWork(TContext context, IEfSpecificationEvaluator specificationEvaluator, 
        IMapper mapper, DataExplorerEfCoreConfiguration options, IEfDataExplorerTypeCache efDataExplorerTypeCache, 
        ICachedInstanceFactory instanceFactory)
    {
        Context = context;
        SpecificationEvaluator = specificationEvaluator;
        _options = options;
        Mapper = mapper;
        _cache = efDataExplorerTypeCache;
        _instanceFactory = instanceFactory;
    }

    /// <inheritdoc />
    public TContext Context { get; }
    
    private void CheckDisposed()
    {
#if NET7_0_OR_GREATER
        ObjectDisposedException.ThrowIf(_disposed, this);
#else
        if (_disposed)
            throw new ObjectDisposedException(nameof(UnitOfWork<TContext>));
#endif
    }

    /// <inheritdoc />
    public async Task<IDbContextTransaction> UseExplicitTransactionAsync(CancellationToken cancellationToken = default)
    {
        CheckDisposed();
        
        if (Transaction is not null)
            return Transaction;
        
        Transaction ??= await Context.Database.BeginTransactionAsync(cancellationToken);

        return Transaction;
    }
    
    /// <inheritdoc />
    public Task<IDbContextTransaction> UseExplicitTransactionAsync(IDbContextTransaction transaction, CancellationToken cancellationToken = default)
    {
        CheckDisposed();
        
        Transaction = transaction;

        return Task.FromResult(Transaction);
    }

    /// <inheritdoc cref="IUnitOfWork.GetRepositoryFor{TRepository}" />
    public IRepository<TEntity> GetRepositoryFor<TEntity>() where TEntity : Entity<long>
    {
        CheckDisposed();
        
        var entityType = typeof(TEntity);
        if (!_cache.TryGetEntityInfo(entityType, out var info))
            ThrowUnknownEntity(entityType.Name);
        
        if (info.CrudLongIdRepoInfo is null)
            ThrowIncompatibleEntityId($"IRepository<{entityType.Name}>");
        
        return LazilyGetOrCreateRepository<IRepository<TEntity>>(info.CrudLongIdRepoInfo!);
    }

    /// <inheritdoc cref="IUnitOfWork.GetReadOnlyRepositoryFor{TRepository}" />
    public IReadOnlyRepository<TEntity> GetReadOnlyRepositoryFor<TEntity>() where TEntity : Entity<long>
    {
        CheckDisposed();
        
        var entityType = typeof(TEntity);
        if (!_cache.TryGetEntityInfo(entityType, out var info))
            ThrowUnknownEntity(entityType.Name);

        if (info.ReadOnlyLongIdRepoInfo is null)
            ThrowIncompatibleEntityId($"IReadOnlyRepository<{entityType.Name}>");

        return LazilyGetOrCreateRepository<IRepository<TEntity>>(info.ReadOnlyLongIdRepoInfo!);
    }

    /// <inheritdoc cref="IUnitOfWork.GetRepositoryFor{TRepository,TId}" />
    public IRepository<TEntity, TId> GetRepositoryFor<TEntity, TId>() where TEntity : Entity<TId>
        where TId : IComparable, IEquatable<TId>, IComparable<TId>
    {
        CheckDisposed();
        
        var entityType = typeof(TEntity);
        if (!_cache.TryGetEntityInfo(entityType, out var info))
            ThrowUnknownEntity(entityType.Name);
        
        return LazilyGetOrCreateRepository<IRepository<TEntity, TId>>(info.CrudGenericIdRepoInfo);
    }

    /// <inheritdoc cref="IUnitOfWork.GetReadOnlyRepositoryFor{TRepository,TId}" />
    public IReadOnlyRepository<TEntity, TId> GetReadOnlyRepositoryFor<TEntity, TId>() where TEntity : Entity<TId>
        where TId : IComparable, IEquatable<TId>, IComparable<TId>
    {
        CheckDisposed();
        
        var entityType = typeof(TEntity);
        if (!_cache.TryGetEntityInfo(entityType, out var info))
            ThrowUnknownEntity(entityType.Name);
        
        return LazilyGetOrCreateRepository<IReadOnlyRepository<TEntity, TId>>(info.ReadOnlyGenericIdRepoInfo);
    }
        
    /// <inheritdoc/>
    public TRepository GetRepository<TRepository>() where TRepository : class, IBaseRepository
    {
        CheckDisposed();
        
        var repoInterfaceType = typeof(TRepository);
        
        if (!repoInterfaceType.IsInterface || !repoInterfaceType.IsGenericType)
            throw new InvalidOperationException("You can only retrieve types: IRepository<TEntity>, IRepository<TEntity,TId>, IReadOnlyRepository<TEntity> and IReadOnlyRepository<TEntity,TId>.");
        
        if (!_cache.TryGetRepoInfo(repoInterfaceType, out var repoInfo))
            throw new InvalidOperationException($"Unknown repository type {repoInterfaceType.Name}, couldn't find the type in repository cache. Make sure you passed the correct assemblies to scan for entity types during registration.");
        
        return LazilyGetOrCreateRepository<TRepository>(repoInfo);
    }

    /// <summary>
    /// Lazily creates a new repository instance of a given type.
    /// </summary>
    /// <param name="repoCacheData">Repository cache data.</param>
    /// <typeparam name="TRepository">Type of the wanted repository</typeparam>
    /// <returns>Created repo instance.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private TRepository LazilyGetOrCreateRepository<TRepository>(DataExplorerRepoInfo repoCacheData) where TRepository : IBaseRepository
    {
        _repositories ??= new ConcurrentDictionary<RepositoryEntryKey, RepositoryEntry>();
        
        if (ShouldExchangeForSingleArgRepo(repoCacheData, out var singleArgRepoInfo))
        {
            repoCacheData = singleArgRepoInfo;
        }

        var repositoryTypeName = repoCacheData.RepoImplType.FullName ?? repoCacheData.RepoImplType.Name;
        var entityTypeName = repoCacheData.EntityInfo.EntityType.FullName ?? repoCacheData.EntityInfo.EntityType.Name;

        var key = new RepositoryEntryKey(entityTypeName);

        var repositoryEntry = _repositories.AddOrUpdate(key, _ => GetRepositoryEntry(repoCacheData), (_, currentEntry) =>
        {
            // currently the key equals/hashcode methods take only entity type into consideration
            // if it's read only but client wanted crud that's not okay and we have to create the repo manually and swap it within the dictionary
            // so subsequent calls will get the "correct" repo instantly
            // we here ignore the fact that client wanted exactly READ-ONLY repo and we're giving him CRUD repo instead (albeit as read-only interface)
            // this way we can halve the amount of repos our dictionary has to hold
            // we could also just always create a crud repo but I've decided it's better to no overly assume what the client wants
            
            if (repoCacheData.IsCrud && !currentEntry.RepoInfo.IsCrud)
            {
                // we replace the value and return the new one
                return GetRepositoryEntry(repoCacheData);
            }

            // we keep the current value and return it
            return currentEntry;
        });

        return (TRepository)repositoryEntry.LazyRepo.Value;
        
        RepositoryEntry GetRepositoryEntry(DataExplorerRepoInfo data) => new(data, new Lazy<IBaseRepository>(()  =>
        {
            var instance = _instanceFactory.CreateInstance(repoCacheData.RepoImplType, Context,
                SpecificationEvaluator, Mapper);

            if (instance is null)
                throw new InvalidOperationException($"Couldn't create an instance of {repositoryTypeName} repository - please file an issue on GitHub.");

            // ReSharper disable once HeapView.PossibleBoxingAllocation
            return (TRepository)instance;
        }, LazyThreadSafetyMode.ExecutionAndPublication));
        
        // this handles the situation where the client wants a IRepository<TEntity,long> rather than IRepository<TEntity>, we will create the Repository<TEntity> instead (or read-only variation)
        bool ShouldExchangeForSingleArgRepo(DataExplorerRepoInfo repoInfo, [NotNullWhen(true)] out DataExplorerRepoInfo? outputInfo)
        {
            outputInfo = null;

            if (!repoInfo.EntityInfo.HasLongId)
            {
                return false;
            }

            if (repoInfo.IsLongIdVariation)
            {
                return false;
            }

            outputInfo = repoInfo.IsCrud
                ? repoInfo.EntityInfo.CrudLongIdRepoInfo
                : repoInfo.EntityInfo.ReadOnlyLongIdRepoInfo;
            
            return outputInfo is not null;
        }
    }
    
    [DoesNotReturn]
    [DebuggerStepThrough]
    private static void ThrowUnknownEntity(string entity)
    {
        throw new InvalidOperationException($"Unknown entity type {entity}, couldn't find the type in entity cache. Make sure you passed the correct assemblies to scan for entity types during registration.");
    }
    
    [DoesNotReturn]
    [DebuggerStepThrough]
    private static void ThrowIncompatibleEntityId(string type)
    {
        throw new InvalidOperationException($"Can't create {type} repository for the given type as it's ID is incompatible - this type only supports long Ids.");
    }
    
    /// <inheritdoc />
    public Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        CheckDisposed();
        return Transaction is not null ? Transaction.RollbackAsync(cancellationToken) : Task.CompletedTask;
    }

    /// <inheritdoc />
    // ReSharper disable once HeapView.PossibleBoxingAllocation
    IDataContextBase IUnitOfWorkBase.Context => Context;
    
    /// <inheritdoc />
    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        CheckDisposed();
        
        if (Transaction is null)
        {
            _ = await Context.SaveChangesAsync(cancellationToken);
            return;
        }

        await Transaction.CommitAsync(cancellationToken);

        Transaction = null;
    }
    
    /// <inheritdoc />
    public async Task<int> CommitWithCountAsync(CancellationToken cancellationToken = default)
    {
        CheckDisposed();
        
        if (Transaction is null)
            return await Context.SaveChangesAsync(cancellationToken);
        
        await Transaction.CommitAsync(cancellationToken);
        
        Transaction = null;
        
        return 0;
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
            Transaction?.Dispose();
        }

        _repositories = null;

        _disposed = true;
    }
}

/// <summary>
/// Repository entry.
/// </summary>
internal sealed record RepositoryEntry
{
    internal RepositoryEntry(DataExplorerRepoInfo repoCacheData, Lazy<IBaseRepository> lazyRepo)
    {
        RepoInfo = repoCacheData;
        LazyRepo = lazyRepo;
    }
    internal DataExplorerRepoInfo RepoInfo { get; }
    internal Lazy<IBaseRepository> LazyRepo { get; }
}

/// <summary>
/// Repository entry key.
/// </summary>
internal readonly struct RepositoryEntryKey : IEquatable<RepositoryEntryKey>
{
    internal RepositoryEntryKey(string entityTypeName)
    {
        EntityTypeName = entityTypeName;
    }

    private string EntityTypeName { get; }

    public bool Equals(RepositoryEntryKey other)
        => EntityTypeName == other.EntityTypeName;

    public override bool Equals(object? obj)
        => obj is RepositoryEntryKey other && Equals(other);

    public override int GetHashCode()
        => EntityTypeName.GetHashCode();
}
