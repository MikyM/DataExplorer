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
public class MongoReadOnlyRepository<TEntity,TId> : IMongoReadOnlyRepository<TEntity,TId> where TEntity : Entity<TId>, IEntity where TId : IComparable, IEquatable<TId>, IComparable<TId>
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
        
    /*/// <summary>
    /// Specification evaluator.
    /// </summary>
    protected readonly ISpecificationEvaluator SpecificationEvaluator;*/

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
    internal MongoReadOnlyRepository(IMongoDbContext context, /*ISpecificationEvaluator specificationEvaluator, */IMapper mapper)
    {
        Context = context ?? throw new ArgumentNullException(nameof(context));
        /*SpecificationEvaluator = specificationEvaluator;*/
        Mapper = mapper;
        MongoQueryable = context.Queryable<TEntity>();
    }

    /// <inheritdoc />
    public async Task<long> CountEstimatedAsync(CancellationToken cancellation = default)
        => await Context.CountEstimatedAsync<TEntity>(cancellation).ConfigureAwait(false);

    public async Task<long> CountAsync(Expression<Func<TEntity, bool>> expression,
        CancellationToken cancellation = default, CountOptions? options = null,
        bool ignoreGlobalFilters = false)
        => await Context.CountAsync(expression, cancellation, options, ignoreGlobalFilters).ConfigureAwait(false);

    public async Task<long> CountAsync(CancellationToken cancellation = default)
        => await Context.CountAsync<TEntity>(cancellation).ConfigureAwait(false);

    public async Task<long> CountAsync(FilterDefinition<TEntity> filter, CancellationToken cancellation = default, CountOptions? options = null,
        bool ignoreGlobalFilters = false)
        => await Context.CountAsync(filter, cancellation, options, ignoreGlobalFilters).ConfigureAwait(false);

    public async Task<long> CountAsync(Func<FilterDefinitionBuilder<TEntity>, FilterDefinition<TEntity>> filter, CancellationToken cancellation = default, CountOptions? options = null,
        bool ignoreGlobalFilters = false)
        => await Context.CountAsync(filter, cancellation, options, ignoreGlobalFilters).ConfigureAwait(false);

    public Distinct<TEntity, TProperty> Distinct<TProperty>()
        => Context.Distinct<TEntity,TProperty>();

    public Find<TEntity> Find()
        => Context.Find<TEntity>();

    public Find<TEntity, TProjection> Find<TProjection>()
        => Context.Find<TEntity,TProjection>();

    public IAggregateFluent<TEntity> Fluent(AggregateOptions? options = null, bool ignoreGlobalFilters = false)
        => Context.Fluent<TEntity>(options, ignoreGlobalFilters);

    public IAggregateFluent<TEntity> FluentTextSearch(Search searchType, string searchTerm, bool caseSensitive = false,
        bool diacriticSensitive = false, string? language = null, AggregateOptions? options = null,
        bool ignoreGlobalFilters = false)
        => Context.FluentTextSearch<TEntity>(searchType, searchTerm, caseSensitive, diacriticSensitive, language, options, ignoreGlobalFilters);

    public IAggregateFluent<TEntity> GeoNear(Coordinates2D nearCoordinates, Expression<Func<TEntity, object>> distanceField, bool spherical = true,
        int? maxDistance = null, int? minDistance = null, int? limit = null, BsonDocument? query = null,
        int? distanceMultiplier = null, Expression<Func<TEntity, object>>? includeLocations = null, string? indexKey = null,
        AggregateOptions? options = null, bool ignoreGlobalFilters = false)
        => Context.GeoNear<TEntity>(nearCoordinates, distanceField, spherical, maxDistance, minDistance, limit, query, distanceMultiplier, includeLocations, indexKey, options, ignoreGlobalFilters);

    public PagedSearch<TEntity> PagedSearch()
        => Context.PagedSearch<TEntity>();

    public PagedSearch<TEntity, TProjection> PagedSearch<TProjection>()
        => Context.PagedSearch<TEntity,TProjection>();

    public async Task<IAsyncCursor<TResult>> PipelineCursorAsync<TResult>(Template<TEntity, TResult> template, AggregateOptions? options = null,
        CancellationToken cancellation = default, bool ignoreGlobalFilters = false)
        => await Context.PipelineCursorAsync(template, options, cancellation, ignoreGlobalFilters).ConfigureAwait(false);

    public async Task<List<TResult>> PipelineAsync<TResult>(Template<TEntity, TResult> template, AggregateOptions? options = null,
        CancellationToken cancellation = default, bool ignoreGlobalFilters = false)
        => await Context.PipelineAsync(template, options, cancellation, ignoreGlobalFilters).ConfigureAwait(false);

    public async Task<TResult> PipelineSingleAsync<TResult>(Template<TEntity, TResult> template, AggregateOptions? options = null,
        CancellationToken cancellation = default, bool ignoreGlobalFilters = false)
        => await Context.PipelineSingleAsync(template, options, cancellation, ignoreGlobalFilters).ConfigureAwait(false);

    public async Task<TResult> PipelineFirstAsync<TResult>(Template<TEntity, TResult> template, AggregateOptions? options = null,
        CancellationToken cancellation = default, bool ignoreGlobalFilters = false)
        => await Context.PipelineFirstAsync(template, options, cancellation, ignoreGlobalFilters).ConfigureAwait(false);

    public IMongoQueryable<T> Queryable<T>(AggregateOptions? options = null, bool ignoreGlobalFilters = false)
        where T : Entity<TId>, IEntity
        => Context.Queryable<T>();

    public Replace<TEntity> Replace()
        => Context.Replace<TEntity>();

    /*/// <inheritdoc />
    public virtual async Task<TEntity?> GetSingleBySpecAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
        => await ApplySpecification(specification)
            .FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);

    /// <inheritdoc />
    public virtual async Task<TProjectTo?> GetSingleBySpecAsync<TProjectTo>(
        ISpecification<TEntity, TProjectTo> specification, CancellationToken cancellationToken = default)
        where TProjectTo : class
    {
        specification.MapperConfigurationProvider ??= Mapper.ConfigurationProvider;
    
        return await ApplySpecification(specification)
            .FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public virtual async Task<IReadOnlyList<TProjectTo>> GetBySpecAsync<TProjectTo>(
        ISpecification<TEntity, TProjectTo> specification, CancellationToken cancellationToken = default) where TProjectTo : class
    {
        specification.MapperConfigurationProvider ??= Mapper.ConfigurationProvider;
    
        var result = await ApplySpecification(specification).ToListAsync(cancellationToken).ConfigureAwait(false);
        return specification.PostProcessingAction is null
            ? result
            : specification.PostProcessingAction(result).ToList();
    }

    /// <inheritdoc />
    public virtual async Task<IReadOnlyList<TEntity>> GetBySpecAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
    {
        var result = await ApplySpecification(specification)
            .ToListAsync(cancellationToken).ConfigureAwait(false);

        return specification.PostProcessingAction is null
            ? result
            : specification.PostProcessingAction(result).ToList();
    }

    /// <inheritdoc />
    public virtual async Task<long> LongCountAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
        => await ApplySpecification(specification)
            .LongCountAsync(cancellationToken).ConfigureAwait(false);*/
    
    /// <inheritdoc />
    public virtual async Task<long> LongCountAsync(CancellationToken cancellationToken = default)
        => await MongoQueryable.LongCountAsync(cancellationToken).ConfigureAwait(false);

    /// <inheritdoc />
    public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        => await MongoQueryable.AnyAsync(predicate, cancellationToken).ConfigureAwait(false);

    /*/// <inheritdoc />
    public async Task<bool> AnyAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
        => await ApplySpecification(specification).AnyAsync(cancellationToken).ConfigureAwait(false);*/

    /// <inheritdoc />
    public virtual async Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
        => await MongoQueryable.ToListAsync(cancellationToken).ConfigureAwait(false);

    /*
    /// <inheritdoc />
    public virtual async Task<IReadOnlyList<TProjectTo>> GetAllAsync<TProjectTo>(CancellationToken cancellationToken = default) where TProjectTo : class
        => await ApplySpecification(new Specification<TEntity, TProjectTo>(Mapper.ConfigurationProvider))
            .ToListAsync(cancellationToken).ConfigureAwait(false);
            */

    /*/// <summary>
    ///     Filters the entities  of <typeparamref name="TEntity" />, to those that match the encapsulated query logic of the
    ///     <paramref name="specification" />.
    /// </summary>
    /// <param name="specification">The encapsulated query logic.</param>
    /// <param name="evaluateCriteriaOnly">Whether to only evaluate criteria.</param>
    /// <returns>The filtered entities as an <see cref="IQueryable{T}" />.</returns>
    protected virtual IQueryable<TEntity> ApplySpecification(ISpecification<TEntity> specification,
        bool evaluateCriteriaOnly = false)
        => SpecificationEvaluator.GetQuery(Set.AsQueryable(), specification,
            evaluateCriteriaOnly);

    /// <summary>
    ///     Filters all entities of <typeparamref name="TEntity" />, that matches the encapsulated query logic of the
    ///     <paramref name="specification" />, from the database.
    ///     <para>
    ///         Projects each entity into a new form, being <typeparamref name="TResult" />.
    ///     </para>
    /// </summary>
    /// <typeparam name="TResult">The type of the value returned by the projection.</typeparam>
    /// <param name="specification">The encapsulated query logic.</param>
    /// <returns>The filtered projected entities as an <see cref="IQueryable{T}" />.</returns>
    protected virtual IQueryable<TResult> ApplySpecification<TResult>(
        ISpecification<TEntity, TResult> specification) where TResult : class
        => SpecificationEvaluator.GetQuery(Set.AsQueryable(), specification);*/
}


/// <summary>
/// Read-only repository.
/// </summary>
/// <inheritdoc cref="IMongoReadOnlyRepository{TEntity}"/>
[PublicAPI]
public class MongoReadOnlyRepository<TEntity> : MongoReadOnlyRepository<TEntity, long>, IMongoReadOnlyRepository<TEntity> where TEntity : Entity<long>, IEntity
{
    internal MongoReadOnlyRepository(IMongoDbContext context,/* ISpecificationEvaluator specificationEvaluator, */IMapper mapper) : base(context, /*specificationEvaluator,*/ mapper)
    {
    }
}
