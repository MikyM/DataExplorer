using AutoMapper;
using DataExplorer.EfCore.Gridify;
using Microsoft.EntityFrameworkCore.Storage;
using ISpecificationEvaluator = DataExplorer.EfCore.Specifications.Evaluators.ISpecificationEvaluator;

namespace DataExplorer.EfCore.Abstractions;

/// <summary>
/// Represents an EF Unit of Work.
/// </summary>
/// <remarks>This also works as a factory for <see cref="IReadOnlyRepository{TEntity,TId}"/>, <see cref="IReadOnlyRepository{TEntity}"/>,
/// , <see cref="IRepository{TEntity,TId}"/> and <see cref="IRepository{TEntity}"/></remarks>
[PublicAPI]
public interface IUnitOfWork : IUnitOfWorkBase
{
    /// <summary>
    /// Begins a transaction or returns an ongoing transaction.
    /// </summary>
    /// <returns>Task representing the asynchronous operation.</returns>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<IDbContextTransaction> UseExplicitTransactionAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Inner <see cref="IDbContextTransaction"/>.
    /// </summary>
    IDbContextTransaction? Transaction { get; }

    /// <summary>
    /// Begins using a provided transaction.
    /// </summary>
    /// <returns>Task representing the asynchronous operation.</returns>
    /// <param name="transaction">Transaction to use.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<IDbContextTransaction> UseExplicitTransactionAsync(IDbContextTransaction transaction, CancellationToken cancellationToken = default);
    
    /// <inheritdoc cref="IUnitOfWorkBase.CommitAsync(CancellationToken)"/>
    /// <returns>Number of affected rows. If a transaction was involved 0 will be returned.</returns>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<int> CommitWithCountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets an <see cref="IRepository{TEntity}"/> for an entity of a given type.
    /// </summary>
    /// <typeparam name="TEntity">Type of the entity for which the repository should be retrieved.</typeparam>
    /// <returns>The searched for repository.</returns>
    /// <exception cref="InvalidOperationException">Thrown when couldn't find proper type or name in cache.</exception>
    IRepository<TEntity> GetRepositoryFor<TEntity>() where TEntity : Entity<long>;
    
    /// <summary>
    /// Gets an <see cref="IReadOnlyRepository{TEntity}"/> for an entity of a given type.
    /// </summary>
    /// <typeparam name="TEntity">Type of the entity for which the repository should be retrieved.</typeparam>
    /// <returns>The searched for repository.</returns>
    /// <exception cref="InvalidOperationException">Thrown when couldn't find proper type or name in cache.</exception>
    IReadOnlyRepository<TEntity> GetReadOnlyRepositoryFor<TEntity>() where TEntity : Entity<long>;

    /// <summary>
    /// Gets an <see cref="IRepository{TEntity,TId}"/> for an entity of a given type and Id type.
    /// </summary>
    /// <typeparam name="TEntity">Type of the entity for which the repository should be retrieved.</typeparam>
    /// <typeparam name="TId">Type of the Id of the entity.</typeparam>
    /// <returns>The searched for repository.</returns>
    /// <exception cref="InvalidOperationException">Thrown when couldn't find proper type or name in cache.</exception>
    IRepository<TEntity, TId> GetRepositoryFor<TEntity, TId>() where TEntity : Entity<TId>
        where TId : IComparable, IEquatable<TId>, IComparable<TId>;
    
    /// <summary>
    /// Gets an <see cref="IReadOnlyRepository{TEntity,TId}"/> for an entity of a given type and Id type.
    /// </summary>
    /// <typeparam name="TEntity">Type of the entity for which the repository should be retrieved.</typeparam>
    /// <typeparam name="TId">Type of the Id of the entity.</typeparam>
    /// <returns>The searched for repository.</returns>
    /// <exception cref="InvalidOperationException">Thrown when couldn't find proper type or name in cache.</exception>
    IReadOnlyRepository<TEntity, TId> GetReadOnlyRepositoryFor<TEntity, TId>() where TEntity : Entity<TId>
        where TId : IComparable, IEquatable<TId>, IComparable<TId>;
    
    /// <summary>
    /// Mapper instance.
    /// </summary>
    IMapper Mapper { get; }
    
    /// <summary>
    /// Specification evaluator instance.
    /// </summary>
    ISpecificationEvaluator SpecificationEvaluator { get; }
    
    /// <summary>
    /// Gridify mapper provider instance.
    /// </summary>
    IGridifyMapperProvider GridifyMapperProvider { get; }
}

/// <inheritdoc cref="IUnitOfWork"/>
/// <summary>
/// Represents an EF Unit of Work with a given context.
/// </summary>
/// <remarks>This also works as a factory for <see cref="IReadOnlyRepository{TEntity,TId}"/>, <see cref="IReadOnlyRepository{TEntity}"/>,
/// , <see cref="IRepository{TEntity,TId}"/> and <see cref="IRepository{TEntity}"/></remarks>
/// <typeparam name="TContext">Type of context to be used.</typeparam>
[PublicAPI]
public interface IUnitOfWork<out TContext> : IUnitOfWork where TContext : IEfDbContext
{
    /// <summary>
    /// Current <see cref="DbContext"/>.
    /// </summary>
    new TContext Context { get; }
}
