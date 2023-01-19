using DataExplorer.Abstractions.DataContexts;
using DataExplorer.Abstractions.Repositories;
using DataExplorer.Entities;

namespace DataExplorer.Abstractions.UnitOfWork;

/// <summary>
/// Defines a base Unit of Work.
/// </summary>
[PublicAPI]
public interface IUnitOfWorkBase : IDisposable
{
    /// <summary>
    /// Gets a repository of a given type.
    /// </summary>
    /// <typeparam name="TRepository">Type of the repository to get.</typeparam>
    /// <returns>Wanted repository</returns>
    TRepository GetRepository<TRepository>() where TRepository : class, IRepositoryBase;
    
    /// <summary>
    /// Gets a repository for an entity of a given type.
    /// </summary>
    /// <typeparam name="TEntity">Type of the entity for which the repository should be retrieved.</typeparam>
    /// <returns>The searched for repository.</returns>
    /// <exception cref="InvalidOperationException">Thrown when couldn't find proper type or name in cache.</exception>
    IRepositoryBase GetRepositoryFor<TEntity>() where TEntity : Entity<long>;
    
    /// <summary>
    /// Gets a repository for an entity of a given type and Id type.
    /// </summary>
    /// <typeparam name="TEntity">Type of the entity for which the repository should be retrieved.</typeparam>
    /// <typeparam name="TId">Type of the Id of the entity.</typeparam>
    /// <returns>The searched for repository.</returns>
    /// <exception cref="InvalidOperationException">Thrown when couldn't find proper type or name in cache.</exception>
    IRepositoryBase GetRepositoryFor<TEntity, TId>() where TEntity : Entity<TId>
        where TId : IComparable, IEquatable<TId>, IComparable<TId>;

    /// <summary>
    /// Commits pending changes to the underlying database.
    /// </summary>
    /// <returns>Number of affected rows.</returns>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task CommitAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Commits pending changes to the underlying database.
    /// </summary>
    /// <param name="userId">Id of the user that is responsible for doing changes.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Number of affected rows.</returns>
    Task CommitAsync(string userId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Rolls the current transaction back.
    /// </summary>
    /// <returns>Task representing the asynchronous operation.</returns>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task RollbackAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// The current data context.
    /// </summary>
    IDataContextBase Context { get; }
}
