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
        if (entity.ShouldHaveIdFilled)
            entity.SetId(_idGenerator.GenerateId());
    }
    
    /// <inheritdoc/>
    public void FillId<TId>(ISnowflakeEntity<TId> entity) where TId : IComparable, IEquatable<TId>, IComparable<TId>
    {
        if (entity.ShouldHaveIdFilled)
            entity.SetId(_idGenerator.GenerateId());
    }
    
    /// <inheritdoc/>
    public void FillId(ISnowflakeEntity<long> entity)
    {
        if (entity.ShouldHaveIdFilled)
            entity.SetId(_idGenerator.GenerateId());
    }

    /// <inheritdoc/>
    public void FillIds(IEnumerable<ISnowflakeEntity> entities)
    {
        foreach (var entity in entities)
        {
            FillId(entity);
        }
    }
    
    /// <inheritdoc/>
    public void FillIds<TId>(IEnumerable<ISnowflakeEntity<TId>> entities)
        where TId : IComparable, IEquatable<TId>, IComparable<TId>
    {
        foreach (var entity in entities)
        {
            FillId(entity);
        }
    }
    
    /// <inheritdoc/>
    public void FillIds(IEnumerable<ISnowflakeEntity<long>> entities)
    {
        foreach (var entity in entities)
        {
            FillId(entity);
        }
    }
}
