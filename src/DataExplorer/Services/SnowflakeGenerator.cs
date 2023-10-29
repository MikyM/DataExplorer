using IdGen;

namespace DataExplorer.Services;

/// <summary>
/// Default snowflake ID generator.
/// </summary>
[PublicAPI]
public class SnowflakeGenerator : ISnowflakeGenerator
{
    private readonly IIdGenerator<long> _inner;

    public SnowflakeGenerator(IIdGenerator<long> inner)
    {
        _inner = inner;
    }
    
    /// <inheritdoc/>
    public long Generate()
        => _inner.CreateId();
}
