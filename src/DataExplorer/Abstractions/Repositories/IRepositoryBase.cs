using DataExplorer.Abstractions.Entities;
using DataExplorer.Entities;

#pragma warning disable CS1574, CS1584, CS1581, CS1580

namespace DataExplorer.Abstractions.Repositories;

/// <summary>
/// Repository.
/// </summary>
/// <typeparam name="TEntity">Entity that derives from <see cref="Entity"/>.</typeparam>
/// <typeparam name="TId">Type of the Id in <typeparamref name="TEntity"/>.</typeparam>
[PublicAPI]
public interface IRepositoryBase<TEntity,TId> : IReadOnlyRepositoryBase<TEntity,TId> where TEntity : Entity<TId> where TId : IComparable, IEquatable<TId>, IComparable<TId>
{
    /// <summary>
    ///     <para>
    ///         Begins tracking the given entity, and any other reachable entities that are
    ///         not already being tracked, in the <see cref="EntityState.Added" /> state such that they will
    ///         be inserted into the database when <see cref="DbContext.SaveChanges()" /> is called.
    ///     </para>
    ///     <para>
    ///         Use <see cref="EntityEntry.State" /> to set the state of only a single entity.
    ///     </para>
    /// </summary>
    /// <remarks>
    ///     See <see href="https://aka.ms/efcore-docs-change-tracking">EF Core change tracking</see> for more information.
    /// </remarks>
    /// <param name="entity">The entity to add.</param>
    /// <returns>
    ///     The <see cref="EntityEntry{TEntity}" /> for the entity. The entry provides
    ///     access to change tracking information and operations for the entity.
    /// </returns>
    void Add(TEntity entity);

    /// <summary>
    ///     Begins tracking the given entities, and any other reachable entities that are
    ///     not already being tracked, in the <see cref="EntityState.Added" /> state such that they will
    ///     be inserted into the database when <see cref="DbContext.SaveChanges()" /> is called.
    /// </summary>
    /// <remarks>
    ///     See <see href="https://aka.ms/efcore-docs-change-tracking">EF Core change tracking</see>
    ///     and <see href="https://aka.ms/efcore-docs-attach-range">Using AddRange, UpdateRange, AttachRange, and RemoveRange</see>
    ///     for more information.
    /// </remarks>
    /// <param name="entities">The entities to add.</param>
    void AddRange(IEnumerable<TEntity> entities);
    
    /// <summary>
    ///     Begins tracking the given entities, and any other reachable entities that are
    ///     not already being tracked, in the <see cref="EntityState.Added" /> state such that they will
    ///     be inserted into the database when <see cref="DbContext.SaveChanges()" /> is called.
    /// </summary>
    /// <remarks>
    ///     See <see href="https://aka.ms/efcore-docs-change-tracking">EF Core change tracking</see>
    ///     and <see href="https://aka.ms/efcore-docs-attach-range">Using AddRange, UpdateRange, AttachRange, and RemoveRange</see>
    ///     for more information.
    /// </remarks>
    /// <param name="entities">The entities to add.</param>
    void AddRange(params TEntity[] entities);
    
    /// <summary>
    ///     <para>
    ///         Begins tracking the given entity, and any other reachable entities that are
    ///         not already being tracked, in the <see cref="EntityState.Added" /> state such that they will
    ///         be inserted into the database when <see cref="DbContext.SaveChanges()" /> is called.
    ///     </para>
    ///     <para>
    ///         Use <see cref="EntityEntry.State" /> to set the state of only a single entity.
    ///     </para>
    /// </summary>
    /// <remarks>
    ///     See <see href="https://aka.ms/efcore-docs-change-tracking">EF Core change tracking</see> for more information.
    /// </remarks>
    /// <param name="entity">The entity to add.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns>
    ///     A task that represents the asynchronous Add operation. The task result contains the
    ///     <see cref="EntityEntry{TEntity}" /> for the entity. The entry provides access to change tracking
    ///     information and operations for the entity.
    /// </returns>
    /// <exception cref="OperationCanceledException">If the <see cref="CancellationToken" /> is canceled.</exception>
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    ///     <para>
    ///         Begins tracking the given entities, and any other reachable entities that are
    ///         not already being tracked, in the <see cref="EntityState.Added" /> state such that they will
    ///         be inserted into the database when <see cref="DbContext.SaveChanges()" /> is called.
    ///     </para>
    /// </summary>
    /// <remarks>
    ///     See <see href="https://aka.ms/efcore-docs-change-tracking">EF Core change tracking</see>
    ///     and <see href="https://aka.ms/efcore-docs-attach-range">Using AddRange, UpdateRange, AttachRange, and RemoveRange</see>
    ///     for more information.
    /// </remarks>
    /// <param name="entities">The entities to add.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="OperationCanceledException">If the <see cref="CancellationToken" /> is canceled.</exception>
    Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
    
    /// <summary>
    ///     <para>
    ///         Begins tracking the given entities, and any other reachable entities that are
    ///         not already being tracked, in the <see cref="EntityState.Added" /> state such that they will
    ///         be inserted into the database when <see cref="DbContext.SaveChanges()" /> is called.
    ///     </para>
    /// </summary>
    /// <remarks>
    ///     See <see href="https://aka.ms/efcore-docs-change-tracking">EF Core change tracking</see>
    ///     and <see href="https://aka.ms/efcore-docs-attach-range">Using AddRange, UpdateRange, AttachRange, and RemoveRange</see>
    ///     for more information.
    /// </remarks>
    /// <param name="entities">The entities to add.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task AddRangeAsync(params TEntity[] entities);

    /// <summary>
    ///     <para>
    ///         Begins tracking the given entity and entries reachable from the given entity.
    ///     </para>
    /// </summary>
    /// <param name="entity">The entity to begin updating.</param>
    /// <param name="shouldSwapAttached">Whether to swapped an already attached entity if found with one that was provided.</param>
    void BeginUpdate(TEntity entity, bool shouldSwapAttached = false);

    /// <summary>
    ///     <para>
    ///         Begins tracking given entities and entries reachable from the given entities.
    ///     </para>
    /// </summary>
    /// <param name="entities">The entity to begin updating.</param>
    /// <param name="shouldSwapAttached">Whether to swapped an already attached entity if found with one that was provided.</param>
    void BeginUpdateRange(IEnumerable<TEntity> entities, bool shouldSwapAttached = false);

    /// <summary>
    ///     Begins tracking the given entity in the <see cref="EntityState.Deleted" /> state such that it will
    ///     be removed from the database when <see cref="DbContext.SaveChanges()" /> is called.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         If the entity is already tracked in the <see cref="EntityState.Added" /> state then the context will
    ///         stop tracking the entity (rather than marking it as <see cref="EntityState.Deleted" />) since the
    ///         entity was previously added to the context and does not exist in the database.
    ///     </para>
    ///     <para>
    ///         Any other reachable entities that are not already being tracked will be tracked in the same way that
    ///         they would be if <see cref="Attach(TEntity)" /> was called before calling this method.
    ///         This allows any cascading actions to be applied when <see cref="DbContext.SaveChanges()" /> is called.
    ///     </para>
    ///     <para>
    ///         Use <see cref="EntityEntry.State" /> to set the state of only a single entity.
    ///     </para>
    ///     <para>
    ///         See <see href="https://aka.ms/efcore-docs-change-tracking">EF Core change tracking</see> for more information.
    ///     </para>
    /// </remarks>
    /// <param name="entity">The entity to remove.</param>
    void Delete(TEntity entity);

#if NET7_0_OR_GREATER
    /// <summary>
    ///     Asynchronously deletes an entity with given Id.
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
    /// <param name="id">Id of the entity to delete.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns>True if an entity was found and deleted, otherwise false.</returns>
    Task<bool> DeleteAsync(TId id, CancellationToken cancellationToken = default);
    
    /// <summary>
    ///     Asynchronously deletes entities with given Ids.
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
    /// <param name="ids">Ids of the entities to delete.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns>The number of deleted entities.</returns>
    Task<long> DeleteRangeAsync(IEnumerable<TId> ids, CancellationToken cancellationToken = default);
#endif
    
    /// <summary>
    ///     Begins tracking the given entities in the <see cref="EntityState.Deleted" /> state such that they will
    ///     be removed from the database when <see cref="DbContext.SaveChanges()" /> is called.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         If any of the entities are already tracked in the <see cref="EntityState.Added" /> state then the context will
    ///         stop tracking those entities (rather than marking them as <see cref="EntityState.Deleted" />) since those
    ///         entities were previously added to the context and do not exist in the database.
    ///     </para>
    ///     <para>
    ///         Any other reachable entities that are not already being tracked will be tracked in the same way that
    ///         they would be if <see cref="AttachRange(IEnumerable{TEntity})" /> was called before calling this method.
    ///         This allows any cascading actions to be applied when <see cref="DbContext.SaveChanges()" /> is called.
    ///     </para>
    ///     <para>
    ///         See <see href="https://aka.ms/efcore-docs-change-tracking">EF Core change tracking</see>
    ///         and <see href="https://aka.ms/efcore-docs-attach-range">Using AddRange, UpdateRange, AttachRange, and RemoveRange</see>
    ///         for more information.
    ///     </para>
    /// </remarks>
    /// <param name="entities">The entities to remove.</param>
    void DeleteRange(IEnumerable<TEntity> entities);
    
    /// <summary>
    ///     <para>
    ///         Disables an entity.
    ///     </para>
    ///     <para>
    ///         Begins tracking the given entity via <see cref="BeginUpdate"/> and sets it's <see cref="IDisableable.IsDisabled"/> property to <b>true</b>.
    ///     </para>
    /// </summary>
    /// <param name="entity">Entity to disable.</param>
    /// <exception cref="InvalidOperationException">Thrown when the given entity does not implement <see cref="IDisableable"/>.</exception>
    void Disable(TEntity entity);

    /// <summary>
    ///     <para>
    ///         Disables a range of entities.
    ///     </para>
    ///     <para>
    ///         Begins tracking the given entities via <see cref="BeginUpdate"/> and sets their <see cref="IDisableable.IsDisabled"/> properties to <b>true</b>.
    ///     </para>
    /// </summary>
    /// <param name="entities">Entities to disable.</param>
    /// <exception cref="InvalidOperationException">Thrown when the given entities do not implement <see cref="IDisableable"/>.</exception>
    void DisableRange(IEnumerable<TEntity> entities);

    /// <summary>
    ///     <para>
    ///         Detaches the given entity from the current context, such that changes applied to it will not be reflected in the database when <see cref="DbContext.SaveChanges()" /> is called.
    ///     </para>
    /// </summary>
    /// <remarks>If using a recursive call - the recursive path will break when it stumbles upon an already detached entry resulting in a not-fully detached entry tree.</remarks>
    /// <param name="entity">Entity to detach.</param>
    /// <param name="recursive">Whether to recursively detach ALL navigation properties.</param>
    void Detach(TEntity entity, bool recursive = false);
}

/// <summary>
/// Repository.
/// </summary>
/// <typeparam name="TEntity">Entity that derives from <see cref="Entity"/>.</typeparam>
[PublicAPI]
public interface IRepositoryBase<TEntity> : IRepositoryBase<TEntity,long>, IReadOnlyRepositoryBase<TEntity> where TEntity : Entity<long>
{
}
