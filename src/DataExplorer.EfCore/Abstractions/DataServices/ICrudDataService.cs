using DataExplorer.Abstractions.DataServices;
using DataExplorer.Abstractions.Specifications;
using DataExplorer.EfCore.Abstractions.Specifications;
using Remora.Results;

namespace DataExplorer.EfCore.Abstractions.DataServices;

/// <summary>
/// CRUD data service.
/// </summary>
[PublicAPI]
public interface ICrudDataService<TEntity, TId, out TContext> : IReadOnlyDataService<TEntity, TId, TContext>, ICrudDataServiceBase<TEntity, TId, TContext>
    where TEntity : Entity<TId>
    where TContext : class, IEfDbContext
    where TId : IComparable, IEquatable<TId>, IComparable<TId>
{
    /// <summary>
    /// The underlying repository.
    /// </summary>
    new IRepository<TEntity,TId> Repository { get; }
    
#if NET7_0_OR_GREATER 
    /// <summary>
    ///     Asynchronously updates database rows for the entity instances which match the LINQ query generated based on the provided specification from the database.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         This operation executes immediately against the database, rather than being deferred until
    ///         <see cref="DbContext.SaveChanges()" /> is called. It also does not interact with the EF change tracker in any way:
    ///         entity instances which happen to be tracked when this operation is invoked aren't taken into account, and aren't updated
    ///         to reflect the changes.
    ///     </para>
    ///     <para>
    ///         See <see href="https://aka.ms/efcore-docs-bulk-operations">Executing bulk operations with EF Core</see>
    ///         for more information and examples.
    ///     </para>
    /// </remarks>
    /// <param name="specification">Specification for the query.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns>The total number of rows updated in the database.</returns>
    Task<Result<int>> ExecuteUpdateAsync(IUpdateSpecification<TEntity> specification,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    ///     Asynchronously deletes all database rows for the entity instances.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         This operation executes immediately against the database, rather than being deferred until
    ///         <see cref="DbContext.SaveChanges()" /> is called. It also does not interact with the EF change tracker in any way:
    ///         entity instances which happen to be tracked when this operation is invoked aren't taken into account, and aren't updated
    ///         to reflect the changes.
    ///     </para>
    ///     <para>
    ///         See <see href="https://aka.ms/efcore-docs-bulk-operations">Executing bulk operations with EF Core</see>
    ///         for more information and examples.
    ///     </para>
    /// </remarks>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns>The total number of rows deleted in the database.</returns>
    Task<Result<int>> ExecuteDeleteAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    ///     Asynchronously deletes database rows for the entity instances which match the LINQ query generated based on the provided specification from the database.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         This operation executes immediately against the database, rather than being deferred until
    ///         <see cref="DbContext.SaveChanges()" /> is called. It also does not interact with the EF change tracker in any way:
    ///         entity instances which happen to be tracked when this operation is invoked aren't taken into account, and aren't updated
    ///         to reflect the changes.
    ///     </para>
    ///     <para>
    ///         See <see href="https://aka.ms/efcore-docs-bulk-operations">Executing bulk operations with EF Core</see>
    ///         for more information and examples.
    ///     </para>
    /// </remarks>
    /// <param name="specification">Specification for the query.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns>The total number of rows deleted in the database.</returns>
    Task<Result<int>> ExecuteDeleteAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);
#endif    
}

/// <summary>
/// CRUD data service.
/// </summary>
[PublicAPI]
public interface ICrudDataService<TEntity, out TContext> : ICrudDataService<TEntity, long, TContext>, IReadOnlyDataService<TEntity, TContext>
    where TEntity : Entity<long> where TContext : class, IEfDbContext
{
}
