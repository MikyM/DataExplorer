using DataExplorer.Abstractions.Entities;
using DataExplorer.Entities;

namespace DataExplorer.Services;

/// <inheritdoc cref="ISnowflakeIdFiller"/>
[PublicAPI]
public class SnowflakeIdFiller : ISnowflakeIdFiller
{
    public SnowflakeIdFiller(ISnowflakeIdGenerator<long> idGenerator)
    {
        _idGenerator = idGenerator;
    }

    private readonly ISnowflakeIdGenerator<long> _idGenerator;
    
    /// <inheritdoc/>
    public void FillId(IEntityBase entity)
    {
        if (entity is Entity<long> longEntity)
            longEntity.SetIdInternal(_idGenerator.GenerateId());
    }

    /// <inheritdoc/>
    public void FillIds(IEnumerable<IEntityBase> entities)
    {
        foreach (var entity in entities.OfType<Entity<long>>())
        {
            entity.SetIdInternal(_idGenerator.GenerateId());
        }
    }
}
