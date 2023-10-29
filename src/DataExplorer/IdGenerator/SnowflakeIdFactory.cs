namespace DataExplorer.IdGenerator;

/// <summary>
/// Factory used to generate snowflake Ids using <see cref="IdGenerator"/>.
/// </summary>
[PublicAPI]
public static class SnowflakeIdFactory
{
    /// <summary>
    /// The factory used to create an id.
    /// </summary>
    private static Dictionary<int, Func<long>> _factories = new();

    /// <summary>
    /// Default factory Id.
    /// </summary>
    public const int DefaultFactoryId = 1;

    /// <summary>
    /// Adds a specified creation factory.
    /// </summary>
    /// <param name="creationFactory">The creation factory.</param>
    /// <param name="factoryId">Factory Id.</param>
    public static void AddFactoryMethod(Func<long> creationFactory, int factoryId = DefaultFactoryId)
        => _factories.TryAdd(factoryId, creationFactory);
    
    /// <summary>
    /// Removes a specified creation factory.
    /// </summary>
    /// <param name="factoryId">Factory Id.</param>
    public static void RemoveFactoryMethod(int factoryId = DefaultFactoryId)
        => _factories.Remove(factoryId);
    
    /// <summary>
    /// Creates a new snowflake Id.
    /// </summary>
    /// <param name="factoryId">Id of the factory.</param>
    /// <returns>Generated snowflake ID.</returns>
    public static long CreateId(int factoryId = DefaultFactoryId)
    {
        if (!_factories.TryGetValue(factoryId, out var df))
            throw new InvalidOperationException($"Factory method with Id {factoryId} doesn't exist.");
        return df();
    }
}
