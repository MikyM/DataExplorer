using DataExplorer.Abstractions.Specifications;
using DataExplorer.EfCore.Abstractions.Specifications;
using Microsoft.EntityFrameworkCore.ChangeTracking;
#pragma warning disable CS1574, CS1584, CS1581, CS1580

namespace DataExplorer.EfCore.Abstractions.Repositories;

/// <summary>
/// Repository.
/// </summary>
/// <typeparam name="TEntity">Entity that derives from <see cref="Entity"/>.</typeparam>
/// <typeparam name="TId">Type of the Id in <typeparamref name="TEntity"/>.</typeparam>
[PublicAPI]
public interface IRepository<TEntity,TId> : IReadOnlyRepository<TEntity,TId>, IRepositoryBase<TEntity,TId> where TEntity : Entity<TId> where TId : IComparable, IEquatable<TId>, IComparable<TId>
{
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
    Task<int> ExecuteUpdateAsync(IUpdateSpecification<TEntity> specification,
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
    Task<int> ExecuteDeleteAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    ///     Asynchronously deletes all database rows that satisfy given predicate.
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
    /// <param name="predicate">Predicate for the query.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns>The total number of rows deleted in the database.</returns>
    Task<int> ExecuteDeleteAsync(Func<TEntity,bool> predicate, CancellationToken cancellationToken = default);
    
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
    Task<int> ExecuteDeleteAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);
#endif
   

    /// <summary>
    ///     <para>
    ///         Begins tracking the given entity and entries reachable from the given entity using
    ///         the <see cref="EntityState.Unchanged" /> state by default, but see below for cases
    ///         when a different state will be used.
    ///     </para>
    ///     <para>
    ///         Generally, no database interaction will be performed until <see cref="SaveChangesAsync()" /> or <see cref="SaveChanges()"/> is called.
    ///     </para>
    ///     <para>
    ///         A recursive search of the navigation properties will be performed to find reachable entities
    ///         that are not already being tracked by the context. All entities found will be tracked
    ///         by the context.
    ///     </para>
    ///     <para>
    ///         For entity types with generated keys if an entity has its primary key value set
    ///         then it will be tracked in the <see cref="EntityState.Unchanged" /> state. If the primary key
    ///         value is not set then it will be tracked in the <see cref="EntityState.Added" /> state.
    ///         This helps ensure only new entities will be inserted.
    ///         An entity is considered to have its primary key value set if the primary key property is set
    ///         to anything other than the CLR default for the property type.
    ///     </para>
    ///     <para>
    ///         For entity types without generated keys, the state set is always <see cref="EntityState.Unchanged" />.
    ///     </para>
    ///     <para>
    ///         Use <see cref="EntityEntry.State" /> to set the state of only a single entity.
    ///     </para>
    /// </summary>
    /// <remarks>
    ///     See <see href="https://aka.ms/efcore-docs-change-tracking">EF Core change tracking</see> for more information.
    /// </remarks>
    /// <param name="entity">The entity to begin updating.</param>
    /// <param name="shouldSwapAttached">Whether to swapped an already attached entity if found with one that was provided.</param>
    /// <returns>
    ///     The <see cref="EntityEntry{TEntity}" /> for the entity. The entry provides
    ///     access to change tracking information and operations for the entity.
    /// </returns>
    EntityEntry<TEntity> BeginUpdateWithEntityEntry(TEntity entity, bool shouldSwapAttached = false);

    /// <summary>
    ///     <para>
    ///         Begins tracking given entities and entries reachable from the given entity using
    ///         the <see cref="EntityState.Unchanged" /> state by default, but see below for cases
    ///         when a different state will be used.
    ///     </para>
    ///     <para>
    ///         Generally, no database interaction will be performed until <see cref="SaveChangesAsync()" /> or <see cref="SaveChanges()"/> is called.
    ///     </para>
    ///     <para>
    ///         A recursive search of the navigation properties will be performed to find reachable entities
    ///         that are not already being tracked by the context. All entities found will be tracked
    ///         by the context.
    ///     </para>
    ///     <para>
    ///         For entity types with generated keys if an entity has its primary key value set
    ///         then it will be tracked in the <see cref="EntityState.Unchanged" /> state. If the primary key
    ///         value is not set then it will be tracked in the <see cref="EntityState.Added" /> state.
    ///         This helps ensure only new entities will be inserted.
    ///         An entity is considered to have its primary key value set if the primary key property is set
    ///         to anything other than the CLR default for the property type.
    ///     </para>
    ///     <para>
    ///         For entity types without generated keys, the state set is always <see cref="EntityState.Unchanged" />.
    ///     </para>
    ///     <para>
    ///         Use <see cref="EntityEntry.State" /> to set the state of only a single entity.
    ///     </para>
    /// </summary>
    /// <remarks>
    ///     See <see href="https://aka.ms/efcore-docs-change-tracking">EF Core change tracking</see> for more information.
    /// </remarks>
    /// <param name="entities">The entity to begin updating.</param>
    /// <param name="shouldSwapAttached">Whether to swapped an already attached entity if found with one that was provided.</param>
    /// <returns>
    ///     The <see cref="EntityEntry{TEntity}" /> for the entities. The entries provide
    ///     access to change tracking information and operations for the entities.
    /// </returns>
    IReadOnlyList<EntityEntry<TEntity>> BeginUpdateRangeWithEntityEntries(IEnumerable<TEntity> entities, bool shouldSwapAttached = false);
}

/// <summary>
/// Repository.
/// </summary>
/// <typeparam name="TEntity">Entity that derives from <see cref="Entity"/>.</typeparam>
[PublicAPI]
public interface IRepository<TEntity> : IRepository<TEntity,long>, IReadOnlyRepository<TEntity> where TEntity : Entity<long>
{
}
