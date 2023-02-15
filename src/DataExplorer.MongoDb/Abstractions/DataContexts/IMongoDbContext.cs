using System.Linq.Expressions;
using DataExplorer.Abstractions.DataContexts;

namespace DataExplorer.MongoDb.Abstractions.DataContexts;

/// <summary>
/// Represents a MongoDB database context.
/// </summary>
[PublicAPI]
public interface IMongoDbContext : IDataContextBase
{
    /// <summary>
    /// The options.
    /// </summary>
    MongoDbContextOptions Options { get; }
    
    /// <summary>
    /// The value of this property will be automatically set on entities when saving/updating if the entity has a ModifiedBy property.
    /// </summary>
    ModifiedBy? ModifiedBy { get; set; }
    
    /// <summary>
    /// Returns the session object used for transactions.
    /// </summary>
    IClientSessionHandle Session { get; }

    /// <summary>
    /// The MongoDB database.
    /// </summary>
    IMongoDatabase MongoDatabase { get; }

    /// <summary>
    /// Creates a collection for an Entity type explicitly using the given options
    /// </summary>
    /// <typeparam name="T">The type of entity that will be stored in the created collection</typeparam>
    /// <param name="options">The options to use for collection creation</param>
    /// <param name="cancellation">An optional cancellation token</param>
    Task CreateCollectionAsync<T>(Action<CreateCollectionOptions<T>> options, CancellationToken cancellation = default)
        where T : IEntity;

    /// <summary>
    /// Deletes the collection of a given entity type as well as the join collections for that entity.
    /// <para>TIP: When deleting a collection, all relationships associated with that entity type is also deleted.</para>
    /// </summary>
    /// <typeparam name="T">The entity type to drop the collection of</typeparam>
    Task DropCollectionAsync<T>() where T : IEntity;

    /// <summary>
    /// Gets a fast estimation of how many documents are in the collection using metadata.
    /// <para>HINT: The estimation may not be exactly accurate.</para>
    /// </summary>
    /// <typeparam name="T">The entity type to get the count for</typeparam>
    /// <param name="cancellation">An optional cancellation token</param>
    Task<long> CountEstimatedAsync<T>(CancellationToken cancellation = default) where T : IEntity;

    /// <summary>
    /// Gets an accurate count of how many entities are matched for a given expression/filter
    /// </summary>
    /// <typeparam name="T">The entity type to get the count for</typeparam>
    /// <param name="expression">A lambda expression for getting the count for a subset of the data</param>
    /// <param name="cancellation">An optional cancellation token</param>
    /// <param name="options">An optional CountOptions object</param>
    /// <param name="ignoreGlobalFilters">Set to true if you'd like to ignore any global filters for this operation</param>
    Task<long> CountAsync<T>(Expression<Func<T, bool>> expression, CancellationToken cancellation = default,
        CountOptions? options = null, bool ignoreGlobalFilters = false) where T : IEntity;

    /// <summary>
    /// Gets an accurate count of how many total entities are in the collection for a given entity type
    /// </summary>
    /// <typeparam name="T">The entity type to get the count for</typeparam>
    /// <param name="cancellation">An optional cancellation token</param>
    Task<long> CountAsync<T>(CancellationToken cancellation = default) where T : IEntity;

    /// <summary>
    /// Gets an accurate count of how many total entities are in the collection for a given entity type
    /// </summary>
    /// <typeparam name="T">The entity type to get the count for</typeparam>
    /// <param name="filter">A filter definition</param>
    /// <param name="cancellation">An optional cancellation token</param>
    /// <param name="options">An optional CountOptions object</param>
    /// <param name="ignoreGlobalFilters">Set to true if you'd like to ignore any global filters for this operation</param>
    Task<long> CountAsync<T>(FilterDefinition<T> filter, CancellationToken cancellation = default,
        CountOptions? options = null, bool ignoreGlobalFilters = false) where T : IEntity;

    /// <summary>
    /// Gets an accurate count of how many total entities are in the collection for a given entity type
    /// </summary>
    /// <typeparam name="T">The entity type to get the count for</typeparam>
    /// <param name="filter">f => f.Eq(x => x.Prop, Value) &amp; f.Gt(x => x.Prop, Value)</param>
    /// <param name="cancellation">An optional cancellation token</param>
    /// <param name="options">An optional CountOptions object</param>
    /// <param name="ignoreGlobalFilters">Set to true if you'd like to ignore any global filters for this operation</param>
    Task<long> CountAsync<T>(Func<FilterDefinitionBuilder<T>, FilterDefinition<T>> filter,
        CancellationToken cancellation = default, CountOptions? options = null, bool ignoreGlobalFilters = false)
        where T : IEntity;

    /// <summary>
    /// Starts a transaction and returns a session object.
    /// <para>WARNING: Only one transaction is allowed per DBContext instance. 
    /// Call Session.Dispose() and assign a null to it before calling this method a second time. 
    /// Trying to start a second transaction for this DBContext instance will throw an exception.</para>
    /// </summary>
    /// <param name="database">The name of the database to use for this transaction. default db is used if not specified</param>
    /// <param name="options">Client session options for this transaction</param>
    IClientSessionHandle Transaction(string? database = default, ClientSessionOptions? options = null);

    /// <summary>
    /// Starts a transaction and returns a session object for a given entity type.
    /// <para>WARNING: Only one transaction is allowed per DBContext instance. 
    /// Call Session.Dispose() and assign a null to it before calling this method a second time. 
    /// Trying to start a second transaction for this DBContext instance will throw an exception.</para>
    /// </summary>
    /// <typeparam name="T">The entity type to determine the database from for the transaction</typeparam>
    /// <param name="options">Client session options (not required)</param>
    IClientSessionHandle Transaction<T>(ClientSessionOptions? options = null) where T : IEntity;

    /// <summary>
    /// Commits a transaction to MongoDB
    /// </summary>
    /// <param name="cancellation">An optional cancellation token</param>
    Task CommitAsync(CancellationToken cancellation = default);
    
    /// <summary>
    /// Aborts and rolls back a transaction
    /// </summary>
    /// <param name="cancellation">An optional cancellation token</param>
    Task AbortAsync(CancellationToken cancellation = default);

    /// <summary>
    /// Deletes a single entity from MongoDB
    /// <para>HINT: If this entity is referenced by one-to-many/many-to-many relationships, those references are also deleted.</para>
    /// </summary>
    /// <typeparam name="T">The type of entity</typeparam>
    /// <param name="ID">The Id of the entity to delete</param>
    /// <param name="cancellation">An optional cancellation token</param>
    /// <param name="ignoreGlobalFilters">Set to true if you'd like to ignore any global filters for this operation</param>
    Task<DeleteResult> DeleteAsync<T>(string ID, CancellationToken cancellation = default,
        bool ignoreGlobalFilters = false) where T : IEntity;

    /// <summary>
    /// Deletes matching entities from MongoDB
    /// <para>HINT: If these entities are referenced by one-to-many/many-to-many relationships, those references are also deleted.</para>
    /// <para>TIP: Try to keep the number of entities to delete under 100 in a single call</para>
    /// </summary>
    /// <typeparam name="T">The type of entity</typeparam>
    /// <param name="IDs">An IEnumerable of entity IDs</param>
    /// <param name="cancellation">An optional cancellation token</param>
    /// <param name="ignoreGlobalFilters">Set to true if you'd like to ignore any global filters for this operation</param>
    Task<DeleteResult> DeleteAsync<T>(IEnumerable<string> IDs, CancellationToken cancellation = default,
        bool ignoreGlobalFilters = false) where T : IEntity;

    /// <summary>
    /// Deletes matching entities from MongoDB
    /// <para>HINT: If these entities are referenced by one-to-many/many-to-many relationships, those references are also deleted.</para>
    /// <para>TIP: Try to keep the number of entities to delete under 100 in a single call</para>
    /// </summary>
    /// <typeparam name="T">The type of entity</typeparam>
    /// <param name="expression">A lambda expression for matching entities to delete.</param>
    /// <param name="cancellation">An optional cancellation token</param>
    /// <param name="collation">An optional collation object</param>
    /// <param name="ignoreGlobalFilters">Set to true if you'd like to ignore any global filters for this operation</param>
    Task<DeleteResult> DeleteAsync<T>(Expression<Func<T, bool>> expression,
        CancellationToken cancellation = default, Collation? collation = null, bool ignoreGlobalFilters = false)
        where T : IEntity;

    /// <summary>
    /// Deletes matching entities with a filter expression
    /// <para>HINT: If the expression matches more than 100,000 entities, they will be deleted in batches of 100k.</para>
    /// <para>HINT: If these entities are referenced by one-to-many/many-to-many relationships, those references are also deleted.</para>
    /// </summary>
    /// <typeparam name="T">Any class that implements IEntity</typeparam>
    /// <param name="filter">f => f.Eq(x => x.Prop, Value) &amp; f.Gt(x => x.Prop, Value)</param>
    /// <param name="cancellation">An optional cancellation token</param>
    /// <param name="collation">An optional collation object</param>
    /// <param name="ignoreGlobalFilters">Set to true if you'd like to ignore any global filters for this operation</param>
    Task<DeleteResult> DeleteAsync<T>(Func<FilterDefinitionBuilder<T>, FilterDefinition<T>> filter,
        CancellationToken cancellation = default, Collation? collation = null, bool ignoreGlobalFilters = false)
        where T : IEntity;

    /// <summary>
    /// Deletes matching entities with a filter definition
    /// <para>HINT: If the expression matches more than 100,000 entities, they will be deleted in batches of 100k.</para>
    /// <para>HINT: If these entities are referenced by one-to-many/many-to-many relationships, those references are also deleted.</para>
    /// </summary>
    /// <typeparam name="T">Any class that implements IEntity</typeparam>
    /// <param name="filter">A filter definition for matching entities to delete.</param>
    /// <param name="cancellation">An optional cancellation token</param>
    /// <param name="collation">An optional collation object</param>
    /// <param name="ignoreGlobalFilters">Set to true if you'd like to ignore any global filters for this operation</param>
    Task<DeleteResult> DeleteAsync<T>(FilterDefinition<T> filter, CancellationToken cancellation = default,
        Collation? collation = null, bool ignoreGlobalFilters = false) where T : IEntity;

    /// <summary>
    /// Represents a MongoDB Distinct command where you can get back distinct values for a given property of a given Entity
    /// </summary>
    /// <typeparam name="T">Any Entity that implements IEntity interface</typeparam>
    /// <typeparam name="TProperty">The type of the property of the entity you'd like to get unique values for</typeparam>
    Distinct<T, TProperty> Distinct<T, TProperty>() where T : IEntity;

    /// <summary>
    /// Starts a find command for the given entity type
    /// </summary>
    /// <typeparam name="T">The type of entity</typeparam>
    Find<T> Find<T>() where T : IEntity;

    /// <summary>
    /// Starts a find command with projection support for the given entity type
    /// </summary>
    /// <typeparam name="T">The type of entity</typeparam>
    /// <typeparam name="TProjection">The type of the end result</typeparam>
    Find<T, TProjection> Find<T, TProjection>() where T : IEntity;

    /// <summary>
    /// Exposes the MongoDB collection for the given entity type as IAggregateFluent in order to facilitate Fluent queries
    /// </summary>
    /// <typeparam name="T">The type of entity</typeparam>
    /// <param name="options">The options for the aggregation. This is not required.</param>
    /// <param name="ignoreGlobalFilters">Set to true if you'd like to ignore any global filters for this operation</param>
    IAggregateFluent<T> Fluent<T>(AggregateOptions? options = null, bool ignoreGlobalFilters = false) where T : IEntity;

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
    IAggregateFluent<T> FluentTextSearch<T>(Search searchType, string searchTerm, bool caseSensitive = false,
        bool diacriticSensitive = false, string? language = null, AggregateOptions? options = null,
        bool ignoreGlobalFilters = false) where T : IEntity;

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
    /// <typeparam name="T">The type of entity</typeparam>
    /// <param name="ignoreGlobalFilters">Set to true if you'd like to ignore any global filters for this operation</param>
    IAggregateFluent<T> GeoNear<T>(Coordinates2D nearCoordinates, Expression<Func<T, object>> distanceField,
        bool spherical = true, int? maxDistance = null, int? minDistance = null, int? limit = null,
        BsonDocument? query = null, int? distanceMultiplier = null, Expression<Func<T, object>>? includeLocations = null,
        string? indexKey = null, AggregateOptions? options = null, bool ignoreGlobalFilters = false)
        where T : IEntity;

    /// <summary>
    /// Saves a complete entity replacing an existing entity or creating a new one if it does not exist. 
    /// If ID value is null, a new entity is created. If ID has a value, then existing entity is replaced.
    /// </summary>
    /// <typeparam name="T">The type of entity</typeparam>
    /// <param name="entity">The instance to persist</param>
    /// <param name="cancellation">And optional cancellation token</param>
    Task InsertAsync<T>(T entity, CancellationToken cancellation = default) where T : IEntity;

    /// <summary>
    /// Saves a batch of complete entities replacing an existing entities or creating a new ones if they do not exist. 
    /// If ID value is null, a new entity is created. If ID has a value, then existing entity is replaced.
    /// </summary>
    /// <typeparam name="T">The type of entity</typeparam>
    /// <param name="entities">The entities to persist</param>
    /// <param name="cancellation">And optional cancellation token</param>
    Task<BulkWriteResult<T>> InsertAsync<T>(IEnumerable<T> entities, CancellationToken cancellation = default)
        where T : IEntity;
    
    /// <summary>
    /// Represents an aggregation query that retrieves results with easy paging support.
    /// </summary>
    /// <typeparam name="T">Any class that implements IEntity</typeparam>
    PagedSearch<T> PagedSearch<T>() where T : IEntity;

    /// <summary>
    /// Represents an aggregation query that retrieves results with easy paging support.
    /// </summary>
    /// <typeparam name="T">Any class that implements IEntity</typeparam>
    /// <typeparam name="TProjection">The type you'd like to project the results to.</typeparam>
    PagedSearch<T, TProjection> PagedSearch<T, TProjection>() where T : IEntity;

    /// <summary>
    /// Executes an aggregation pipeline by supplying a 'Template' object.
    /// Gets a cursor back as the result.
    /// </summary>
    /// <typeparam name="T">The type of entity</typeparam>
    /// <typeparam name="TResult">The type of the resulting objects</typeparam>
    /// <param name="template">A 'Template' object with tags replaced</param>
    /// <param name="options">The options for the aggregation. This is not required.</param>
    /// <param name="cancellation">An optional cancellation token</param>
    /// <param name="ignoreGlobalFilters">Set to true if you'd like to ignore any global filters for this operation</param>
    Task<IAsyncCursor<TResult>> PipelineCursorAsync<T, TResult>(Template<T, TResult> template,
        AggregateOptions? options = null, CancellationToken cancellation = default, bool ignoreGlobalFilters = false)
        where T : IEntity;

    /// <summary>
    /// Executes an aggregation pipeline by supplying a 'Template' object.
    /// Gets a list back as the result.
    /// </summary>
    /// <typeparam name="T">The type of entity</typeparam>
    /// <typeparam name="TResult">The type of the resulting objects</typeparam>
    /// <param name="template">A 'Template' object with tags replaced</param>
    /// <param name="options">The options for the aggregation. This is not required.</param>
    /// <param name="cancellation">An optional cancellation token</param>
    /// <param name="ignoreGlobalFilters">Set to true if you'd like to ignore any global filters for this operation</param>
    Task<List<TResult>> PipelineAsync<T, TResult>(Template<T, TResult> template, AggregateOptions? options = null,
        CancellationToken cancellation = default, bool ignoreGlobalFilters = false) where T : IEntity;

    /// <summary>
    /// Executes an aggregation pipeline by supplying a 'Template' object.
    /// Gets a single or default value as the result.
    /// </summary>
    /// <typeparam name="T">The type of entity</typeparam>
    /// <typeparam name="TResult">The type of the resulting objects</typeparam>
    /// <param name="template">A 'Template' object with tags replaced</param>
    /// <param name="options">The options for the aggregation. This is not required.</param>
    /// <param name="cancellation">An optional cancellation token</param>
    /// <param name="ignoreGlobalFilters">Set to true if you'd like to ignore any global filters for this operation</param>
    Task<TResult> PipelineSingleAsync<T, TResult>(Template<T, TResult> template, AggregateOptions? options = null,
        CancellationToken cancellation = default, bool ignoreGlobalFilters = false) where T : IEntity;

    /// <summary>
    /// Executes an aggregation pipeline by supplying a 'Template' object.
    /// Gets the first or default value as the result.
    /// </summary>
    /// <typeparam name="T">The type of entity</typeparam>
    /// <typeparam name="TResult">The type of the resulting objects</typeparam>
    /// <param name="template">A 'Template' object with tags replaced</param>
    /// <param name="options">The options for the aggregation. This is not required.</param>
    /// <param name="cancellation">An optional cancellation token</param>
    /// <param name="ignoreGlobalFilters">Set to true if you'd like to ignore any global filters for this operation</param>
    Task<TResult> PipelineFirstAsync<T, TResult>(Template<T, TResult> template, AggregateOptions? options = null,
        CancellationToken cancellation = default, bool ignoreGlobalFilters = false) where T : IEntity;

    /// <summary>
    /// Exposes the MongoDB collection for the given entity type as IQueryable in order to facilitate LINQ queries
    /// </summary>
    /// <param name="options">The aggregate options</param>
    /// <typeparam name="T">The type of entity</typeparam>
    /// <param name="ignoreGlobalFilters">Set to true if you'd like to ignore any global filters for this operation</param>
    IMongoQueryable<T> Queryable<T>(AggregateOptions? options = null, bool ignoreGlobalFilters = false)
        where T : IEntity;

    /// <summary>
    /// Starts a replace command for the given entity type
    /// <para>TIP: Only the first matched entity will be replaced</para>
    /// </summary>
    /// <typeparam name="T">The type of entity</typeparam>
    Replace<T> Replace<T>() where T : IEntity;
    
    /// <summary>
    /// Saves a complete entity replacing an existing entity or creating a new one if it does not exist. 
    /// If ID value is null, a new entity is created. If ID has a value, then existing entity is replaced.
    /// </summary>
    /// <typeparam name="T">The type of entity</typeparam>
    /// <param name="entity">The instance to persist</param>
    /// <param name="cancellation">And optional cancellation token</param>
    Task SaveAsync<T>(T entity, CancellationToken cancellation = default) where T : IEntity;

    /// <summary>
    /// Saves a batch of complete entities replacing an existing entities or creating a new ones if they do not exist. 
    /// If ID value is null, a new entity is created. If ID has a value, then existing entity is replaced.
    /// </summary>
    /// <typeparam name="T">The type of entity</typeparam>
    /// <param name="entities">The entities to persist</param>
    /// <param name="cancellation">And optional cancellation token</param>
    Task<BulkWriteResult<T>> SaveAsync<T>(IEnumerable<T> entities, CancellationToken cancellation = default)
        where T : IEntity;

    /// <summary>
    /// Saves an entity partially with only the specified subset of properties. 
    /// If ID value is null, a new entity is created. If ID has a value, then existing entity is updated.
    /// <para>TIP: The properties to be saved can be specified with a 'New' expression. 
    /// You can only specify root level properties with the expression.</para>
    /// </summary>
    /// <typeparam name="T">Any class that implements IEntity</typeparam>
    /// <param name="entity">The entity to save</param>
    /// <param name="members">x => new { x.PropOne, x.PropTwo }</param>
    /// <param name="cancellation">An optional cancellation token</param>
    Task<UpdateResult> SaveOnlyAsync<T>(T entity, Expression<Func<T, object>> members,
        CancellationToken cancellation = default) where T : IEntity;

    /// <summary>
    /// Saves an entity partially with only the specified subset of properties. 
    /// If ID value is null, a new entity is created. If ID has a value, then existing entity is updated.
    /// <para>TIP: The properties to be saved can be specified with an IEnumerable. 
    /// Property names must match exactly.</para>
    /// </summary>
    /// <typeparam name="T">Any class that implements IEntity</typeparam>
    /// <param name="entity">The entity to save</param>
    /// <param name="propNames">new List { "PropOne", "PropTwo" }</param>
    /// <param name="cancellation">An optional cancellation token</param>
    Task<UpdateResult> SaveOnlyAsync<T>(T entity, IEnumerable<string> propNames,
        CancellationToken cancellation = default) where T : IEntity;

    /// <summary>
    /// Saves a batch of entities partially with only the specified subset of properties. 
    /// If ID value is null, a new entity is created. If ID has a value, then existing entity is updated.
    /// <para>TIP: The properties to be saved can be specified with a 'New' expression. 
    /// You can only specify root level properties with the expression.</para>
    /// </summary>
    /// <typeparam name="T">Any class that implements IEntity</typeparam>
    /// <param name="entities">The batch of entities to save</param>
    /// <param name="members">x => new { x.PropOne, x.PropTwo }</param>
    /// <param name="cancellation">An optional cancellation token</param>
    Task<BulkWriteResult<T>> SaveOnlyAsync<T>(IEnumerable<T> entities, Expression<Func<T, object>> members,
        CancellationToken cancellation = default) where T : IEntity;

    /// <summary>
    /// Saves a batch of entities partially with only the specified subset of properties. 
    /// If ID value is null, a new entity is created. If ID has a value, then existing entity is updated.
    /// <para>TIP: The properties to be saved can be specified with an IEnumerable. 
    /// Property names must match exactly.</para>
    /// </summary>
    /// <typeparam name="T">Any class that implements IEntity</typeparam>
    /// <param name="entities">The batch of entities to save</param>
    /// <param name="propNames">new List { "PropOne", "PropTwo" }</param>
    /// <param name="cancellation">An optional cancellation token</param>
    Task<BulkWriteResult<T>> SaveOnlyAsync<T>(IEnumerable<T> entities, IEnumerable<string> propNames,
        CancellationToken cancellation = default) where T : IEntity;

    /// <summary>
    /// Saves an entity partially excluding the specified subset of properties. 
    /// If ID value is null, a new entity is created. If ID has a value, then existing entity is updated.
    /// <para>TIP: The properties to be excluded can be specified with a 'New' expression. 
    /// You can only specify root level properties with the expression.</para>
    /// </summary>
    /// <typeparam name="T">Any class that implements IEntity</typeparam>
    /// <param name="entity">The entity to save</param>
    /// <param name="members">x => new { x.PropOne, x.PropTwo }</param>
    /// <param name="cancellation">An optional cancellation token</param>
    Task<UpdateResult> SaveExceptAsync<T>(T entity, Expression<Func<T, object>> members,
        CancellationToken cancellation = default) where T : IEntity;

    /// <summary>
    /// Saves an entity partially excluding the specified subset of properties. 
    /// If ID value is null, a new entity is created. If ID has a value, then existing entity is updated.
    /// <para>TIP: The properties to be saved can be specified with an IEnumerable. 
    /// Property names must match exactly.</para>
    /// </summary>
    /// <typeparam name="T">Any class that implements IEntity</typeparam>
    /// <param name="entity">The entity to save</param>
    /// <param name="propNames">new List { "PropOne", "PropTwo" }</param>
    /// <param name="cancellation">An optional cancellation token</param>
    Task<UpdateResult> SaveExceptAsync<T>(T entity, IEnumerable<string> propNames,
        CancellationToken cancellation = default) where T : IEntity;

    /// <summary>
    /// Saves a batch of entities partially excluding the specified subset of properties. 
    /// If ID value is null, a new entity is created. If ID has a value, then existing entity is updated.
    /// <para>TIP: The properties to be excluded can be specified with a 'New' expression. 
    /// You can only specify root level properties with the expression.</para>
    /// </summary>
    /// <typeparam name="T">Any class that implements IEntity</typeparam>
    /// <param name="entities">The batch of entities to save</param>
    /// <param name="members">x => new { x.PropOne, x.PropTwo }</param>
    /// <param name="cancellation">An optional cancellation token</param>
    Task<BulkWriteResult<T>> SaveExceptAsync<T>(IEnumerable<T> entities, Expression<Func<T, object>> members,
        CancellationToken cancellation = default) where T : IEntity;

    /// <summary>
    /// Saves a batch of entities partially excluding the specified subset of properties. 
    /// If ID value is null, a new entity is created. If ID has a value, then existing entity is updated.
    /// <para>TIP: The properties to be saved can be specified with an IEnumerable. 
    /// Property names must match exactly.</para>
    /// </summary>
    /// <typeparam name="T">Any class that implements IEntity</typeparam>
    /// <param name="entities">The batch of entities to save</param>
    /// <param name="propNames">new List { "PropOne", "PropTwo" }</param>
    /// <param name="cancellation">An optional cancellation token</param>
    Task<BulkWriteResult<T>> SaveExceptAsync<T>(IEnumerable<T> entities, IEnumerable<string> propNames,
        CancellationToken cancellation = default) where T : IEntity;

    /// <summary>
    /// Saves an entity partially while excluding some properties
    /// The properties to be excluded can be specified using the [Preserve] or [DontPreserve] attributes.
    /// </summary>
    /// <typeparam name="T">The type of entity</typeparam>
    /// <param name="entity">The entity to save</param>
    /// <param name="cancellation">An optional cancellation token</param>
    Task<UpdateResult> SavePreservingAsync<T>(T entity, CancellationToken cancellation = default)
        where T : IEntity;

    /// <summary>
    /// Starts an update command for the given entity type
    /// </summary>
    /// <typeparam name="T">The type of entity</typeparam>
    Update<T> Update<T>() where T : IEntity;

    /// <summary>
    /// Starts an update-and-get command for the given entity type
    /// </summary>
    /// <typeparam name="T">The type of entity</typeparam>
    UpdateAndGet<T, T> UpdateAndGet<T>() where T : IEntity;

    /// <summary>
    /// Starts an update-and-get command with projection support for the given entity type
    /// </summary>
    /// <typeparam name="T">The type of entity</typeparam>
    /// <typeparam name="TProjection">The type of the end result</typeparam>
    UpdateAndGet<T, TProjection> UpdateAndGet<T, TProjection>() where T : IEntity;

    /// <summary>
    /// Returns a new instance of the supplied IMongoEntity type
    /// </summary>
    /// <typeparam name="T">Any class that implements IMongoEntity</typeparam>
    T Entity<T>() where T : IMongoEntity, new();

    /// <summary>
    /// Returns a new instance of the supplied IMongoEntity type with the ID set to the supplied value
    /// </summary>
    /// <typeparam name="T">Any class that implements IMongoEntity</typeparam>
    /// <param name="id">The ID to set on the returned instance</param>
    T Entity<T>(string id) where T : IMongoEntity, new();

    /// <summary>
    /// Gets the MongoDB database for the given entity.
    /// </summary>
    /// <typeparam name="TEntity">The entity to get the database for.</typeparam>
    /// <returns>MongoDB database.</returns>
    IMongoDatabase GetDatabaseFor<TEntity>() where TEntity : IEntity;
}
