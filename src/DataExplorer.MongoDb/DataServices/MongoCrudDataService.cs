using System.Linq.Expressions;
using DataExplorer.Abstractions.Repositories;
using DataExplorer.MongoDb.Abstractions;
using DataExplorer.MongoDb.Abstractions.DataContexts;
using DataExplorer.MongoDb.Abstractions.DataServices;

// ReSharper disable ClassWithVirtualMembersNeverInherited.Global

namespace DataExplorer.MongoDb.DataServices;

/// <summary>
/// CRUD data service.
/// </summary>
/// <inheritdoc cref="IMongoCrudDataService{TEntity,TContext}"/>
[PublicAPI]
public class MongoCrudDataService<TEntity, TContext> : MongoReadOnlyDataService<TEntity,TContext>,
    IMongoCrudDataService<TEntity, TContext>
    where TEntity : MongoEntity
    where TContext : class, IMongoDbContext
{
    /// <summary>
    /// Creates a new instance of <see cref="MongoCrudDataService{TEntity,TContext}"/>.
    /// </summary>
    /// <param name="uof">Unit of work instance.</param>
    public MongoCrudDataService(IMongoUnitOfWork<TContext> uof) : base(uof)
    {
    }

    /// <inheritdoc />
    internal override IRepositoryBase BaseRepositoryInternal => UnitOfWork.GetRepository<IMongoRepository<TEntity>>();

    /// <summary>
    /// Gets the CRUD version of the <see cref="MongoReadOnlyDataService{TEntity,TId,TContext}.BaseRepository"/> (essentially casts it for you).
    /// </summary>
    protected IMongoRepository<TEntity> Repository => (IMongoRepository<TEntity>)BaseRepository;

    /// <inheritdoc />
    public async Task<Result> CreateCollectionAsync(Action<CreateCollectionOptions<TEntity>> options, CancellationToken cancellation = default)
    {
        try
        {
            await Repository.CreateCollectionAsync(options, cancellation).ConfigureAwait(false);
            return Result.FromSuccess();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    /// <inheritdoc />
    public async Task<Result> DropCollectionAsync()
    {
        try
        {
            await Repository.DropCollectionAsync().ConfigureAwait(false);
            return Result.FromSuccess();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    /// <inheritdoc />
    public async Task<Result<DeleteResult>> DeleteAsync(string id, CancellationToken cancellation = default, bool ignoreGlobalFilters = false)
    {
        try
        {
            return await Repository.DeleteAsync(id, cancellation, ignoreGlobalFilters).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    /// <inheritdoc />
    public async Task<Result<DeleteResult>> DeleteAsync(IEnumerable<string> ids, CancellationToken cancellation = default, bool ignoreGlobalFilters = false)
    {
        try
        {
            return await Repository.DeleteAsync(ids, cancellation, ignoreGlobalFilters).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    /// <inheritdoc />
    public async Task<Result<DeleteResult>> DeleteAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellation = default, Collation? collation = null,
        bool ignoreGlobalFilters = false)
    {
        try
        {
            return await Repository.DeleteAsync(expression, cancellation, collation, ignoreGlobalFilters).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    /// <inheritdoc />
    public async Task<Result<DeleteResult>> DeleteAsync(Func<FilterDefinitionBuilder<TEntity>, FilterDefinition<TEntity>> filter, CancellationToken cancellation = default, Collation? collation = null,
        bool ignoreGlobalFilters = false)
    {
        try
        {
            return await Repository.DeleteAsync(filter, cancellation, collation, ignoreGlobalFilters).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    /// <inheritdoc />
    public async Task<Result<DeleteResult>> DeleteAsync(FilterDefinition<TEntity> filter, CancellationToken cancellation = default, Collation? collation = null,
        bool ignoreGlobalFilters = false)
    {
        try
        {
            return await Repository.DeleteAsync(filter, cancellation, collation, ignoreGlobalFilters).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    /// <inheritdoc />
    public async Task<Result> InsertAsync(TEntity entity, CancellationToken cancellation = default)
    {
        try
        {
            await Repository.InsertAsync(entity, cancellation).ConfigureAwait(false);
            return Result.FromSuccess();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    /// <inheritdoc />
    public async Task<Result<BulkWriteResult<TEntity>>> InsertAsync(IEnumerable<TEntity> entities, CancellationToken cancellation = default)
    {
        try
        {
            return await Repository.InsertAsync(entities, cancellation).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    /// <inheritdoc />
    public Replace<TEntity> Replace()
        => Repository.Replace();

    /// <inheritdoc />
    public async Task<Result> SaveAsync(TEntity entity, CancellationToken cancellation = default)
    {
        try
        {
            await Repository.SaveAsync(entity, cancellation).ConfigureAwait(false);
            return Result.FromSuccess();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    /// <inheritdoc />
    public async Task<Result<BulkWriteResult<TEntity>>> SaveAsync(IEnumerable<TEntity> entities, CancellationToken cancellation = default)
    {
        try
        {
            return await Repository.SaveAsync(entities, cancellation).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    /// <inheritdoc />
    public async Task<Result<UpdateResult>> SaveOnlyAsync(TEntity entity, Expression<Func<TEntity, object>> members, CancellationToken cancellation = default)
    {
        try
        {
            return await Repository.SaveOnlyAsync(entity, members, cancellation).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    /// <inheritdoc />
    public async Task<Result<UpdateResult>> SaveOnlyAsync(TEntity entity, IEnumerable<string> propNames, CancellationToken cancellation = default)
    {
        try
        {
            return await Repository.SaveOnlyAsync(entity, propNames, cancellation).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    /// <inheritdoc />
    public async Task<Result<BulkWriteResult<TEntity>>> SaveOnlyAsync(IEnumerable<TEntity> entities, Expression<Func<TEntity, object>> members, CancellationToken cancellation = default)
    {
        try
        {
            return await Repository.SaveOnlyAsync(entities, members, cancellation).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    /// <inheritdoc />
    public async Task<Result<BulkWriteResult<TEntity>>> SaveOnlyAsync(IEnumerable<TEntity> entities, IEnumerable<string> propNames, CancellationToken cancellation = default)
    {
        try
        {
            return await Repository.SaveOnlyAsync(entities, propNames, cancellation).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    /// <inheritdoc />
    public async Task<Result<UpdateResult>> SaveExceptAsync(TEntity entity, Expression<Func<TEntity, object>> members, CancellationToken cancellation = default)
    {
        try
        {
            return await Repository.SaveExceptAsync(entity, members, cancellation).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    /// <inheritdoc />
    public async Task<Result<UpdateResult>> SaveExceptAsync(TEntity entity, IEnumerable<string> propNames, CancellationToken cancellation = default)
    {
        try
        {
            return await Repository.SaveExceptAsync(entity, propNames, cancellation).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    /// <inheritdoc />
    public async Task<Result<BulkWriteResult<TEntity>>> SaveExceptAsync(IEnumerable<TEntity> entities, Expression<Func<TEntity, object>> members, CancellationToken cancellation = default)
    {
        try
        {
            return await Repository.SaveExceptAsync(entities, members, cancellation).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    /// <inheritdoc />
    public async Task<Result<BulkWriteResult<TEntity>>> SaveExceptAsync(IEnumerable<TEntity> entities, IEnumerable<string> propNames, CancellationToken cancellation = default)
    {
        try
        {
            return await Repository.SaveExceptAsync(entities, propNames, cancellation).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    /// <inheritdoc />
    public async Task<Result<UpdateResult>> SavePreservingAsync(TEntity entity, CancellationToken cancellation = default)
    {
        try
        {
            return await Repository.SavePreservingAsync(entity, cancellation).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    /// <inheritdoc />
    public Update<TEntity> Update()
        => Repository.Update();

    /// <inheritdoc />
    public UpdateAndGet<TEntity, TEntity> UpdateAndGet()
        => Repository.UpdateAndGet();

    /// <inheritdoc />
    public UpdateAndGet<TEntity, TProjection> UpdateAndGet<TProjection>()
        => Repository.UpdateAndGet<TProjection>();

    /// <inheritdoc />
    public virtual async Task<Result> DisableRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        try
        {
            await Repository.DisableRangeAsync(entities, cancellationToken);
            return Result.FromSuccess();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    /// <inheritdoc />
    public virtual async Task<Result> DisableAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        try
        {
            await Repository.DisableAsync(entity, cancellationToken);
            return Result.FromSuccess();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
}
