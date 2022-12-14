using DataExplorer.Abstractions.Entities;
namespace DataExplorer.Services;

/// <summary>
/// Default Snowflake Id filler for <see cref="IEntity{TId}"/>.
/// </summary>
[PublicAPI]
public interface ISnowflakeIdFiller
{
    /// <summary>
    /// Fills Id for a single entity.
    /// </summary>
    /// <param name="entity">Entity to handle.</param>
    void FillId(ISnowflakeEntity entity);
    
    /// <summary>
    /// Fills Ids for a list of entities.
    /// </summary>
    /// <param name="entities">Entities to handle.</param>
    void FillIds(IEnumerable<ISnowflakeEntity> entities);
}
