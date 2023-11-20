using DataExplorer.EfCore.Abstractions.DataServices;
using Remora.Results;

namespace DataExplorer.EfCore.Extensions;

/// <summary>
/// Repository extensions.
/// </summary>
[PublicAPI]
public static class CrudDataServiceExtensions
{
#if NET7_0_OR_GREATER
    /// <summary>
    /// Disables an entity.
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
    /// <param name="ids">Ids of the entities to disable</param>
    /// <param name="service">The service.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><see cref="Result"/> of the operation</returns>
    public static Task<Result<long>> DisableAsync<TEntity, TId, TContext>(
        this ICrudDataService<TEntity, TId, TContext> service, IEnumerable<TId> ids,
        CancellationToken cancellationToken = default)
        where TEntity : Entity<TId>, IDisableableEntity
        where TId : IComparable, IEquatable<TId>, IComparable<TId>
        where TContext : class, IEfDbContext 
        => DisableRangeAsync(service, ids, cancellationToken);

    /// <summary>
    /// Disables a range of entities.
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
    /// <param name="service">The service.</param>
    /// <param name="id">Id of the entity to disable.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><see cref="Result"/> of the operation</returns>
    public static async Task<Result<bool>> DisableAsync<TEntity,TId,TContext>(this ICrudDataService<TEntity,TId,TContext> service, TId id,
        CancellationToken cancellationToken = default) 
        where TEntity : Entity<TId>, IDisableableEntity
        where TId : IComparable, IEquatable<TId>, IComparable<TId> 
        where TContext : class, IEfDbContext
    {
        try
        {
            return await service.Repository.DisableAsync(id, cancellationToken);
        }
        catch (Exception ex)
        {
            return new ExceptionError(ex);
        }
    }

    /// <summary>
    /// Disables a range of entities.
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
    /// <param name="service">The service.</param>
    /// <param name="ids">Ids of the entities to disable</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><see cref="Result"/> of the operation</returns>
    public static async Task<Result<long>> DisableRangeAsync<TEntity,TId,TContext>(this ICrudDataService<TEntity,TId,TContext> service, IEnumerable<TId> ids,
        CancellationToken cancellationToken = default) 
        where TEntity : Entity<TId>, IDisableableEntity
        where TId : IComparable, IEquatable<TId>, IComparable<TId> 
        where TContext : class, IEfDbContext
    {
        try
        {
            return await service.Repository.DisableRangeAsync(ids, cancellationToken);
        }
        catch (Exception ex)
        {
            return new ExceptionError(ex);
        }
    }
#endif
}
