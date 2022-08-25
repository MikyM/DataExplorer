using IdGen;

namespace DataExplorer.Services;

/// <summary>
/// Default snowflake ID generator.
/// </summary>
[PublicAPI]
public class SnowflakeIdGenerator : ISnowflakeIdGenerator<long>
{
    private readonly IIdGenerator<long> _inner;

    public SnowflakeIdGenerator(IIdGenerator<long> inner)
    {
        _inner = inner;
    }
    
    /// <inheritdoc/>
    public long GenerateId()
        => _inner.CreateId();
}
