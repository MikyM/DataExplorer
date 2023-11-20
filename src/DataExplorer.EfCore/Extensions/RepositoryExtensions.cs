using DataExplorer.EfCore.Specifications;

namespace DataExplorer.EfCore.Extensions;

/// <summary>
/// Repository extensions.
/// </summary>
[PublicAPI]
public static class RepositoryExtensions
{
#if NET7_0_OR_GREATER
    /// <summary>
    ///     <para>
    ///         Disables an entity by executing an explicit update on the database.
    ///     </para>
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
    /// <param name="repository">The repository.</param>
    /// <param name="id">Id of the entity to disable.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <remarks>True if any row was affected, otherwise false.</remarks>
    public static async Task<bool> DisableAsync<TEntity,TId>(this IRepository<TEntity,TId> repository, TId id, CancellationToken cancellationToken = default) 
        where TEntity : Entity<TId>, IDisableableEntity where TId : IComparable, IEquatable<TId>, IComparable<TId>
    {
        var res = await repository.ExecuteUpdateAsync(new DisableSpecification<TEntity,TId>(id), cancellationToken);
        return res == 1;
    }
    
    /// <summary>
    ///     <para>
    ///         Disables a range of entities by executing an explicit update on the database.
    ///     </para>
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
    /// <param name="repository">The repository.</param>
    /// <param name="ids">Ids of the entities to disable.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The amount of affected rows.</returns>
    public static async Task<long> DisableRangeAsync<TEntity,TId>(this IRepository<TEntity,TId> repository, IEnumerable<TId> ids, CancellationToken cancellationToken = default) 
        where TEntity : Entity<TId>, IDisableableEntity where TId : IComparable, IEquatable<TId>, IComparable<TId>
    {
        var res = await repository.ExecuteUpdateAsync(new DisableSpecification<TEntity,TId>(ids), cancellationToken);
        return res;
    }
#endif
}
