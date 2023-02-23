namespace DataExplorer.EfCore;

/// <summary>
/// Datetime filling strategy for <see cref="IUpdatedAt"/> and <see cref="ICreatedAt"/> entities.
/// </summary>
[PublicAPI]
public enum DateTimeStrategy
{
    /// <summary>
    /// 
    /// </summary>
    UtcNow,
    Now
}
