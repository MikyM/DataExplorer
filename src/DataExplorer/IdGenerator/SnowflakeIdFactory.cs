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
    public static int DefaultFactoryId => 1;

    /// <summary>
    /// Adds a specified creation factory.
    /// </summary>
    /// <param name="creationFactory">The creation factory.</param>
    /// <param name="factoryId">Factory Id.</param>
    public static void AddFactoryMethod(Func<long> creationFactory, int factoryId)
        => _factories.TryAdd(factoryId, creationFactory);
    
    /// <summary>
    /// Removes a specified creation factory.
    /// </summary>
    /// <param name="factoryId">Factory Id.</param>
    public static void RemoveFactoryMethod(int factoryId)
        => _factories.Remove(factoryId);

    /// <summary>
    /// Creates a new snowflake Id based on a first registered factory.
    /// </summary>
    /// <returns>Generated snowflake ID.</returns>
    public static long CreateId()
    {
        if (_factories.Count == 0) 
            throw new InvalidOperationException("You can not create an instance without first adding a factory.");
        return _factories[DefaultFactoryId]();
    }
    
    /// <summary>
    /// Creates a new snowflake Id based on a passed registered id.
    /// </summary>
    /// <param name="factoryId">Id of the factory.</param>
    /// <returns>Generated snowflake ID.</returns>
    public static long CreateId(int factoryId)
    {
        if (!_factories.TryGetValue(factoryId, out var df))
            throw new InvalidOperationException("Couldn't find generator factory registered for default generator name.");
        return df();
    }
}
