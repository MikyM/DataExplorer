using DataExplorer.EfCore.Specifications;
using Remora.Results;

namespace DataExplorer.EfCore.Abstractions.DataServices;

/// <summary>
/// CRUD data service.
/// </summary>
[PublicAPI]
public interface ICrudDataService<TEntity, TId, out TContext> : IReadOnlyDataService<TEntity, TId, TContext>
    where TEntity : Entity<TId>
    where TContext : class, IEfDbContext
    where TId : IComparable, IEquatable<TId>, IComparable<TId>
{
    /// <summary>
    /// The underlying repository.
    /// </summary>
    IRepository<TEntity,TId> Repository { get; }
    
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

    /// <summary>
    /// Adds an entry.
    /// </summary>
    /// <typeparam name="TPost">Type of the entry</typeparam>
    /// <param name="entry">Entry to add</param>
    /// <param name="shouldSave">Whether to automatically call SaveChangesAsync() </param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><see cref="Result"/> with the Id of the newly created entity</returns>
    Task<Result<TId?>> AddAsync<TPost>(TPost entry, bool shouldSave = false, CancellationToken cancellationToken = default)
        where TPost : class;
    
    /// <summary>
    /// Adds a range of entries.
    /// </summary>
    /// <typeparam name="TPost">Type of the entries</typeparam>
    /// <param name="entries">Entries to add</param>
    /// <param name="shouldSave">Whether to automatically call SaveChangesAsync() </param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><see cref="Result"/> with <see cref="IEnumerable{T}"/> containing Ids of the newly created entities</returns>
    Task<Result<IReadOnlyList<TId>>> AddAsync<TPost>(IEnumerable<TPost> entries, bool shouldSave = false,
        CancellationToken cancellationToken = default) where TPost : class;
    
    /// <summary>
    /// Adds a range of entries.
    /// </summary>
    /// <typeparam name="TPost">Type of the entries</typeparam>
    /// <param name="entries">Entries to add</param>
    /// <param name="shouldSave">Whether to automatically call SaveChangesAsync() </param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><see cref="Result"/> with <see cref="IEnumerable{T}"/> containing Ids of the newly created entities</returns>
    Task<Result<IReadOnlyList<TId>>> AddRangeAsync<TPost>(IEnumerable<TPost> entries, bool shouldSave = false,
        CancellationToken cancellationToken = default) where TPost : class;

    /// <summary>
    /// Begins updating an entity.
    /// </summary>
    /// <typeparam name="TPatch">Type of the entry</typeparam>
    /// <param name="entry">Entry to attach</param>
    /// <param name="shouldSwapAttached">Whether to swap existing entity with same primary keys attached to current <see cref="DbContext"/> with new one </param>
    /// <returns><see cref="Result"/> of the operation</returns>
    Result BeginUpdate<TPatch>(TPatch entry, bool shouldSwapAttached = false) where TPatch : class;

    /// <summary>
    /// Begins updating a range of entries.
    /// </summary>
    /// <typeparam name="TPatch">Type of the entries</typeparam>
    /// <param name="entries">Entries to attach</param>
    /// <param name="shouldSwapAttached">Whether to swap existing entities with same primary keys attached to current <see cref="DbContext"/> with new ones </param>
    /// <returns><see cref="Result"/> of the operation</returns>
    Result BeginUpdate<TPatch>(IEnumerable<TPatch> entries, bool shouldSwapAttached = false) where TPatch : class;
    
    /// <summary>
    /// Begins updating a range of entries.
    /// </summary>
    /// <typeparam name="TPatch">Type of the entries</typeparam>
    /// <param name="entries">Entries to attach</param>
    /// <param name="shouldSwapAttached">Whether to swap existing entities with same primary keys attached to current <see cref="DbContext"/> with new ones </param>
    /// <returns><see cref="Result"/> of the operation</returns>
    Result BeginUpdateRange<TPatch>(IEnumerable<TPatch> entries, bool shouldSwapAttached = false) where TPatch : class;
    
    /// <summary>
    /// Deletes an entity.
    /// </summary>
    /// <typeparam name="TDelete">Type of the entry</typeparam>
    /// <param name="entry">Entry to delete</param>
    /// <param name="shouldSave">Whether to automatically call SaveChangesAsync() </param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><see cref="Result"/> of the operation</returns>
    Task<Result> DeleteAsync<TDelete>(TDelete entry, bool shouldSave = false, CancellationToken cancellationToken = default)
        where TDelete : class;

#if NET7_0_OR_GREATER 
    /// <summary>
    /// Deletes an entity.
    /// </summary>
    /// <param name="id">Id of the entity to delete</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><see cref="Result"/> of the operation containing information whether any entity has been deleted.</returns>
    Task<Result<bool>> DeleteAsync(TId id, CancellationToken cancellationToken = default);
#endif


    /// <summary>
    /// Deletes a range of entities.
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
    /// <param name="entries">Entries to delete</param>
    /// <param name="shouldSave">Whether to automatically call SaveChangesAsync() </param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><see cref="Result"/> of the operation</returns>
    Task<Result> DeleteAsync<TDelete>(IEnumerable<TDelete> entries, bool shouldSave, CancellationToken cancellationToken = default)
        where TDelete : class;

#if NET7_0_OR_GREATER
    /// <summary>
    /// Deletes a range of entities.
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
    /// <param name="ids">Ids of the entities to delete</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><see cref="Result"/> of the operation containing the amount of affected entities.</returns>
    Task<Result<long>> DeleteAsync(IEnumerable<TId> ids, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Deletes a range of entities.
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
    /// <param name="ids">Ids of the entities to delete</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><see cref="Result"/> of the operation</returns>
    Task<Result<long>> DeleteRangeAsync(IEnumerable<TId> ids, CancellationToken cancellationToken = default);
#endif
    
    /// <summary>
    /// Deletes a range of entities.
    /// </summary>
    /// <param name="entries">Entries to delete</param>
    /// <param name="shouldSave">Whether to automatically call SaveChangesAsync() </param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><see cref="Result"/> of the operation</returns>
    Task<Result> DeleteRangeAsync<TDelete>(IEnumerable<TDelete> entries, bool shouldSave = false, CancellationToken cancellationToken = default)
        where TDelete : class;
    
    /// <summary>
    /// Disables an entity.
    /// </summary>
    /// <typeparam name="TDisable">Type of the entry</typeparam>
    /// <param name="entry">Entry to disable</param>
    /// <param name="shouldSave">Whether to automatically call SaveChangesAsync() </param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><see cref="Result"/> of the operation</returns>
    Task<Result> DisableAsync<TDisable>(TDisable entry, bool shouldSave = false, CancellationToken cancellationToken = default)
        where TDisable : class;
    
    /// <summary>
    /// Disables a range of entities.
    /// </summary>
    /// <typeparam name="TDisable">Type of the entry</typeparam>
    /// <param name="entries">Entries to disable</param>
    /// <param name="shouldSave">Whether to automatically call SaveChangesAsync() </param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><see cref="Result"/> of the operation</returns>
    Task<Result> DisableAsync<TDisable>(IEnumerable<TDisable> entries, bool shouldSave = false,
        CancellationToken cancellationToken = default) where TDisable : class;
    
    /// <summary>
    /// Disables a range of entities.
    /// </summary>
    /// <typeparam name="TDisable">Type of the entry</typeparam>
    /// <param name="entries">Entries to disable</param>
    /// <param name="shouldSave">Whether to automatically call SaveChangesAsync() </param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><see cref="Result"/> of the operation</returns>
    Task<Result> DisableRangeAsync<TDisable>(IEnumerable<TDisable> entries, bool shouldSave = false,
        CancellationToken cancellationToken = default) where TDisable : class;
    
    /// <summary>
    ///     <para>
    ///         Detaches the given entry from the current context, such that changes applied to it will not be reflected in the database when <see cref="DbContext.SaveChanges()"/> is called.
    ///     </para>
    /// </summary>
    /// <remarks>If using a recursive call - the recursive path will break when it stumbles upon an already detached entry resulting in a not-fully detached entry tree.</remarks>
    /// <param name="recursive">Whether to recursively detach ALL navigation properties.</param>
    /// <typeparam name="TDetach">Type of the entry</typeparam>
    /// <param name="entry">Entry to detach</param>
    /// <returns><see cref="Result"/> of the operation</returns>
    Result Detach<TDetach>(TDetach entry, bool recursive = false) where TDetach : class;
}

/// <summary>
/// CRUD data service.
/// </summary>
[PublicAPI]
public interface ICrudDataService<TEntity, out TContext> : ICrudDataService<TEntity, long, TContext>, IReadOnlyDataService<TEntity, TContext>
    where TEntity : Entity<long> where TContext : class, IEfDbContext
{
}
