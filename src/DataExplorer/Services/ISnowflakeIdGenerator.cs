﻿namespace DataExplorer.Services;

/// <summary>
/// Represents a snowflake ID generator.
/// </summary>
[PublicAPI]
public interface ISnowflakeIdGenerator
{
    /// <summary>
    /// Generates a new snowflake Id.
    /// </summary>
    /// <returns>Newly created snowflake Id.</returns>
    long GenerateId();
}
