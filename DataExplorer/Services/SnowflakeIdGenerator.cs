using IdGen;

namespace DataExplorer.Services;

/// <summary>
/// Default snowflake ID generator.
/// </summary>
[PublicAPI]
public class SnowflakeIdGenerator : ISnowflakeIdGenerator
{
    private readonly IIdGenerator<long> _inner;

    public SnowflakeIdGenerator(IIdGenerator<long> inner)
    {
        _inner = inner;
    }
    
    /// <inheritdoc/>
    public object GenerateId()
        => _inner.CreateId();
}
