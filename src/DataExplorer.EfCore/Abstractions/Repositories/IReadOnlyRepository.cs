﻿using System.Linq.Expressions;
using DataExplorer.EfCore.Gridify;
using DataExplorer.EfCore.Specifications;
using Gridify;
using ISpecificationEvaluator = DataExplorer.EfCore.Specifications.Evaluators.ISpecificationEvaluator;

namespace DataExplorer.EfCore.Abstractions.Repositories;

/// <summary>
/// Read-only repository.
/// </summary>
/// <typeparam name="TEntity">Entity that derives from <see cref="Entity{TId}"/>.</typeparam>
/// <typeparam name="TId">Type of the Id in <typeparamref name="TEntity"/>.</typeparam>
[PublicAPI]
public interface IReadOnlyRepository<TEntity,TId> : IRepositoryBase where TEntity : Entity<TId> where TId : IComparable, IEquatable<TId>, IComparable<TId>
{
    /// <summary>
    /// Current <see cref="IEfDbContext"/>.
    /// </summary>
    new IEfDbContext Context { get; }
    
    /// <summary>
    /// Gridify mapper provider.
    /// </summary>
    IGridifyMapperProvider GridifyMapperProvider { get; }
    
    /// <summary>
    /// Specification evaluator.
    /// </summary>
    ISpecificationEvaluator SpecificationEvaluator { get; }
    
    /// <summary>
    /// Current <see cref="DbSet{TEntity}"/>.
    /// </summary>
    DbSet<TEntity> Set { get; }
    
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
    /// Gets a single (top 1) entity that satisfies given <see cref="Specifications.ISpecification"/>.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <param name="specification">Specification for the query.</param>
    /// <returns>Entity if found, null if not found.</returns>
    Task<TEntity?> GetSingleBySpecAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a single (top 1) entity that satisfies <see cref="Specifications.ISpecification{T,TProjectTo}"/> and projects it to another entity.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <param name="specification">Specification for the query.</param>
    /// <returns>Entity if found, null if not found.</returns>
    Task<TResult?> GetSingleBySpecAsync<TResult>(ISpecification<TEntity, TResult> specification,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets a single (top 1) entity that satisfies given <see cref="Specifications.ISpecification"/>.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <param name="specification">Specification for the query.</param>
    /// <returns>Entity if found, null if not found.</returns>
    Task<TEntity?> GetSingleAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a single (top 1) entity that satisfies <see cref="Specifications.ISpecification{T,TProjectTo}"/> and projects it to another entity.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <param name="specification">Specification for the query.</param>
    /// <returns>Entity if found, null if not found.</returns>
    Task<TResult?> GetSingleAsync<TResult>(ISpecification<TEntity, TResult> specification,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all entities that satisfy given <see cref="Specifications.ISpecification{T}"/>.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <param name="specification">Specification for the query.</param>
    /// <returns><see cref="IReadOnlyList{T}"/> with found entities.</returns>
    Task<IReadOnlyList<TEntity>> GetBySpecAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all entities that satisfy <see cref="Specifications.ISpecification{T,TProjectTo}"/> and projects them to another entities.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <param name="specification">Specification for the query.</param>
    /// <returns><see cref="IReadOnlyList{T}"/> with found entities.</returns>
    Task<IReadOnlyList<TResult>> GetBySpecAsync<TResult>(ISpecification<TEntity, TResult> specification,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets all entities that satisfy given <see cref="Specifications.ISpecification{T}"/>.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <param name="specification">Specification for the query.</param>
    /// <returns><see cref="IReadOnlyList{T}"/> with found entities.</returns>
    Task<IReadOnlyList<TEntity>> GetAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all entities that satisfy <see cref="Specifications.ISpecification{T,TProjectTo}"/> and projects them to another entities.
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
    /// Counts the entities that satisfy the given <see cref="Specifications.ISpecification{T}"/>.
    /// </summary>
    /// <param name="predicate">Predicate for the query.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Number of entities that satisfy given <see cref="Specifications.ISpecification{T}"/>.</returns>
    Task<long> LongCountAsync(Expression<Func<TEntity,bool>> predicate, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Counts the entities that satisfy the given <see cref="Specifications.ISpecification{T}"/>.
    /// </summary>
    /// <param name="specification">Specification for the query.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Number of entities that satisfy given <see cref="Specifications.ISpecification{T}"/>.</returns>
    Task<long> LongCountAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Counts the entities, optionally using a provided <see cref="Specifications.ISpecification{T}"/>.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Number of entities that satisfy given <see cref="Specifications.ISpecification{T}"/>.</returns>
    Task<long> LongCountAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Asynchronously determines whether any elements satisfy the given condition.
    /// </summary>
    /// <param name="predicate">Predicate for the query.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if any elements in the source sequence satisfy the condition, otherwise false.</returns>
    Task<bool> AnyAsync(Expression<Func<TEntity,bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously determines whether any elements satisfy the given <see cref="Specifications.ISpecification{TEntity}"/>.
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
    
    /// <summary>
    /// Gets all entities that satisfy given <see cref="IGridifyQuery"/>.
    /// </summary>
    /// <remarks>
    /// This method will attempt to pull a <see cref="IGridifyMapper{T}"/> from <see cref="IGridifyMapperProvider"/>.
    /// </remarks>
    /// <param name="gridifyQuery">Gridify query query.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><see cref="Paging{T}"/> with found entities and count.</returns>
    Task<Paging<TEntity>> GetByGridifyQueryAsync(IGridifyQuery gridifyQuery,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all entities that satisfy given <see cref="IGridifyQuery"/>.
    /// </summary>
    /// <param name="gridifyQuery">Gridify query query.</param>
    /// <param name="gridifyMapper">Gridify mapper</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><see cref="Paging{T}"/> with found entities and count.</returns>
    Task<Paging<TEntity>> GetByGridifyQueryAsync(IGridifyQuery gridifyQuery,
        IGridifyMapper<TEntity> gridifyMapper, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets all entities that satisfy given <see cref="IGridifyQuery"/> and projects them to another type using AutoMapper's ProjectTo.
    /// </summary>
    /// <remarks>
    /// This method will attempt to pull a <see cref="IGridifyMapper{T}"/> from <see cref="IGridifyMapperProvider"/>.
    /// </remarks>
    /// <param name="gridifyQuery">Gridify query query.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><see cref="Paging{T}"/> with found entities and count.</returns>
    Task<Paging<TResult>> GetByGridifyQueryAsync<TResult>(IGridifyQuery gridifyQuery,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all entities that satisfy given <see cref="IGridifyQuery"/> and projects them to another type using AutoMapper's ProjectTo.
    /// </summary>
    /// <param name="gridifyQuery">Gridify query query.</param>
    /// <param name="gridifyMapper">Gridify mapper</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><see cref="Paging{T}"/> with found entities and count.</returns>
    Task<Paging<TResult>> GetByGridifyQueryAsync<TResult>(IGridifyQuery gridifyQuery,
        IGridifyMapper<TEntity> gridifyMapper, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets all entities that satisfy given <see cref="IGridifyQuery"/>.
    /// </summary>
    /// <remarks>
    /// This method will attempt to pull a <see cref="IGridifyMapper{T}"/> from <see cref="IGridifyMapperProvider"/>.
    /// </remarks>
    /// <param name="gridifyQuery">Gridify query query.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><see cref="Paging{T}"/> with found entities and count.</returns>
    Task<Paging<TEntity>> GetAsync(IGridifyQuery gridifyQuery,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all entities that satisfy given <see cref="IGridifyQuery"/>.
    /// </summary>
    /// <param name="gridifyQuery">Gridify query query.</param>
    /// <param name="gridifyMapper">Gridify mapper</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><see cref="Paging{T}"/> with found entities and count.</returns>
    Task<Paging<TEntity>> GetAsync(IGridifyQuery gridifyQuery,
        IGridifyMapper<TEntity> gridifyMapper, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets all entities that satisfy given <see cref="IGridifyQuery"/> and projects them to another type using AutoMapper's ProjectTo.
    /// </summary>
    /// <remarks>
    /// This method will attempt to pull a <see cref="IGridifyMapper{T}"/> from <see cref="IGridifyMapperProvider"/>.
    /// </remarks>
    /// <param name="gridifyQuery">Gridify query query.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><see cref="Paging{T}"/> with found entities and count.</returns>
    Task<Paging<TResult>> GetAsync<TResult>(IGridifyQuery gridifyQuery,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all entities that satisfy given <see cref="IGridifyQuery"/> and projects them to another type using AutoMapper's ProjectTo.
    /// </summary>
    /// <param name="gridifyQuery">Gridify query query.</param>
    /// <param name="gridifyMapper">Gridify mapper</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><see cref="Paging{T}"/> with found entities and count.</returns>
    Task<Paging<TResult>> GetAsync<TResult>(IGridifyQuery gridifyQuery,
        IGridifyMapper<TEntity> gridifyMapper, CancellationToken cancellationToken = default);
}

/// <summary>
/// Read-only repository.
/// </summary>
/// <typeparam name="TEntity">Entity that derives from <see cref="Entity{TId}"/>.</typeparam>
[PublicAPI]
public interface IReadOnlyRepository<TEntity> : IReadOnlyRepository<TEntity,long> where TEntity : Entity<long>
{
}
