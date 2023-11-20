namespace DataExplorer.EfCore;

#if NET8_0_OR_GREATER
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
#else
/// <summary>
/// Filling strategy for <see cref="IUpdatedAt"/>, <see cref="IUpdatedAtOffset"/>, <see cref="ICreatedAtOffset"/> and <see cref="ICreatedAt"/> entities.
/// </summary>
[PublicAPI]
public enum DateTimeStrategy
{
    /// <summary>
    /// Strategy based on <see cref="DateTimeOffset.UtcNow"/>.
    /// </summary>
    UtcNow,
    /// <summary>
    /// Strategy based on <see cref="DateTimeOffset.Now"/>.
    /// </summary>
    Now
}
#endif

