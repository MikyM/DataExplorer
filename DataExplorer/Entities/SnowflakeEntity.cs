using DataExplorer.Abstractions.Entities;
using DataExplorer.IdGenerator;

namespace DataExplorer.Entities;

/// <summary>
/// A base snowflake entity.
/// </summary>
[PublicAPI]
public abstract class SnowflakeEntity : Entity, ISnowflakeEntity<long>
{
    /// <summary>
    /// Whether to fill this entity's Id automatically.
    /// </summary>
    public virtual bool ShouldHaveIdFilled => true;

    /// <summary>
    /// The ID of the entity.
    /// </summary>
    public override long Id { get; protected set; } = SnowflakeIdFactory.CreateId();
}
