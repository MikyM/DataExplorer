using DataExplorer.Exceptions;
using MongoDB.Entities;

#pragma warning disable CS1574, CS1584, CS1581, CS1580

namespace DataExplorer.MongoDb.Abstractions.Repositories;

/// <summary>
/// Repository.
/// </summary>
/// <typeparam name="TEntity">Entity that derives from <see cref="Entities.Entity"/>.</typeparam>
/// <typeparam name="TId">Type of the Id in <typeparamref name="TEntity"/>.</typeparam>
[PublicAPI]
public interface IMongoRepository<TEntity,TId> : IMongoReadOnlyRepository<TEntity,TId> where TEntity : Entity<TId>, IEntity where TId : IComparable, IEquatable<TId>, IComparable<TId>
{
    /// <summary>
    ///     <para>
    ///         Disables an entity.
    ///     </para>
    ///     <para>
    ///         Tries to fetch the entity by the provided Id, then begins tracking the given entity via <see cref="BeginUpdate"/> and sets it's <see cref="IDisableableEntity.IsDisabled"/> property to <b>true</b>.
    ///     </para>
    /// </summary>
    /// <param name="id">Id of the entity to disable.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <exception cref="NotFoundException">Thrown when entity with given Id is not found.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the given entity does not implement <see cref="IDisableableEntity"/>.</exception>
    Task DisableAsync(TId id, CancellationToken cancellationToken = default);

    /// <summary>
    ///     <para>
    ///         Disables a range of entities.
    ///     </para>
    ///     <para>
    ///         Begins tracking the given entities via <see cref="BeginUpdate"/> and sets their <see cref="IDisableableEntity.IsDisabled"/> properties to <b>true</b>.
    ///     </para>
    /// </summary>
    /// <param name="entities">Entities to disable.</param>
    /// <exception cref="InvalidOperationException">Thrown when the given entities do not implement <see cref="IDisableableEntity"/>.</exception>
    void DisableRange(IEnumerable<TEntity> entities);
}

/// <summary>
/// Repository.
/// </summary>
/// <typeparam name="TEntity">Entity that derives from <see cref="Entities.Entity"/>.</typeparam>
[PublicAPI]
public interface IMongoRepository<TEntity> : IMongoRepository<TEntity,long>, IMongoReadOnlyRepository<TEntity> where TEntity : Entity<long>, MongoDB.Entities.IEntity
{
}
