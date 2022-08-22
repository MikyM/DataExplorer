using DataExplorer.Abstractions.Entities;
using DataExplorer.IdGenerator;

namespace DataExplorer.Entities;

/// <summary>
/// A snowflake entity.
/// </summary>
[PublicAPI]
public class SnowflakeEntity : Entity, ISnowflakeEntity<long>
{
    /// <inheritdoc/>
    public override long Id { get; protected set; } = IdGeneratorFactory.Build().CreateId();

    public long GenerateSnowflakeId()
        => IdGeneratorFactory.Build().CreateId();
}
