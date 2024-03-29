﻿namespace DataExplorer.Abstractions.Entities;

/// <summary>
/// Represents a base entity with <see cref="long"/> Id.
/// </summary>
[PublicAPI]
public interface IEntity : IEntity<long>
{
}

/// <summary>
/// Represents a generic base entity.
/// </summary>
[PublicAPI]
public interface IEntity<TId> : IEntityBase where TId : IComparable, IEquatable<TId>, IComparable<TId>
{
    /// <summary>
    /// Sets the Id of this entity.
    /// </summary>
    /// <param name="id"></param>
    public void SetId(TId id);

    /// <summary>
    /// The Id of the entity.
    /// </summary>
    public TId Id { get; }
}

/// <summary>
/// Represents a base marker for entities. <b> Shouldn't be implemented manually.</b>
/// </summary>
[PublicAPI]
public interface IEntityBase
{
    /// <summary>
    /// Sets the ID of this entity.
    /// </summary>
    public void SetId(object id);

    /// <summary>
    /// Whether the entity has a valid Id.
    /// </summary>
    public bool HasValidId { get; }
}
