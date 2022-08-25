namespace DataExplorer.Services;

/// <summary>
/// Defines a snowflake ID generator.
/// </summary>
[PublicAPI]
public interface ISnowflakeIdGenerator
{
    /// <summary>
    /// Generates a new snowflake Id.
    /// </summary>
    /// <returns>Newly created snowflake Id.</returns>
    object GenerateId();
}
