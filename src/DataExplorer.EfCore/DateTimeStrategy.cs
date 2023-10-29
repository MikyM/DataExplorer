namespace DataExplorer.EfCore;

/// <summary>
/// Filling strategy for <see cref="IUpdatedAt"/>, <see cref="IUpdatedAtOffset"/>, <see cref="ICreatedAtOffset"/> and <see cref="ICreatedAt"/> entities.
/// </summary>
[PublicAPI]
public enum DateTimeStrategy
{
    /// <summary>
    /// Strategy based on <see cref="TimeProvider"/> <see cref="TimeProvider.GetUtcNow"/>.
    /// </summary>
    UtcNow,
    /// <summary>
    /// Strategy based on <see cref="TimeProvider"/> <see cref="TimeProvider.GetLocalNow"/>.
    /// </summary>
    Now
}
