using DataExplorer.Abstractions.Entities;
using DataExplorer.IdGenerator;

namespace DataExplorer.Entities;

/// <summary>
/// A base snowflake entity.
/// </summary>
[PublicAPI]
public abstract class SnowflakeEntity : Entity, ISnowflakeEntity<long>
{
}
