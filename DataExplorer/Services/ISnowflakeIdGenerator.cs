namespace DataExplorer.Services;

/// <summary>
/// Defines a snowflake ID generator.
/// </summary>
[PublicAPI]
public interface ISnowflakeIdGenerator<out TId> where TId : IComparable, IEquatable<TId>, IComparable<TId>
{
    /// <summary>
    /// Generates a new snowflake Id.
    /// </summary>
    /// <returns>Newly created snowflake Id.</returns>
    TId GenerateId();
}
