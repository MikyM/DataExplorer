﻿using System.Linq.Expressions;
using DataExplorer.Abstractions.DataContexts;
using DataExplorer.Abstractions.Entities;
using DataExplorer.Abstractions.Repositories;
using DataExplorer.Abstractions.Specifications;
using DataExplorer.Entities;
using Remora.Results;

namespace DataExplorer.Abstractions.DataServices;

/// <summary>
/// Read-only data service.
/// </summary>
/// <typeparam name="TEntity">Type of the entity to create the service for, must derive from <see cref="IEntity"/>.</typeparam>
/// <typeparam name="TContext">Type of the <see cref="IDataContextBase"/> to use.</typeparam>
/// <typeparam name="TId">Type of the Id of the entity.</typeparam>
[PublicAPI]
public interface IReadOnlyDataServiceBase<TEntity, TId, out TContext> : IDataServiceBase<TContext>
    where TEntity : Entity<TId>
    where TContext : class, IDataContextBase
    where TId : IComparable, IEquatable<TId>, IComparable<TId>
{
    /// <summary>
    /// The underlying repository.
    /// </summary>
    IReadOnlyRepositoryBase<TEntity,TId> ReadOnlyRepository { get; }
        
    /// <summary>
    /// Gets an entity based on given primary key values.
    /// </summary>
    /// <param name="keyValues">Primary key values.</param>
    /// <returns><see cref="Result"/> containing the result of this operation, with the found entity if any.</returns>
    Task<Result<TEntity>> GetAsync(params object[] keyValues);
    
    /// <summary>
    /// Gets an entity based on given primary key values.
    /// </summary>
    /// <param name="keyValues">Primary key values.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><see cref="Result"/> containing the result of this operation, with the found entity if any.</returns>
    Task<Result<TEntity>> GetAsync(object?[]? keyValues, CancellationToken cancellationToken);

    /// <summary>
    /// Gets an entity based on given primary key values and maps it to another type.
    /// </summary>
    /// 
    /// <param name="keyValues">Primary key values.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><see cref="Result"/> containing the result of this operation, with the found entity if any.</returns>
    Task<Result<TGetResult>> GetAsync<TGetResult>(object?[]? keyValues, CancellationToken cancellationToken = default)
        where TGetResult : class;

    /// <summary>
    /// Gets an entity based on given <see cref="ISpecification"/>.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <param name="specification">Specification with query settings.</param>
    /// <returns><see cref="Result"/> containing the result of this operation, with the found entity if any.</returns>
    Task<Result<TEntity>> GetSingleBySpecAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets an entity based on given <see cref="ISpecification{T}"/> and maps it to another type.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <param name="specification">Specification with query settings.</param>
    /// <returns><see cref="Result"/> containing the result of this operation, with the found entity if any.</returns>
    Task<Result<TGetResult>> GetSingleBySpecAsync<TGetResult>(ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
        where TGetResult : class;

    /// <summary>
    /// Gets an entity based on given <see cref="ISpecification{T,TProjectTo}"/> and projects it to another type using AutoMappers ProjectTo method.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <param name="specification">Specification with query settings.</param>
    /// <returns><see cref="Result"/> containing the result of this operation, with the found entity if any.</returns>
    Task<Result<TGetProjectedResult>> GetSingleBySpecAsync<TGetProjectedResult>(ISpecification<TEntity, TGetProjectedResult> specification, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets an entity based on given <see cref="ISpecification"/>.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <param name="specification">Specification with query settings.</param>
    /// <returns><see cref="Result"/> containing the result of this operation, with the found entity if any.</returns>
    Task<Result<TEntity>> GetSingleAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets an entity based on given <see cref="ISpecification{T}"/> and maps it to another type.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <param name="specification">Specification with query settings.</param>
    /// <returns><see cref="Result"/> containing the result of this operation, with the found entity if any.</returns>
    Task<Result<TGetResult>> GetSingleAsync<TGetResult>(ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
        where TGetResult : class;

    /// <summary>
    /// Gets an entity based on given <see cref="ISpecification{T,TProjectTo}"/> and projects it to another type using AutoMappers ProjectTo method.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <param name="specification">Specification with query settings.</param>
    /// <returns><see cref="Result"/> containing the result of this operation, with the found entity if any.</returns>
    Task<Result<TGetProjectedResult>> GetSingleAsync<TGetProjectedResult>(ISpecification<TEntity, TGetProjectedResult> specification, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets entities based on given <see cref="ISpecification{T}"/>.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <param name="specification">Specification with query settings.</param>
    /// <returns><see cref="Result"/> with <see cref="IReadOnlyList{T}"/> containing the result of this operation, with the found entities if any.</returns>
    Task<Result<IReadOnlyList<TEntity>>> GetBySpecAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets entities based on given <see cref="ISpecification{T}"/> and maps them to another type.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <param name="specification">Specification with query settings.</param>
    /// <returns><see cref="Result"/> with <see cref="IReadOnlyList{T}"/> containing the result of this operation, with the found entities if any.</returns>
    Task<Result<IReadOnlyList<TGetResult>>> GetBySpecAsync<TGetResult>(ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
        where TGetResult : class;

    /// <summary>
    /// Gets entities based on given <see cref="ISpecification{T,TProjectTo}"/> and projects them to another type using AutoMappers ProjectTo method.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <param name="specification">Specification with query settings.</param>
    /// <returns><see cref="Result"/> with <see cref="IReadOnlyList{T}"/> containing the result of this operation, with the found entities if any.</returns>
    Task<Result<IReadOnlyList<TGetProjectedResult>>> GetBySpecAsync<TGetProjectedResult>(ISpecification<TEntity, TGetProjectedResult> specification, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets entities based on given <see cref="ISpecification{T}"/>.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <param name="specification">Specification with query settings.</param>
    /// <returns><see cref="Result"/> with <see cref="IReadOnlyList{T}"/> containing the result of this operation, with the found entities if any.</returns>
    Task<Result<IReadOnlyList<TEntity>>> GetAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets entities based on given <see cref="ISpecification{T}"/> and maps them to another type.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <param name="specification">Specification with query settings.</param>
    /// <returns><see cref="Result"/> with <see cref="IReadOnlyList{T}"/> containing the result of this operation, with the found entities if any.</returns>
    Task<Result<IReadOnlyList<TGetResult>>> GetAsync<TGetResult>(ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
        where TGetResult : class;

    /// <summary>
    /// Gets entities based on given <see cref="ISpecification{T,TProjectTo}"/> and projects them to another type using AutoMappers ProjectTo method.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <param name="specification">Specification with query settings.</param>
    /// <returns><see cref="Result"/> with <see cref="IReadOnlyList{T}"/> containing the result of this operation, with the found entities if any.</returns>
    Task<Result<IReadOnlyList<TGetProjectedResult>>> GetAsync<TGetProjectedResult>(ISpecification<TEntity, TGetProjectedResult> specification, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets all entities and maps them to another type.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <param name="shouldProject">Whether to use AutoMappers ProjectTo method.</param>
    /// <returns><see cref="Result"/> with <see cref="IReadOnlyList{T}"/> containing the result of this operation, with the found entities if any.</returns>
    Task<Result<IReadOnlyList<TGetResult>>> GetAllAsync<TGetResult>(bool shouldProject = false, CancellationToken cancellationToken = default)
        where TGetResult : class;

    /// <summary>
    /// Gets all entities.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><see cref="Result"/> with <see cref="IReadOnlyList{T}"/> containing the result of this operation, with the found entities if any.</returns>
    Task<Result<IReadOnlyList<TEntity>>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Counts the entities with optional query parameters set by passing a <see cref="ISpecification{T}"/>.
    /// </summary>
    /// <param name="specification">Specification with query settings.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><see cref="Result"/> with <see cref="IReadOnlyList{T}"/> containing the result of this operation.</returns>
    Task<Result<long>> LongCountAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Counts the entities with optional query parameters set by passing a <see cref="ISpecification{T}"/>.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><see cref="Result"/> with <see cref="IReadOnlyList{T}"/> containing the result of this operation.</returns>
    Task<Result<long>> LongCountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously determines whether any elements satisfy the condition.
    /// </summary>
    /// <param name="predicate">Predicate for the query.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if any elements in the source sequence satisfy the condition, otherwise false.</returns>
    Task<Result<bool>> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously determines whether any elements satisfy the condition.
    /// </summary>
    /// <param name="specification">Specification for the query.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if any elements in the source sequence satisfy the condition, otherwise false.</returns>
    Task<Result<bool>> AnyAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Finds all entities of <typeparamref name="TEntity" />, that matches the encapsulated query logic of the
    /// <paramref name="specification"/>, from the database.
    /// </summary>
    /// <param name="specification">The encapsulated query logic.</param>
    /// <returns>
    ///  Returns an IAsyncEnumerable which can be enumerated asynchronously.
    /// </returns>
    Result<IAsyncEnumerable<TEntity>> AsAsyncEnumerable(ISpecification<TEntity> specification);
}

/// <summary>
/// Read-only data service.
/// </summary>
/// <typeparam name="TEntity">Type of the entity to create the service for, must derive from <see cref="IEntity{TId}"/>.</typeparam>
/// <typeparam name="TContext">Type of the <see cref="IDataContextBase"/> to use.</typeparam>
[PublicAPI]
public interface IReadOnlyDataServiceBase<TEntity, out TContext> : IReadOnlyDataServiceBase<TEntity, long, TContext>
    where TEntity : Entity<long> where TContext : class, IDataContextBase
{
}
