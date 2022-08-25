using DataExplorer.Abstractions.Entities;
using DataExplorer.IdGenerator;

namespace DataExplorer.Entities;

/// <summary>
/// A snowflake entity.
/// </summary>
[PublicAPI]
public abstract class SnowflakeEntity : Entity, ISnowflakeEntity<long>
{
}
