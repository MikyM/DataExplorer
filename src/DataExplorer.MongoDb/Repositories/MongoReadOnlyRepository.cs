using System.Linq.Expressions;
using AutoMapper;
using DataExplorer.Abstractions.DataContexts;
using DataExplorer.Abstractions.Repositories;
using DataExplorer.MongoDb.Abstractions.DataContexts;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using MongoDB.Entities;

namespace DataExplorer.MongoDb.Repositories;

/// <summary>
/// Read-only repository.
/// </summary>
/// <inheritdoc cref="IMongoReadOnlyRepository{TEntity}"/>
[PublicAPI]
public class MongoReadOnlyRepository<TEntity> : IMongoReadOnlyRepository<TEntity> where TEntity : MongoEntity
{
    /// <inheritdoc />
    public Type EntityType => typeof(TEntity);

    /// <inheritdoc />
    IDataContextBase IRepositoryBase.Context => Context;

    /// <inheritdoc />
    public IMongoDbContext Context { get; }
        
    /// <summary>
    /// Queryable
    /// </summary>
    public IMongoQueryable<TEntity> MongoQueryable { get; }

    /// <summary>
    /// Mapper instance.
    /// </summary>
    protected readonly IMapper Mapper;

    /// <summary>
    /// Internal ctor.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="mapper">Mapper.</param>
    /// <exception cref="ArgumentNullException"></exception>
    internal MongoReadOnlyRepository(IMongoDbContext context, IMapper mapper)
    {
        Context = context ?? throw new ArgumentNullException(nameof(context));
        /*SpecificationEvaluator = specificationEvaluator;*/
        Mapper = mapper;
        MongoQueryable = context.Queryable<TEntity>();
    }

    /// <inheritdoc />
    public async Task<long> CountEstimatedAsync(CancellationToken cancellation = default)
        => await Context.CountEstimatedAsync<TEntity>(cancellation).ConfigureAwait(false);

    /// <inheritdoc />
    public async Task<long> CountAsync(Expression<Func<TEntity, bool>> expression,
        CancellationToken cancellation = default, CountOptions? options = null,
        bool ignoreGlobalFilters = false)
        => await Context.CountAsync(expression, cancellation, options, ignoreGlobalFilters).ConfigureAwait(false);

    /// <inheritdoc />
    public async Task<long> CountAsync(CancellationToken cancellation = default)
        => await Context.CountAsync<TEntity>(cancellation).ConfigureAwait(false);

    /// <inheritdoc />
    public async Task<long> CountAsync(FilterDefinition<TEntity> filter, CancellationToken cancellation = default, CountOptions? options = null,
        bool ignoreGlobalFilters = false)
        => await Context.CountAsync(filter, cancellation, options, ignoreGlobalFilters).ConfigureAwait(false);

    /// <inheritdoc />
    public async Task<long> CountAsync(Func<FilterDefinitionBuilder<TEntity>, FilterDefinition<TEntity>> filter, CancellationToken cancellation = default, CountOptions? options = null,
        bool ignoreGlobalFilters = false)
        => await Context.CountAsync(filter, cancellation, options, ignoreGlobalFilters).ConfigureAwait(false);

    /// <inheritdoc />
    public Distinct<TEntity, TProperty> Distinct<TProperty>()
        => Context.Distinct<TEntity,TProperty>();

    /// <inheritdoc />
    public Find<TEntity> Find()
        => Context.Find<TEntity>();

    /// <inheritdoc />
    public Find<TEntity, TProjection> Find<TProjection>()
        => Context.Find<TEntity,TProjection>();

    /// <inheritdoc />
    public IAggregateFluent<TEntity> Fluent(AggregateOptions? options = null, bool ignoreGlobalFilters = false)
        => Context.Fluent<TEntity>(options, ignoreGlobalFilters);

    /// <inheritdoc />
    public IAggregateFluent<TEntity> FluentTextSearch(Search searchType, string searchTerm, bool caseSensitive = false,
        bool diacriticSensitive = false, string? language = null, AggregateOptions? options = null,
        bool ignoreGlobalFilters = false)
        => Context.FluentTextSearch<TEntity>(searchType, searchTerm, caseSensitive, diacriticSensitive, language, options, ignoreGlobalFilters);

    /// <inheritdoc />
    public IAggregateFluent<TEntity> GeoNear(Coordinates2D nearCoordinates, Expression<Func<TEntity, object>> distanceField, bool spherical = true,
        int? maxDistance = null, int? minDistance = null, int? limit = null, BsonDocument? query = null,
        int? distanceMultiplier = null, Expression<Func<TEntity, object>>? includeLocations = null, string? indexKey = null,
        AggregateOptions? options = null, bool ignoreGlobalFilters = false)
        => Context.GeoNear<TEntity>(nearCoordinates, distanceField, spherical, maxDistance, minDistance, limit, query, distanceMultiplier, includeLocations, indexKey, options, ignoreGlobalFilters);

    /// <inheritdoc />
    public PagedSearch<TEntity> PagedSearch()
        => Context.PagedSearch<TEntity>();

    /// <inheritdoc />
    public PagedSearch<TEntity, TProjection> PagedSearch<TProjection>()
        => Context.PagedSearch<TEntity,TProjection>();

    /// <inheritdoc />
    public async Task<IAsyncCursor<TResult>> PipelineCursorAsync<TResult>(Template<TEntity, TResult> template, AggregateOptions? options = null,
        CancellationToken cancellation = default, bool ignoreGlobalFilters = false)
        => await Context.PipelineCursorAsync(template, options, cancellation, ignoreGlobalFilters).ConfigureAwait(false);

    /// <inheritdoc />
    public async Task<List<TResult>> PipelineAsync<TResult>(Template<TEntity, TResult> template, AggregateOptions? options = null,
        CancellationToken cancellation = default, bool ignoreGlobalFilters = false)
        => await Context.PipelineAsync(template, options, cancellation, ignoreGlobalFilters).ConfigureAwait(false);

    /// <inheritdoc />
    public async Task<TResult> PipelineSingleAsync<TResult>(Template<TEntity, TResult> template, AggregateOptions? options = null,
        CancellationToken cancellation = default, bool ignoreGlobalFilters = false)
        => await Context.PipelineSingleAsync(template, options, cancellation, ignoreGlobalFilters).ConfigureAwait(false);

    /// <inheritdoc />
    public async Task<TResult> PipelineFirstAsync<TResult>(Template<TEntity, TResult> template, AggregateOptions? options = null,
        CancellationToken cancellation = default, bool ignoreGlobalFilters = false)
        => await Context.PipelineFirstAsync(template, options, cancellation, ignoreGlobalFilters).ConfigureAwait(false);

    /// <inheritdoc />
    public IMongoQueryable<T> Queryable<T>(AggregateOptions? options = null, bool ignoreGlobalFilters = false)
        where T : MongoEntity
        => Context.Queryable<T>();

    /// <inheritdoc />
    public virtual async Task<long> LongCountAsync(CancellationToken cancellationToken = default)
        => await MongoQueryable.LongCountAsync(cancellationToken).ConfigureAwait(false);
    
    /// <inheritdoc />
    public async Task<long> LongCountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        => await MongoQueryable.LongCountAsync(predicate, cancellationToken).ConfigureAwait(false);

    /// <inheritdoc />
    public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        => await MongoQueryable.AnyAsync(predicate, cancellationToken).ConfigureAwait(false);

    /// <inheritdoc />
    public async Task<IReadOnlyList<TEntity>> WhereAsync(Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
        => await MongoQueryable.Where(predicate).ToListAsync(cancellationToken);
    
    /// <inheritdoc />
    public async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
        => await MongoQueryable.FirstOrDefaultAsync(predicate, cancellationToken);
    
    /// <inheritdoc />
    public virtual async Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
        => await MongoQueryable.ToListAsync(cancellationToken).ConfigureAwait(false);
}
