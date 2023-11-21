using System.Linq.Expressions;
using AutoMapper;
using DataExplorer.Abstractions.Entities;
using DataExplorer.MongoDb.Abstractions.DataContexts;

// ReSharper disable SuspiciousTypeConversion.Global

namespace DataExplorer.MongoDb.Repositories;

/// <summary>
/// Repository.
/// </summary>
/// <inheritdoc cref="IMongoRepository{TEntity}"/>
[PublicAPI]
public class MongoRepository<TEntity> : MongoReadOnlyRepository<TEntity>, IMongoRepository<TEntity>
    where TEntity : MongoEntity
{
    internal MongoRepository(IMongoDbContext context, IMapper mapper) : base(context, mapper)
    {
    }

    /// <inheritdoc/>
    public Task CreateCollectionAsync(Action<CreateCollectionOptions<TEntity>> options,
        CancellationToken cancellation = default)
        => Context.CreateCollectionAsync(options, cancellation);

    /// <inheritdoc/>
    public Task DropCollectionAsync()
        => Context.DropCollectionAsync<TEntity>();

    /// <inheritdoc/>
    public Task<DeleteResult> DeleteAsync(string id, CancellationToken cancellation = default, bool ignoreGlobalFilters = false)
        => Context.DeleteAsync<TEntity>(id, cancellation, ignoreGlobalFilters);

    /// <inheritdoc/>
    public Task<DeleteResult> DeleteAsync(IEnumerable<string> ids, CancellationToken cancellation = default, bool ignoreGlobalFilters = false)
        => Context.DeleteAsync<TEntity>(ids, cancellation, ignoreGlobalFilters);

    /// <inheritdoc/>
    public Task<DeleteResult> DeleteAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellation = default, Collation? collation = null,
        bool ignoreGlobalFilters = false)
        => Context.DeleteAsync(expression, cancellation, collation, ignoreGlobalFilters);

    /// <inheritdoc/>
    public Task<DeleteResult> DeleteAsync(Func<FilterDefinitionBuilder<TEntity>, FilterDefinition<TEntity>> filter, CancellationToken cancellation = default, Collation? collation = null,
        bool ignoreGlobalFilters = false)
        => Context.DeleteAsync(filter, cancellation, collation, ignoreGlobalFilters);

    /// <inheritdoc/>
    public Task<DeleteResult> DeleteAsync(FilterDefinition<TEntity> filter, CancellationToken cancellation = default, Collation? collation = null,
        bool ignoreGlobalFilters = false)
        => Context.DeleteAsync(filter, cancellation, collation, ignoreGlobalFilters);
    
    /// <inheritdoc/>
    public Task InsertAsync(TEntity entity, CancellationToken cancellation = default)
        => Context.InsertAsync(entity, cancellation);
    
    /// <inheritdoc/>
    public Task<BulkWriteResult<TEntity>> InsertAsync(IEnumerable<TEntity> entities, CancellationToken cancellation = default)
        => Context.InsertAsync(entities, cancellation);

    /// <inheritdoc/>
    public Replace<TEntity> Replace()
        => Context.Replace<TEntity>();

    /// <inheritdoc/>
    public Task SaveAsync(TEntity entity, CancellationToken cancellation = default)
        => Context.SaveAsync(entity, cancellation);

    /// <inheritdoc/>
    public Task<BulkWriteResult<TEntity>> SaveAsync(IEnumerable<TEntity> entities, CancellationToken cancellation = default)
        => Context.SaveAsync(entities, cancellation);

    /// <inheritdoc/>
    public Task<UpdateResult> SaveOnlyAsync(TEntity entity, Expression<Func<TEntity, object?>> members, CancellationToken cancellation = default)
        => Context.SaveOnlyAsync(entity, members, cancellation);

    /// <inheritdoc/>
    public Task<UpdateResult> SaveOnlyAsync(TEntity entity, IEnumerable<string> propNames, CancellationToken cancellation = default)
        => Context.SaveOnlyAsync(entity, propNames, cancellation);

    /// <inheritdoc/>
    public Task<BulkWriteResult<TEntity>> SaveOnlyAsync(IEnumerable<TEntity> entities, Expression<Func<TEntity, object?>> members, CancellationToken cancellation = default)
        => Context.SaveOnlyAsync(entities, members, cancellation);

    /// <inheritdoc/>
    public Task<BulkWriteResult<TEntity>> SaveOnlyAsync(IEnumerable<TEntity> entities, IEnumerable<string> propNames, CancellationToken cancellation = default)
        => Context.SaveOnlyAsync(entities, propNames, cancellation);

    /// <inheritdoc/>
    public Task<UpdateResult> SaveExceptAsync(TEntity entity, Expression<Func<TEntity, object?>> members, CancellationToken cancellation = default)
        => Context.SaveExceptAsync(entity, members, cancellation);

    /// <inheritdoc/>
    public Task<UpdateResult> SaveExceptAsync(TEntity entity, IEnumerable<string> propNames, CancellationToken cancellation = default)
        => Context.SaveExceptAsync(entity, propNames, cancellation);

    /// <inheritdoc/>
    public Task<BulkWriteResult<TEntity>> SaveExceptAsync(IEnumerable<TEntity> entities, Expression<Func<TEntity, object?>> members, CancellationToken cancellation = default)
        => Context.SaveExceptAsync(entities, members, cancellation);

    /// <inheritdoc/>
    public Task<BulkWriteResult<TEntity>> SaveExceptAsync(IEnumerable<TEntity> entities, IEnumerable<string> propNames, CancellationToken cancellation = default)
        => Context.SaveExceptAsync(entities, propNames, cancellation);

    /// <inheritdoc/>
    public Task<UpdateResult> SavePreservingAsync(TEntity entity, CancellationToken cancellation = default)
        => Context.SavePreservingAsync(entity, cancellation);

    /// <inheritdoc/>
    public Update<TEntity> Update()
        => Context.Update<TEntity>();

    /// <inheritdoc/>
    public UpdateAndGet<TEntity, TProjection> UpdateAndGet<TProjection>()
        => Context.UpdateAndGet<TEntity, TProjection>();

    /// <inheritdoc/>
    public UpdateAndGet<TEntity,TEntity> UpdateAndGet()
        => Context.UpdateAndGet<TEntity, TEntity>();

    public virtual Task DisableAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        if (entity is not IDisableable disableableEntity)
            throw new InvalidOperationException("Can't disable an entity that isn't disableable.");

        disableableEntity.IsDisabled = true;
        return entity.SaveAsync(cancellation: cancellationToken);;
    }

    /// <inheritdoc />
    public virtual async Task DisableRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        var list = entities.ToList();
        
        if (list.FirstOrDefault() is not IDisableable)
            throw new InvalidOperationException("Can't disable an entity that isn't disableable.");
        
        foreach (var entity in list) 
            ((IDisableable)entity).IsDisabled = true;

        await SaveAsync(list, cancellationToken);;
    }
}
