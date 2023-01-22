using System.Linq.Expressions;
using AutoMapper;
using DataExplorer.Abstractions.Repositories;
using DataExplorer.MongoDb.Abstractions;
using DataExplorer.MongoDb.Abstractions.DataContexts;
using DataExplorer.MongoDb.Abstractions.DataServices;

namespace DataExplorer.MongoDb.DataServices;

/// <summary>
/// Read-only data service.
/// </summary>
/// <inheritdoc cref="IMongoReadOnlyDataService{TEntity,TContext}"/>
[PublicAPI]
public class MongoReadOnlyDataService<TEntity, TContext> : MongoDataServiceBase<TContext>,
    IMongoReadOnlyDataService<TEntity, TContext>
    where TEntity : MongoEntity
    where TContext : class, IMongoDbContext
{
    /// <summary>
    /// Creates a new instance of <see cref="IMongoReadOnlyDataService{TEntity,TContext}"/>.
    /// </summary>
    /// <param name="uof">Instance of <see cref="IMongoUnitOfWork"/>.</param>
    public MongoReadOnlyDataService(IMongoUnitOfWork<TContext> uof) : base(uof)
    {
    }

    /// <summary>
    /// Gets the base repository for this data service.
    /// </summary>
    internal virtual IRepositoryBase BaseRepositoryInternal => UnitOfWork.GetRepository<IMongoReadOnlyRepository<TEntity>>();
    
    /// <summary>
    /// Gets the base repository for this data service.
    /// </summary>
    protected IRepositoryBase BaseRepository => BaseRepositoryInternal;
    
    /// <summary>
    /// Gets the read-only version of the <see cref="BaseRepository"/> (essentially casts it for you).
    /// </summary>
    protected IMongoReadOnlyRepository<TEntity> ReadOnlyRepository =>
        (IMongoReadOnlyRepository<TEntity>)BaseRepository;

    /// <inheritdoc />
    public virtual async Task<Result<IReadOnlyList<TGetResult>>> GetAllAsync<TGetResult>(CancellationToken cancellationToken = default)
        where TGetResult : class
    {
        try
        {
            return Result<IReadOnlyList<TGetResult>>.FromSuccess(Mapper.Map<IReadOnlyList<TGetResult>>(
                await ReadOnlyRepository.GetAllAsync(cancellationToken).ConfigureAwait(false)));
        }
        catch (Exception ex)
        {
            return new ExceptionError(ex);
        }
    }
    
    /// <inheritdoc />
    public virtual async Task<Result<IReadOnlyList<TEntity>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return Result<IReadOnlyList<TEntity>>.FromSuccess(await ReadOnlyRepository.GetAllAsync(cancellationToken).ConfigureAwait(false));
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    /// <inheritdoc />
    public async Task<Result<long>> CountEstimatedAsync(CancellationToken cancellation = default)
    {
        try
        {
            return await ReadOnlyRepository.CountEstimatedAsync(cancellation).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    /// <inheritdoc />
    public async Task<Result<long>> CountAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellation = default, CountOptions? options = null,
        bool ignoreGlobalFilters = false)
    {
        try
        {
            return await ReadOnlyRepository.CountAsync(expression, cancellation, options, ignoreGlobalFilters).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    /// <inheritdoc />
    public async Task<Result<long>> CountAsync(CancellationToken cancellation = default)
    {
        try
        {
            return await ReadOnlyRepository.CountAsync(cancellation).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    /// <inheritdoc />
    public async Task<Result<long>> CountAsync(FilterDefinition<TEntity> filter, CancellationToken cancellation = default, CountOptions? options = null,
        bool ignoreGlobalFilters = false)
    {
        try
        {
            return await ReadOnlyRepository.CountAsync(filter, cancellation, options, ignoreGlobalFilters).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    /// <inheritdoc />
    public async Task<Result<long>> CountAsync(Func<FilterDefinitionBuilder<TEntity>, FilterDefinition<TEntity>> filter, CancellationToken cancellation = default, CountOptions? options = null,
        bool ignoreGlobalFilters = false)
    {
        try
        {
            return await ReadOnlyRepository.CountAsync(filter, cancellation, options, ignoreGlobalFilters).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    /// <inheritdoc />
    public Distinct<TEntity, TProperty> Distinct<TProperty>()
        => ReadOnlyRepository.Distinct<TProperty>();

    /// <inheritdoc />
    public Find<TEntity> Find()
        => ReadOnlyRepository.Find();

    /// <inheritdoc />
    public Find<TEntity, TProjection> Find<TProjection>()
        => ReadOnlyRepository.Find<TProjection>();

    /// <inheritdoc />
    public IAggregateFluent<TEntity> Fluent(AggregateOptions? options = null, bool ignoreGlobalFilters = false)
        => ReadOnlyRepository.Fluent(options, ignoreGlobalFilters);

    /// <inheritdoc />
    public IAggregateFluent<TEntity> FluentTextSearch(Search searchType, string searchTerm, bool caseSensitive = false,
        bool diacriticSensitive = false, string? language = null, AggregateOptions? options = null,
        bool ignoreGlobalFilters = false)
        => ReadOnlyRepository.FluentTextSearch(searchType, searchTerm, caseSensitive, diacriticSensitive, language, options, ignoreGlobalFilters);

    /// <inheritdoc />
    public IAggregateFluent<TEntity> GeoNear(Coordinates2D nearCoordinates, Expression<Func<TEntity, object>> distanceField, bool spherical = true,
        int? maxDistance = null, int? minDistance = null, int? limit = null, BsonDocument? query = null,
        int? distanceMultiplier = null, Expression<Func<TEntity, object>>? includeLocations = null,
        string? indexKey = null,
        AggregateOptions? options = null, bool ignoreGlobalFilters = false)
        => ReadOnlyRepository.GeoNear(nearCoordinates, distanceField, spherical, maxDistance, minDistance, limit, query,
            distanceMultiplier, includeLocations, indexKey, options, ignoreGlobalFilters);

    /// <inheritdoc />
    public PagedSearch<TEntity> PagedSearch()
        => ReadOnlyRepository.PagedSearch();

    /// <inheritdoc />
    public PagedSearch<TEntity, TProjection> PagedSearch<TProjection>()
        => ReadOnlyRepository.PagedSearch<TProjection>();

    /// <inheritdoc />
    public async Task<Result<IAsyncCursor<TResult>>> PipelineCursorAsync<TResult>(Template<TEntity, TResult> template,
        AggregateOptions? options = null,
        CancellationToken cancellation = default, bool ignoreGlobalFilters = false)
    {
        try
        {
            return Result<IAsyncCursor<TResult>>.FromSuccess(await ReadOnlyRepository.PipelineCursorAsync(template, options, cancellation, ignoreGlobalFilters).ConfigureAwait(false));
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    /// <inheritdoc />
    public async Task<Result<IReadOnlyList<TResult>>> PipelineAsync<TResult>(Template<TEntity, TResult> template, AggregateOptions? options = null,
        CancellationToken cancellation = default, bool ignoreGlobalFilters = false)
    {
        try
        {
            return Result<IReadOnlyList<TResult>>.FromSuccess(await ReadOnlyRepository.PipelineAsync(template, options, cancellation, ignoreGlobalFilters).ConfigureAwait(false));
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    /// <inheritdoc />
    public async Task<Result<TResult>> PipelineSingleAsync<TResult>(Template<TEntity, TResult> template, AggregateOptions? options = null,
        CancellationToken cancellation = default, bool ignoreGlobalFilters = false)
    {
        try
        {
            return Result<TResult>.FromSuccess(await ReadOnlyRepository.PipelineSingleAsync(template, options, cancellation, ignoreGlobalFilters).ConfigureAwait(false));
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    /// <inheritdoc />
    public async Task<Result<TResult>> PipelineFirstAsync<TResult>(Template<TEntity, TResult> template, AggregateOptions? options = null,
        CancellationToken cancellation = default, bool ignoreGlobalFilters = false)
    {
        try
        {
            return Result<TResult>.FromSuccess(await ReadOnlyRepository.PipelineFirstAsync(template, options, cancellation, ignoreGlobalFilters).ConfigureAwait(false));
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    /// <inheritdoc />
    public IMongoQueryable<T> Queryable<T>(AggregateOptions? options = null, bool ignoreGlobalFilters = false)
        where T : MongoEntity
        => ReadOnlyRepository.Queryable<T>();

    /// <inheritdoc />
    public virtual async Task<Result<long>> LongCountAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return Result<long>.FromSuccess(await ReadOnlyRepository.LongCountAsync(cancellationToken).ConfigureAwait(false));
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    /// <inheritdoc />
    public async Task<Result<long>> LongCountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        try
        {
            return Result<long>.FromSuccess(await ReadOnlyRepository.LongCountAsync(predicate, cancellationToken).ConfigureAwait(false));
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    /// <inheritdoc />
    public async Task<Result<bool>> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        try
        {
            return Result<bool>.FromSuccess(await ReadOnlyRepository.AnyAsync(predicate, cancellationToken).ConfigureAwait(false));
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    /// <inheritdoc />
    public async Task<Result<IReadOnlyList<TEntity>>> WhereAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        try
        {
            return Result<IReadOnlyList<TEntity>>.FromSuccess(await ReadOnlyRepository.WhereAsync(predicate, cancellationToken).ConfigureAwait(false));
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
    
    /// <inheritdoc />
    public async Task<Result<TEntity?>> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        try
        {
            var res = await ReadOnlyRepository.FirstOrDefaultAsync(predicate, cancellationToken).ConfigureAwait(false);
            if (res is null)
                return new NotFoundError();

            return res;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

}
