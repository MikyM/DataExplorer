using AutoMapper;
using DataExplorer.Abstractions.Repositories;
using DataExplorer.Abstractions.UnitOfWork;
using DataExplorer.MongoDb.Abstractions.DataContexts;
using DataExplorer.MongoDb.DataContexts;
using MongoDB.Driver;
using MongoDB.Entities;

namespace DataExplorer.MongoDb.Abstractions;

/// <summary>
/// Represents a MongoDB Unit of Work.
/// </summary>
[PublicAPI]
public interface IMongoUnitOfWork : IUnitOfWorkBase
{
    /// <summary>
    /// Begins a transaction.
    /// </summary>
    /// <returns>Task representing the asynchronous operation.</returns>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task UseTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a repository of a given type.
    /// </summary>
    /// <remarks>You can <b>only</b> retrieve types: <see cref="IMongoRepository{TEntity}"/>, <see cref="IMongoRepository{TEntity,TId}"/>, <see cref="IMongoReadOnlyRepository{TEntity}"/> and <see cref="IMongoReadOnlyRepository{TEntity,TId}"/>.</remarks>
    /// <typeparam name="TRepository">Type of the repository to get.</typeparam>
    /// <returns>The searched for repository.</returns>
    /// <exception cref="InvalidOperationException">Thrown when couldn't find proper type or name in cache.</exception>
    /// <exception cref="NotSupportedException">Thrown when passed type isn't equal to any of the types listed in remarks, isn't a generic type or isn't an interface.</exception>
    new TRepository GetRepository<TRepository>() where TRepository : class, IRepositoryBase;

    /// <summary>
    /// Gets an <see cref="IMongoRepository{TEntity}"/> for an entity of a given type.
    /// </summary>
    /// <typeparam name="TEntity">Type of the entity for which the repository should be retrieved.</typeparam>
    /// <returns>The searched for repository.</returns>
    /// <exception cref="InvalidOperationException">Thrown when couldn't find proper type or name in cache.</exception>
    IMongoRepository<TEntity> GetRepositoryFor<TEntity>() where TEntity : MongoEntity<long>;
    
    /// <summary>
    /// Gets an <see cref="IMongoReadOnlyRepository{TEntity}"/> for an entity of a given type.
    /// </summary>
    /// <typeparam name="TEntity">Type of the entity for which the repository should be retrieved.</typeparam>
    /// <returns>The searched for repository.</returns>
    /// <exception cref="InvalidOperationException">Thrown when couldn't find proper type or name in cache.</exception>
    IMongoReadOnlyRepository<TEntity> GetReadOnlyRepositoryFor<TEntity>() where TEntity : MongoEntity<long>;

    /// <summary>
    /// Gets an <see cref="IMongoRepository{TEntity,TId}"/> for an entity of a given type and Id type.
    /// </summary>
    /// <typeparam name="TEntity">Type of the entity for which the repository should be retrieved.</typeparam>
    /// <typeparam name="TId">Type of the Id of the entity.</typeparam>
    /// <returns>The searched for repository.</returns>
    /// <exception cref="InvalidOperationException">Thrown when couldn't find proper type or name in cache.</exception>
    IMongoRepository<TEntity, TId> GetRepositoryFor<TEntity, TId>() where TEntity : MongoEntity<TId>
        where TId : IComparable, IEquatable<TId>, IComparable<TId>;
    
    /// <summary>
    /// Gets an <see cref="IMongoReadOnlyRepository{TEntity,TId}"/> for an entity of a given type and Id type.
    /// </summary>
    /// <typeparam name="TEntity">Type of the entity for which the repository should be retrieved.</typeparam>
    /// <typeparam name="TId">Type of the Id of the entity.</typeparam>
    /// <returns>The searched for repository.</returns>
    /// <exception cref="InvalidOperationException">Thrown when couldn't find proper type or name in cache.</exception>
    IMongoReadOnlyRepository<TEntity, TId> GetReadOnlyRepositoryFor<TEntity, TId>() where TEntity : MongoEntity<TId>
        where TId : IComparable, IEquatable<TId>, IComparable<TId>;
    
    /// <summary>
    /// Mapper instance.
    /// </summary>
    IMapper Mapper { get; }
}

/// <inheritdoc cref="IMongoUnitOfWork"/>
/// <summary>
/// Represents a MongoDB Unit of Work.
/// </summary>
/// <typeparam name="TContext">Type of context to be used.</typeparam>
[PublicAPI]
public interface IMongoUnitOfWork<out TContext> : IMongoUnitOfWork where TContext : IMongoDbContext
{
    /// <summary>
    /// Current <see cref="MongoDbContext"/>.
    /// </summary>
    new TContext Context { get; }
    
    IClientSessionHandle? Transaction { get; }
}
