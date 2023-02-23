namespace DataExplorer.EfCore;

/// <summary>
/// Datetime filling strategy for <see cref="IUpdatedAt"/> and <see cref="ICreatedAt"/> entities.
/// </summary>
[PublicAPI]
public enum DateTimeStrategy
{
    /// <summary>
    /// <see cref="DateTime.UtcNow"/> strategy.
    /// </summary>
    UtcNow,
    /// <summary>
    /// <see cref="DateTime.Now"/> strategy.
    /// </summary>
    Now
}
