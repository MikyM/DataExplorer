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
    public void FillId(Entity<long> entity)
    {
        entity.SetIdInternal(_idGenerator.GenerateId());
    }

    /// <inheritdoc/>
    public void FillIds(IEnumerable<Entity<long>> entities)
    {
        foreach (var entity in entities)
        {
            entity.SetIdInternal(_idGenerator.GenerateId());
        }
    }
}
