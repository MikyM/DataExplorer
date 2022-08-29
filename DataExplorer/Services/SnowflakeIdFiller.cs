using DataExplorer.Abstractions.Entities;

namespace DataExplorer.Services;

/// <inheritdoc cref="ISnowflakeIdFiller"/>
[PublicAPI]
public class SnowflakeIdFiller : ISnowflakeIdFiller
{
    public SnowflakeIdFiller(ISnowflakeIdGenerator idGenerator)
    {
        _idGenerator = idGenerator;
    }

    private readonly ISnowflakeIdGenerator _idGenerator;
    
    /// <inheritdoc/>
    public void FillId(ISnowflakeEntity entity)
    {
        entity.SetId(_idGenerator.GenerateId());
    }

    /// <inheritdoc/>
    public void FillIds(IEnumerable<ISnowflakeEntity> entities)
    {
        foreach (var entity in entities)
        {
            entity.SetId(_idGenerator.GenerateId());
        }
    }
}
