using System.Linq.Expressions;
using DataExplorer.Abstractions.Specifications;
using DataExplorer.Abstractions.Specifications.Evaluators;
using DataExplorer.Entities;

namespace DataExplorer.Abstractions.Repositories;

/// <summary>
/// Read-only repository.
/// </summary>
/// <typeparam name="TEntity">Entity that derives from <see cref="Entity"/>.</typeparam>
/// <typeparam name="TId">Type of the Id in <typeparamref name="TEntity"/>.</typeparam>
[PublicAPI]
public interface IReadOnlyRepositoryBase<TEntity,TId> : IBaseRepository where TEntity : Entity<TId> where TId : IComparable, IEquatable<TId>, IComparable<TId>
{
    /// <summary>
    /// Specification evaluator.
    /// </summary>
    ISpecificationEvaluator SpecificationEvaluator { get; }
    
    /// <summary>
    /// Gets an entity based on given primary key values.
    /// </summary>
    /// <param name="keyValues">Primary key values.</param>
    /// <returns>Entity if found, null if not found.</returns>
    ValueTask<TEntity?> GetAsync(params object[] keyValues);
    
    /// <summary>
    /// Gets an entity based on given primary key values.
    /// </summary>
    /// <param name="keyValues">Primary key values.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Entity if found, null if not found.</returns>
    ValueTask<TEntity?> GetAsync(object?[]? keyValues, CancellationToken cancellationToken);

    /// <summary>
    /// Gets a single (top 1) entity that satisfies given <see cref="ISpecification"/>.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <param name="specification">Specification for the query.</param>
    /// <returns>Entity if found, null if not found.</returns>
    Task<TEntity?> GetSingleBySpecAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a single (top 1) entity that satisfies <see cref="ISpecification{T,TProjectTo}"/> and projects it to another entity.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <param name="specification">Specification for the query.</param>
    /// <returns>Entity if found, null if not found.</returns>
    Task<TResult?> GetSingleBySpecAsync<TResult>(ISpecification<TEntity, TResult> specification,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets a single (top 1) entity that satisfies given <see cref="ISpecification"/>.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <param name="specification">Specification for the query.</param>
    /// <returns>Entity if found, null if not found.</returns>
    Task<TEntity?> GetSingleAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a single (top 1) entity that satisfies <see cref="ISpecification{T,TProjectTo}"/> and projects it to another entity.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <param name="specification">Specification for the query.</param>
    /// <returns>Entity if found, null if not found.</returns>
    Task<TResult?> GetSingleAsync<TResult>(ISpecification<TEntity, TResult> specification,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all entities that satisfy given <see cref="ISpecification{T}"/>.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <param name="specification">Specification for the query.</param>
    /// <returns><see cref="IReadOnlyList{T}"/> with found entities.</returns>
    Task<IReadOnlyList<TEntity>> GetBySpecAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all entities that satisfy <see cref="ISpecification{T,TProjectTo}"/> and projects them to another entities.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <param name="specification">Specification for the query.</param>
    /// <returns><see cref="IReadOnlyList{T}"/> with found entities.</returns>
    Task<IReadOnlyList<TResult>> GetBySpecAsync<TResult>(ISpecification<TEntity, TResult> specification,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets all entities that satisfy given <see cref="ISpecification{T}"/>.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <param name="specification">Specification for the query.</param>
    /// <returns><see cref="IReadOnlyList{T}"/> with found entities.</returns>
    Task<IReadOnlyList<TEntity>> GetAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all entities that satisfy <see cref="ISpecification{T,TProjectTo}"/> and projects them to another entities.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <param name="specification">Specification for the query.</param>
    /// <returns><see cref="IReadOnlyList{T}"/> with found entities.</returns>
    Task<IReadOnlyList<TResult>> GetAsync<TResult>(ISpecification<TEntity, TResult> specification,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all entities.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><see cref="IReadOnlyList{T}"/> with all entities.</returns>
    Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all entities and projects them to another entity.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><see cref="IReadOnlyList{T}"/> with all entities.</returns>
    Task<IReadOnlyList<TProjectTo>> GetAllAsync<TProjectTo>(CancellationToken cancellationToken = default) where TProjectTo : class;

    /// <summary>
    /// Counts the entities that satisfy the given <see cref="ISpecification{T}"/>.
    /// </summary>
    /// <param name="predicate">Predicate for the query.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Number of entities that satisfy given <see cref="ISpecification{T}"/>.</returns>
    Task<long> LongCountAsync(Expression<Func<TEntity,bool>> predicate, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Counts the entities that satisfy the given <see cref="ISpecification{T}"/>.
    /// </summary>
    /// <param name="specification">Specification for the query.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Number of entities that satisfy given <see cref="ISpecification{T}"/>.</returns>
    Task<long> LongCountAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Counts the entities, optionally using a provided <see cref="ISpecification{T}"/>.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Number of entities that satisfy given <see cref="ISpecification{T}"/>.</returns>
    Task<long> LongCountAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Asynchronously determines whether any elements satisfy the given condition.
    /// </summary>
    /// <param name="predicate">Predicate for the query.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if any elements in the source sequence satisfy the condition, otherwise false.</returns>
    Task<bool> AnyAsync(Expression<Func<TEntity,bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously determines whether any elements satisfy the given <see cref="ISpecification{TEntity}"/>.
    /// </summary>
    /// <param name="specification">Specification for the query.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if any elements in the source sequence satisfy the condition, otherwise false.</returns>
    Task<bool> AnyAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);

    /// <summary>
    /// Finds all entities of <typeparamref name="TEntity"/>, that match the encapsulated query logic of the
    /// <paramref name="specification"/>, from the database.
    /// </summary>
    /// <param name="specification">The encapsulated query logic.</param>
    /// <returns>
    ///  Returns an IAsyncEnumerable which can be enumerated asynchronously.
    /// </returns>
    IAsyncEnumerable<TEntity> AsAsyncEnumerable(ISpecification<TEntity> specification);
    
    /// <summary>
    /// Finds all entities of <typeparamref name="TEntity"/>.
    /// </summary>
    /// <returns>
    ///  Returns an IAsyncEnumerable which can be enumerated asynchronously.
    /// </returns>
    IAsyncEnumerable<TEntity> AsAsyncEnumerable();
    
    /// <summary>
    /// Finds all entities of <typeparamref name="TEntity"/>, that match the given predicate.
    /// </summary>
    /// <param name="predicate">Predicate for the query.</param>
    /// <returns>
    ///  Returns an IAsyncEnumerable which can be enumerated asynchronously.
    /// </returns>
    IAsyncEnumerable<TEntity> AsAsyncEnumerable(Expression<Func<TEntity,bool>> predicate);
}

/// <summary>
/// Read-only repository.
/// </summary>
/// <typeparam name="TEntity">Entity that derives from <see cref="Entity{TId}"/>.</typeparam>
[PublicAPI]
public interface IReadOnlyRepositoryBase<TEntity> : IReadOnlyRepositoryBase<TEntity,long> where TEntity : Entity<long>
{
}
