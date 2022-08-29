using DataExplorer.Abstractions.Entities;

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
}
