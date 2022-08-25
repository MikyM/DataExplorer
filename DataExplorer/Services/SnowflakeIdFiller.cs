using DataExplorer.Abstractions.Entities;
using DataExplorer.Entities;

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
    public void FillId(IEntityBase entity)
    {
        entity.SetId(_idGenerator.GenerateId());
    }

    /// <inheritdoc/>
    public void FillIds(IEnumerable<IEntityBase> entities)
    {
        foreach (var entity in entities)
        {
            entity.SetId(_idGenerator.GenerateId());
        }
    }
}
