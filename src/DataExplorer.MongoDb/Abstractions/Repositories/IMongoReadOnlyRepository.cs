using System.Linq.Expressions;
using DataExplorer.Abstractions.Repositories;
using DataExplorer.MongoDb.Abstractions.DataContexts;

namespace DataExplorer.MongoDb.Abstractions.Repositories;

/// <summary>
/// Read-only repository.
/// </summary>
/// <typeparam name="TEntity">Entity that derives from <see cref="MongoEntity"/>.</typeparam>
[PublicAPI]
public interface IMongoReadOnlyRepository<TEntity> : IRepositoryBase where TEntity : MongoEntity
{
    /// <summary>
    /// Current <see cref="IMongoDbContext"/>.
    /// </summary>
    new IMongoDbContext Context { get; }
        
    /// <summary>
    /// Queryable.
    /// </summary>
    IMongoQueryable<TEntity> MongoQueryable { get; }

    /// <summary>
    /// Gets a fast estimation of how many documents are in the collection using metadata.
    /// <para>HINT: The estimation may not be exactly accurate.</para>
    /// </summary>
    /// <param name="cancellation">An optional cancellation token</param>
    Task<long> CountEstimatedAsync(CancellationToken cancellation = default);

    /// <summary>
    /// Gets an accurate count of how many entities are matched for a given expression/filter
    /// </summary>
    /// <param name="expression">A lambda expression for getting the count for a subset of the data</param>
    /// <param name="cancellation">An optional cancellation token</param>
    /// <param name="options">An optional CountOptions object</param>
    /// <param name="ignoreGlobalFilters">Set to true if you'd like to ignore any global filters for this operation</param>
    Task<long> CountAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellation = default,
        CountOptions? options = null, bool ignoreGlobalFilters = false);

    /// <summary>
    /// Gets an accurate count of how many total entities are in the collection for a given entity type
    /// </summary>
    /// <param name="cancellation">An optional cancellation token</param>
    Task<long> CountAsync(CancellationToken cancellation = default);

    /// <summary>
    /// Gets an accurate count of how many total entities are in the collection for a given entity type
    /// </summary>
    /// <param name="filter">A filter definition</param>
    /// <param name="cancellation">An optional cancellation token</param>
    /// <param name="options">An optional CountOptions object</param>
    /// <param name="ignoreGlobalFilters">Set to true if you'd like to ignore any global filters for this operation</param>
    Task<long> CountAsync(FilterDefinition<TEntity> filter, CancellationToken cancellation = default,
        CountOptions? options = null, bool ignoreGlobalFilters = false);

    /// <summary>
    /// Gets an accurate count of how many total entities are in the collection for a given entity type
    /// </summary>
    /// <param name="filter">f => f.Eq(x => x.Prop, Value) &amp; f.Gt(x => x.Prop, Value)</param>
    /// <param name="cancellation">An optional cancellation token</param>
    /// <param name="options">An optional CountOptions object</param>
    /// <param name="ignoreGlobalFilters">Set to true if you'd like to ignore any global filters for this operation</param>
    Task<long> CountAsync(Func<FilterDefinitionBuilder<TEntity>, FilterDefinition<TEntity>> filter,
        CancellationToken cancellation = default, CountOptions? options = null, bool ignoreGlobalFilters = false);
    
    /// <summary>
    /// Represents a MongoDB Distinct command where you can get back distinct values for a given property of a given Entity
    /// </summary>
    /// <typeparam name="TProperty">The type of the property of the entity you'd like to get unique values for</typeparam>
    Distinct<TEntity,TProperty> Distinct<TProperty>();

    /// <summary>
    /// Starts a find command for the given entity type
    /// </summary>
    Find<TEntity> Find();

    /// <summary>
    /// Starts a find command with projection support for the given entity type
    /// </summary>
    /// <typeparam name="TProjection">The type of the end result</typeparam>
    Find<TEntity, TProjection> Find<TProjection>();

    /// <summary>
    /// Exposes the MongoDB collection for the given entity type as IAggregateFluent in order to facilitate Fluent queries
    /// </summary>
    /// <param name="options">The options for the aggregation. This is not required.</param>
    /// <param name="ignoreGlobalFilters">Set to true if you'd like to ignore any global filters for this operation</param>
    IAggregateFluent<TEntity> Fluent(AggregateOptions? options = null, bool ignoreGlobalFilters = false);

    /// <summary>
    /// Start a fluent aggregation pipeline with a $text stage with the supplied parameters
    /// <para>TIP: Make sure to define a text index with DB.Index&lt;T&gt;() before searching</para>
    /// </summary>
    /// <param name="searchType">The type of text matching to do</param>
    /// <param name="searchTerm">The search term</param>
    /// <param name="caseSensitive">Case sensitivity of the search (optional)</param>
    /// <param name="diacriticSensitive">Diacritic sensitivity of the search (optional)</param>
    /// <param name="language">The language for the search (optional)</param>
    /// <param name="options">Options for finding documents (not required)</param>
    /// <param name="ignoreGlobalFilters">Set to true if you'd like to ignore any global filters for this operation</param>
    IAggregateFluent<TEntity> FluentTextSearch(Search searchType, string searchTerm, bool caseSensitive = false,
        bool diacriticSensitive = false, string? language = null, AggregateOptions? options = null,
        bool ignoreGlobalFilters = false);

    /// <summary>
    /// Start a fluent aggregation pipeline with a $GeoNear stage with the supplied parameters
    /// </summary>
    /// <param name="nearCoordinates">The coordinates from which to find documents from</param>
    /// <param name="distanceField">x => x.Distance</param>
    /// <param name="spherical">Calculate distances using spherical geometry or not</param>
    /// <param name="maxDistance">The maximum distance in meters from the center point that the documents can be</param>
    /// <param name="minDistance">The minimum distance in meters from the center point that the documents can be</param>
    /// <param name="limit">The maximum number of documents to return</param>
    /// <param name="query">Limits the results to the documents that match the query</param>
    /// <param name="distanceMultiplier">The factor to multiply all distances returned by the query</param>
    /// <param name="includeLocations">Specify the output field to store the point used to calculate the distance</param>
    /// <param name="indexKey"></param>
    /// <param name="options">The options for the aggregation. This is not required.</param>
    /// <param name="ignoreGlobalFilters">Set to true if you'd like to ignore any global filters for this operation</param>
    IAggregateFluent<TEntity> GeoNear(Coordinates2D nearCoordinates, Expression<Func<TEntity, object>> distanceField,
        bool spherical = true, int? maxDistance = null, int? minDistance = null, int? limit = null,
        BsonDocument? query = null, int? distanceMultiplier = null, Expression<Func<TEntity, object>>? includeLocations = null,
        string? indexKey = null, AggregateOptions? options = null, bool ignoreGlobalFilters = false);
    
    /// <summary>
    /// Represents an aggregation query that retrieves results with easy paging support.
    /// </summary>
    PagedSearch<TEntity> PagedSearch();

    /// <summary>
    /// Represents an aggregation query that retrieves results with easy paging support.
    /// </summary>
    /// <typeparam name="TProjection">The type you'd like to project the results to.</typeparam>
    PagedSearch<TEntity, TProjection> PagedSearch<TProjection>();

    /// <summary>
    /// Executes an aggregation pipeline by supplying a 'Template' object.
    /// Gets a cursor back as the result.
    /// </summary>
    /// <typeparam name="TResult">The type of the resulting objects</typeparam>
    /// <param name="template">A 'Template' object with tags replaced</param>
    /// <param name="options">The options for the aggregation. This is not required.</param>
    /// <param name="cancellation">An optional cancellation token</param>
    /// <param name="ignoreGlobalFilters">Set to true if you'd like to ignore any global filters for this operation</param>
    Task<IAsyncCursor<TResult>> PipelineCursorAsync<TResult>(Template<TEntity, TResult> template,
        AggregateOptions? options = null, CancellationToken cancellation = default, bool ignoreGlobalFilters = false);

    /// <summary>
    /// Executes an aggregation pipeline by supplying a 'Template' object.
    /// Gets a list back as the result.
    /// </summary>
    /// <typeparam name="TResult">The type of the resulting objects</typeparam>
    /// <param name="template">A 'Template' object with tags replaced</param>
    /// <param name="options">The options for the aggregation. This is not required.</param>
    /// <param name="cancellation">An optional cancellation token</param>
    /// <param name="ignoreGlobalFilters">Set to true if you'd like to ignore any global filters for this operation</param>
    Task<List<TResult>> PipelineAsync<TResult>(Template<TEntity, TResult> template, AggregateOptions? options = null,
        CancellationToken cancellation = default, bool ignoreGlobalFilters = false);

    /// <summary>
    /// Executes an aggregation pipeline by supplying a 'Template' object.
    /// Gets a single or default value as the result.
    /// </summary>
    /// <typeparam name="TResult">The type of the resulting objects</typeparam>
    /// <param name="template">A 'Template' object with tags replaced</param>
    /// <param name="options">The options for the aggregation. This is not required.</param>
    /// <param name="cancellation">An optional cancellation token</param>
    /// <param name="ignoreGlobalFilters">Set to true if you'd like to ignore any global filters for this operation</param>
    Task<TResult> PipelineSingleAsync<TResult>(Template<TEntity, TResult> template, AggregateOptions? options = null,
        CancellationToken cancellation = default, bool ignoreGlobalFilters = false);

    /// <summary>
    /// Executes an aggregation pipeline by supplying a 'Template' object.
    /// Gets the first or default value as the result.
    /// </summary>
    /// <typeparam name="TResult">The type of the resulting objects</typeparam>
    /// <param name="template">A 'Template' object with tags replaced</param>
    /// <param name="options">The options for the aggregation. This is not required.</param>
    /// <param name="cancellation">An optional cancellation token</param>
    /// <param name="ignoreGlobalFilters">Set to true if you'd like to ignore any global filters for this operation</param>
    Task<TResult> PipelineFirstAsync<TResult>(Template<TEntity, TResult> template, AggregateOptions? options = null,
        CancellationToken cancellation = default, bool ignoreGlobalFilters = false);

    /// <summary>
    /// Exposes the MongoDB collection for the given entity type as IQueryable in order to facilitate LINQ queries
    /// </summary>
    /// <param name="options">The aggregate options</param>
    /// <param name="ignoreGlobalFilters">Set to true if you'd like to ignore any global filters for this operation</param>
    IMongoQueryable<T> Queryable<T>(AggregateOptions? options = null, bool ignoreGlobalFilters = false) where T : MongoEntity;

    /// <summary>
    /// Counts the entities.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Number of entities.</returns>
    Task<long> LongCountAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Counts the entities that satisfy the given predicate.
    /// </summary>
    /// <param name="predicate">The predicate to filter the entities with.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Number of entities.</returns>
    Task<long> LongCountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously determines whether any elements satisfy the given condition.
    /// </summary>
    /// <param name="predicate">Predicate for the query.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if any elements in the source sequence satisfy the condition, otherwise false.</returns>
    Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all entities that satisfy the given predicate.
    /// </summary>
    /// <param name="predicate">The predicate to filter the entities with.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><see cref="IReadOnlyList{T}"/> with filtered entities.</returns>
    Task<IReadOnlyList<TEntity>> WhereAsync(Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the first entity that satisfies the given predicate.
    /// </summary>
    /// <param name="predicate">The predicate to filter the entities with.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>If found an instance of the found entity, otherwise null.</returns>
    Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all entities.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><see cref="IReadOnlyList{T}"/> with all entities.</returns>
    Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
}
