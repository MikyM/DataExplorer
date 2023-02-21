using DataExplorer.EfCore.Specifications;
using Remora.Results;

namespace DataExplorer.EfCore.Abstractions.DataServices;

/// <summary>
/// CRUD data service.
/// </summary>
[PublicAPI]
public interface ICrudDataService<TEntity, TId, out TContext> : IReadOnlyDataService<TEntity, TId, TContext>
    where TEntity : class, IEntity<TId>
    where TContext : class, IEfDbContext
    where TId : IComparable, IEquatable<TId>, IComparable<TId>
{
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
    
    /// <summary>
    /// Adds an entry.
    /// </summary>
    /// <typeparam name="TPost">Type of the entry</typeparam>
    /// <param name="entry">Entry to add</param>
    /// <param name="shouldSave">Whether to automatically call SaveChangesAsync() </param>
    /// <param name="userId">Optional Id of the user that is responsible for the changes</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><see cref="Result"/> with the Id of the newly created entity</returns>
    Task<Result<TId?>> AddAsync<TPost>(TPost entry, bool shouldSave, string? userId, CancellationToken cancellationToken = default)
        where TPost : class;
    
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
    /// <param name="userId">Optional Id of the user that is responsible for the changes</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><see cref="Result"/> with <see cref="IEnumerable{T}"/> containing Ids of the newly created entities</returns>
    Task<Result<IReadOnlyList<TId>>> AddAsync<TPost>(IEnumerable<TPost> entries, bool shouldSave,
        string? userId, CancellationToken cancellationToken = default) where TPost : class;
    
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
    /// <param name="userId">Optional Id of the user that is responsible for the changes</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><see cref="Result"/> with <see cref="IEnumerable{T}"/> containing Ids of the newly created entities</returns>
    Task<Result<IReadOnlyList<TId>>> AddRangeAsync<TPost>(IEnumerable<TPost> entries, bool shouldSave,
        string? userId, CancellationToken cancellationToken = default) where TPost : class;
    
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
    /// <param name="userId">Optional Id of the user that is responsible for the changes</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><see cref="Result"/> of the operation</returns>
    Task<Result> DeleteAsync<TDelete>(TDelete entry, bool shouldSave, string? userId, CancellationToken cancellationToken = default)
        where TDelete : class;
    
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

    /// <summary>
    /// Deletes an entity.
    /// </summary>
    /// <param name="id">Id of the entity to delete</param>
    /// <param name="shouldSave">Whether to automatically call SaveChangesAsync() </param>
    /// <param name="userId">Optional Id of the user that is responsible for the changes</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><see cref="Result"/> of the operation</returns>
    Task<Result> DeleteAsync(TId id, bool shouldSave, string? userId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Deletes an entity.
    /// </summary>
    /// <param name="id">Id of the entity to delete</param>
    /// <param name="shouldSave">Whether to automatically call SaveChangesAsync() </param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><see cref="Result"/> of the operation</returns>
    Task<Result> DeleteAsync(TId id, bool shouldSave = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a range of entities.
    /// </summary>
    /// <param name="entries">Entries to delete</param>
    /// <param name="shouldSave">Whether to automatically call SaveChangesAsync() </param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><see cref="Result"/> of the operation</returns>
    Task<Result> DeleteAsync<TDelete>(IEnumerable<TDelete> entries, bool shouldSave, CancellationToken cancellationToken = default)
        where TDelete : class;
    
    /// <summary>
    /// Deletes a range of entities.
    /// </summary>
    /// <param name="ids">Ids of the entities to delete</param>
    /// <param name="shouldSave">Whether to automatically call SaveChangesAsync() </param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><see cref="Result"/> of the operation</returns>
    Task<Result> DeleteAsync(IEnumerable<TId> ids, bool shouldSave, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Deletes a range of entities.
    /// </summary>
    /// <param name="entries">Entries to delete</param>
    /// <param name="shouldSave">Whether to automatically call SaveChangesAsync() </param>
    /// <param name="userId">Optional Id of the user that is responsible for the changes</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><see cref="Result"/> of the operation</returns>
    Task<Result> DeleteAsync<TDelete>(IEnumerable<TDelete> entries, bool shouldSave, string? userId, CancellationToken cancellationToken = default)
        where TDelete : class;
    
    /// <summary>
    /// Deletes a range of entities.
    /// </summary>
    /// <param name="ids">Ids of the entities to delete</param>
    /// <param name="shouldSave">Whether to automatically call SaveChangesAsync() </param>
    /// <param name="userId">Optional Id of the user that is responsible for the changes</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><see cref="Result"/> of the operation</returns>
    Task<Result> DeleteAsync(IEnumerable<TId> ids, bool shouldSave, string? userId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Deletes a range of entities.
    /// </summary>
    /// <param name="entries">Entries to delete</param>
    /// <param name="shouldSave">Whether to automatically call SaveChangesAsync() </param>
    /// <param name="userId">Optional Id of the user that is responsible for the changes</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><see cref="Result"/> of the operation</returns>
    Task<Result> DeleteRangeAsync<TDelete>(IEnumerable<TDelete> entries, bool shouldSave, string? userId, CancellationToken cancellationToken = default)
        where TDelete : class;
    
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
    /// Deletes a range of entities.
    /// </summary>
    /// <param name="ids">Ids of the entities to delete</param>
    /// <param name="shouldSave">Whether to automatically call SaveChangesAsync() </param>
    /// <param name="userId">Optional Id of the user that is responsible for the changes</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><see cref="Result"/> of the operation</returns>
    Task<Result> DeleteRangeAsync(IEnumerable<TId> ids, bool shouldSave, string? userId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Deletes a range of entities.
    /// </summary>
    /// <param name="ids">Ids of the entities to delete</param>
    /// <param name="shouldSave">Whether to automatically call SaveChangesAsync() </param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><see cref="Result"/> of the operation</returns>
    Task<Result> DeleteRangeAsync(IEnumerable<TId> ids, bool shouldSave = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Disables an entity.
    /// </summary>
    /// <typeparam name="TDisable">Type of the entry</typeparam>
    /// <param name="entry">Entry to disable</param>
    /// <param name="shouldSave">Whether to automatically call SaveChangesAsync() </param>
    /// <param name="userId">Optional Id of the user that is responsible for the changes</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><see cref="Result"/> of the operation</returns>
    Task<Result> DisableAsync<TDisable>(TDisable entry, bool shouldSave, string? userId, CancellationToken cancellationToken = default)
        where TDisable : class;
    
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
    /// Disables an entity.
    /// </summary>
    /// <param name="id">Id of the entity to disable</param>
    /// <param name="shouldSave">Whether to automatically call SaveChangesAsync() </param>
    /// <param name="userId">Optional Id of the user that is responsible for the changes</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><see cref="Result"/> of the operation</returns>
    Task<Result> DisableAsync(TId id, bool shouldSave, string? userId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Disables an entity.
    /// </summary>
    /// <param name="id">Id of the entity to disable</param>
    /// <param name="shouldSave">Whether to automatically call SaveChangesAsync() </param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><see cref="Result"/> of the operation</returns>
    Task<Result> DisableAsync(TId id, bool shouldSave = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Disables a range of entities.
    /// </summary>
    /// <typeparam name="TDisable">Type of the entry</typeparam>
    /// <param name="entries">Entries to disable</param>
    /// <param name="shouldSave">Whether to automatically call SaveChangesAsync() </param>
    /// <param name="userId">Optional Id of the user that is responsible for the changes</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><see cref="Result"/> of the operation</returns>
    Task<Result> DisableAsync<TDisable>(IEnumerable<TDisable> entries, bool shouldSave,
        string? userId, CancellationToken cancellationToken = default) where TDisable : class;
    
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
    /// <param name="ids">Ids of the entities to disable</param>
    /// <param name="shouldSave">Whether to automatically call SaveChangesAsync() </param>
    /// <param name="userId">Optional Id of the user that is responsible for the changes</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><see cref="Result"/> of the operation</returns>
    Task<Result> DisableAsync(IEnumerable<TId> ids, bool shouldSave, string? userId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Disables a range of entities.
    /// </summary>
    /// <param name="ids">Ids of the entities to disable</param>
    /// <param name="shouldSave">Whether to automatically call SaveChangesAsync() </param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><see cref="Result"/> of the operation</returns>
    Task<Result> DisableAsync(IEnumerable<TId> ids, bool shouldSave = false, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Disables a range of entities.
    /// </summary>
    /// <typeparam name="TDisable">Type of the entry</typeparam>
    /// <param name="entries">Entries to disable</param>
    /// <param name="shouldSave">Whether to automatically call SaveChangesAsync() </param>
    /// <param name="userId">Optional Id of the user that is responsible for the changes</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><see cref="Result"/> of the operation</returns>
    Task<Result> DisableRangeAsync<TDisable>(IEnumerable<TDisable> entries, bool shouldSave,
        string? userId, CancellationToken cancellationToken = default) where TDisable : class;
    
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
    /// Disables a range of entities.
    /// </summary>
    /// <param name="ids">Ids of the entities to disable</param>
    /// <param name="shouldSave">Whether to automatically call SaveChangesAsync() </param>
    /// <param name="userId">Optional Id of the user that is responsible for the changes</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><see cref="Result"/> of the operation</returns>
    Task<Result> DisableRangeAsync(IEnumerable<TId> ids, bool shouldSave, string? userId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Disables a range of entities.
    /// </summary>
    /// <param name="ids">Ids of the entities to disable</param>
    /// <param name="shouldSave">Whether to automatically call SaveChangesAsync() </param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><see cref="Result"/> of the operation</returns>
    Task<Result> DisableRangeAsync(IEnumerable<TId> ids, bool shouldSave = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Detaches an entry and all other reachable entries.
    /// </summary>
    /// <typeparam name="TDetach">Type of the entry</typeparam>
    /// <param name="entry">Entry to detach</param>
    /// <returns><see cref="Result"/> of the operation</returns>
    Result Detach<TDetach>(TDetach entry) where TDetach : class;
}

/// <summary>
/// CRUD data service.
/// </summary>
[PublicAPI]
public interface ICrudDataService<TEntity, out TContext> : ICrudDataService<TEntity, long, TContext>, IReadOnlyDataService<TEntity, TContext>
    where TEntity : class, IEntity<long> where TContext : class, IEfDbContext
{
}
