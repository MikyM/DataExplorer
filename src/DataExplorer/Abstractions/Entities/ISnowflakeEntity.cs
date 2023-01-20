﻿namespace DataExplorer.Abstractions.Entities;

/// <summary>
/// Represents a snowflake entity.
/// </summary>
[PublicAPI]
public interface ISnowflakeEntity<TId> : ISnowflakeEntity, IEntity<TId> where TId : IComparable, IEquatable<TId>, IComparable<TId>
{
}

/// <summary>
/// Represents a base snowflake entity.
/// </summary>
[PublicAPI]
public interface ISnowflakeEntity : IEntityBase
{
    /// <summary>
    /// Whether to fill this entity's Id automatically.
    /// </summary>
    bool ShouldHaveIdFilled { get; }
}
