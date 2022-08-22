namespace DataExplorer.Abstractions.Entities;

/// <summary>
/// Defines a snowflake entity.
/// </summary>
[PublicAPI]
public interface ISnowflakeEntity<out TId> : IEntity<TId> where TId : IComparable, IEquatable<TId>, IComparable<TId>
{
    TId GenerateSnowflakeId();
}
