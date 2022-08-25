using DataExplorer.Entities;

namespace DataExplorer.Services;

/// <inheritdoc cref="ISnowflakeIdFiller"/>
[PublicAPI]
public class SnowflakeIdFiller : ISnowflakeIdFiller
{
    public SnowflakeIdFiller(ISnowflakeIdGenerator<long> idGenerator)
    {
        IdGenerator = idGenerator;
    }

    /// <inheritdoc/>
    public ISnowflakeIdGenerator<long> IdGenerator { get; }
    
    /// <inheritdoc/>
    public void FillId(Entity<long> entity)
    {
        entity.SetIdInternal(IdGenerator.GenerateId());
    }

    /// <inheritdoc/>
    public void FillIds(IEnumerable<Entity<long>> entities)
    {
        foreach (var entity in entities)
        {
            entity.SetIdInternal(IdGenerator.GenerateId());
        }
    }
}
