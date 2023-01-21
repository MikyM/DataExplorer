using System.Linq.Expressions;
using AutoMapper;
using DataExplorer.Abstractions.Entities;
using DataExplorer.Exceptions;
using DataExplorer.MongoDb.Abstractions.DataContexts;
using MongoDB.Driver;
using MongoDB.Entities;
using IEntity = MongoDB.Entities.IEntity;
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
    public async Task CreateCollectionAsync(Action<CreateCollectionOptions<TEntity>> options,
        CancellationToken cancellation = default)
        => await Context.CreateCollectionAsync(options, cancellation).ConfigureAwait(false);

    /// <inheritdoc/>
    public async Task DropCollectionAsync()
        => await Context.DropCollectionAsync<TEntity>().ConfigureAwait(false);

    /// <inheritdoc/>
    public async Task<DeleteResult> DeleteAsync(string id, CancellationToken cancellation = default, bool ignoreGlobalFilters = false)
        => await Context.DeleteAsync<TEntity>(id, cancellation, ignoreGlobalFilters).ConfigureAwait(false);

    /// <inheritdoc/>
    public async Task<DeleteResult> DeleteAsync(IEnumerable<string> ids, CancellationToken cancellation = default, bool ignoreGlobalFilters = false)
        => await Context.DeleteAsync<TEntity>(ids, cancellation, ignoreGlobalFilters).ConfigureAwait(false);

    /// <inheritdoc/>
    public async Task<DeleteResult> DeleteAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellation = default, Collation? collation = null,
        bool ignoreGlobalFilters = false)
        => await Context.DeleteAsync(expression, cancellation, collation, ignoreGlobalFilters).ConfigureAwait(false);

    /// <inheritdoc/>
    public async Task<DeleteResult> DeleteAsync(Func<FilterDefinitionBuilder<TEntity>, FilterDefinition<TEntity>> filter, CancellationToken cancellation = default, Collation? collation = null,
        bool ignoreGlobalFilters = false)
        => await Context.DeleteAsync(filter, cancellation, collation, ignoreGlobalFilters).ConfigureAwait(false);

    /// <inheritdoc/>
    public async Task<DeleteResult> DeleteAsync(FilterDefinition<TEntity> filter, CancellationToken cancellation = default, Collation? collation = null,
        bool ignoreGlobalFilters = false)
        => await Context.DeleteAsync(filter, cancellation, collation, ignoreGlobalFilters).ConfigureAwait(false);
    
    /// <inheritdoc/>
    public async Task InsertAsync(TEntity entity, CancellationToken cancellation = default)
        => await Context.InsertAsync(entity, cancellation).ConfigureAwait(false);
    
    /// <inheritdoc/>
    public async Task<BulkWriteResult<TEntity>> InsertAsync(IEnumerable<TEntity> entities, CancellationToken cancellation = default)
        => await Context.InsertAsync(entities, cancellation).ConfigureAwait(false);

    /// <inheritdoc/>
    public Replace<TEntity> Replace()
        => Context.Replace<TEntity>();

    /// <inheritdoc/>
    public async Task SaveAsync(TEntity entity, CancellationToken cancellation = default)
        => await Context.SaveAsync(entity, cancellation).ConfigureAwait(false);

    /// <inheritdoc/>
    public async Task<BulkWriteResult<TEntity>> SaveAsync(IEnumerable<TEntity> entities, CancellationToken cancellation = default)
        => await Context.SaveAsync(entities, cancellation).ConfigureAwait(false);

    /// <inheritdoc/>
    public async Task<UpdateResult> SaveOnlyAsync(TEntity entity, Expression<Func<TEntity, object>> members, CancellationToken cancellation = default)
        => await Context.SaveOnlyAsync(entity, members, cancellation).ConfigureAwait(false);

    /// <inheritdoc/>
    public async Task<UpdateResult> SaveOnlyAsync(TEntity entity, IEnumerable<string> propNames, CancellationToken cancellation = default)
        => await Context.SaveOnlyAsync(entity, propNames, cancellation).ConfigureAwait(false);

    /// <inheritdoc/>
    public async Task<BulkWriteResult<TEntity>> SaveOnlyAsync(IEnumerable<TEntity> entities, Expression<Func<TEntity, object>> members, CancellationToken cancellation = default)
        => await Context.SaveOnlyAsync(entities, members, cancellation).ConfigureAwait(false);

    /// <inheritdoc/>
    public async Task<BulkWriteResult<TEntity>> SaveOnlyAsync(IEnumerable<TEntity> entities, IEnumerable<string> propNames, CancellationToken cancellation = default)
        => await Context.SaveOnlyAsync(entities, propNames, cancellation).ConfigureAwait(false);

    /// <inheritdoc/>
    public async Task<UpdateResult> SaveExceptAsync(TEntity entity, Expression<Func<TEntity, object>> members, CancellationToken cancellation = default)
        => await Context.SaveExceptAsync(entity, members, cancellation).ConfigureAwait(false);

    /// <inheritdoc/>
    public async Task<UpdateResult> SaveExceptAsync(TEntity entity, IEnumerable<string> propNames, CancellationToken cancellation = default)
        => await Context.SaveExceptAsync(entity, propNames, cancellation).ConfigureAwait(false);

    /// <inheritdoc/>
    public async Task<BulkWriteResult<TEntity>> SaveExceptAsync(IEnumerable<TEntity> entities, Expression<Func<TEntity, object>> members, CancellationToken cancellation = default)
        => await Context.SaveExceptAsync(entities, members, cancellation).ConfigureAwait(false);

    /// <inheritdoc/>
    public async Task<BulkWriteResult<TEntity>> SaveExceptAsync(IEnumerable<TEntity> entities, IEnumerable<string> propNames, CancellationToken cancellation = default)
        => await Context.SaveExceptAsync(entities, propNames, cancellation).ConfigureAwait(false);

    /// <inheritdoc/>
    public async Task<UpdateResult> SavePreservingAsync(TEntity entity, CancellationToken cancellation = default)
        => await Context.SavePreservingAsync(entity, cancellation).ConfigureAwait(false);

    /// <inheritdoc/>
    public Update<TEntity> Update()
        => Context.Update<TEntity>();

    /// <inheritdoc/>
    public UpdateAndGet<TEntity, TProjection> UpdateAndGet<TProjection>()
        => Context.UpdateAndGet<TEntity, TProjection>();

    /// <inheritdoc/>
    public UpdateAndGet<TEntity,TEntity> UpdateAndGet()
        => Context.UpdateAndGet<TEntity, TEntity>();

    public virtual async Task DisableAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        if (entity is not IDisableableEntity disableableEntity)
            throw new InvalidOperationException("Can't disable an entity that isn't disableable.");

        disableableEntity.IsDisabled = true;
        await entity.SaveAsync(cancellation: cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task DisableRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        var list = entities.ToList();
        
        if (list.FirstOrDefault() is not IDisableableEntity)
            throw new InvalidOperationException("Can't disable an entity that isn't disableable.");
        
        foreach (var entity in list) 
            ((IDisableableEntity)entity).IsDisabled = true;

        await SaveAsync(list, cancellationToken);
    }
}
