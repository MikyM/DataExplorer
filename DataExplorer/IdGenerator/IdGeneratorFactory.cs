namespace DataExplorer.IdGenerator;

/// <summary>
/// Factory used to generate snowflake Ids using <see cref="IdGenerator"/>.
/// </summary>
[PublicAPI]
public static class IdGeneratorFactory
{
    /// <summary>
    /// The factory used to create an instance of a <see cref="IdGenerator"/>.
    /// </summary>
    private static Dictionary<int, Func<IdGen.IdGenerator>> _factories = new();

    /// <summary>
    /// Initializes the specified creation factory.
    /// </summary>
    /// <param name="creationFactory">The creation factory.</param>
    /// <param name="generatorId">Generator Id.</param>
    public static void AddFactoryMethod(Func<IdGen.IdGenerator> creationFactory, int generatorId)
        => _factories.TryAdd(generatorId, creationFactory);

    /// <summary>
    /// Creates a <see cref="IdGenerator"/> instance based on the first registered factory method.
    /// </summary>
    /// <returns>Returns an instance of an <see cref="IdGenerator"/>.</returns>
    public static IdGen.IdGenerator Build()
    {
        if (_factories.Count == 0) 
            throw new InvalidOperationException("You can not create an instance without first adding a factory.");
        return _factories.First().Value();
    }
    
    /// <summary>
    /// Creates a <see cref="IdGenerator"/> instance.
    /// </summary>
    /// <param name="generatorId">The Id of the generator.</param>
    /// <returns>Returns an instance of an <see cref="IdGenerator"/>.</returns>
    public static IdGen.IdGenerator Build(int generatorId)
    {
        if (!_factories.TryGetValue(generatorId, out var df))
            throw new InvalidOperationException("Couldn't find generator factory registered for default generator name.");
        return df();
    }
}
