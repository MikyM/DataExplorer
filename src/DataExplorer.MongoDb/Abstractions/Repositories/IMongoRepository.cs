using System.Linq.Expressions;
using DataExplorer.Abstractions.Entities;
using DataExplorer.Exceptions;

#pragma warning disable CS1574, CS1584, CS1581, CS1580

namespace DataExplorer.MongoDb.Abstractions.Repositories;

/// <summary>
/// Repository.
/// </summary>
/// <typeparam name="TEntity">Entity that derives from <see cref="MongoEntity"/>.</typeparam>
[PublicAPI]
public interface IMongoRepository<TEntity> : IMongoReadOnlyRepository<TEntity> where TEntity : MongoEntity
{
    /// <summary>
    /// Creates a collection for an Entity type explicitly using the given options
    /// </summary>

    /// <param name="options">The options to use for collection creation</param>
    /// <param name="cancellation">An optional cancellation token</param>
    Task CreateCollectionAsync(Action<CreateCollectionOptions<TEntity>> options, CancellationToken cancellation = default);

    /// <summary>
    /// Deletes the collection of a given entity type as well as the join collections for that entity.
    /// <para>TIP: When deleting a collection, all relationships associated with that entity type is also deleted.</para>
    /// </summary>

    Task DropCollectionAsync();
    
    /// <summary>
    /// Deletes a single entity from MongoDB
    /// <para>HINT: If this entity is referenced by one-to-many/many-to-many relationships, those references are also deleted.</para>
    /// </summary>

    /// <param name="id">The Id of the entity to delete</param>
    /// <param name="cancellation">An optional cancellation token</param>
    /// <param name="ignoreGlobalFilters">Set to true if you'd like to ignore any global filters for this operation</param>
    Task<DeleteResult> DeleteAsync(string id, CancellationToken cancellation = default,
        bool ignoreGlobalFilters = false);

    /// <summary>
    /// Deletes matching entities from MongoDB
    /// <para>HINT: If these entities are referenced by one-to-many/many-to-many relationships, those references are also deleted.</para>
    /// <para>TIP: Try to keep the number of entities to delete under 100 in a single call</para>
    /// </summary>

    /// <param name="ids">An IEnumerable of entity IDs</param>
    /// <param name="cancellation">An optional cancellation token</param>
    /// <param name="ignoreGlobalFilters">Set to true if you'd like to ignore any global filters for this operation</param>
    Task<DeleteResult> DeleteAsync(IEnumerable<string> ids, CancellationToken cancellation = default,
        bool ignoreGlobalFilters = false);

    /// <summary>
    /// Deletes matching entities from MongoDB
    /// <para>HINT: If these entities are referenced by one-to-many/many-to-many relationships, those references are also deleted.</para>
    /// <para>TIP: Try to keep the number of entities to delete under 100 in a single call</para>
    /// </summary>

    /// <param name="expression">A lambda expression for matching entities to delete.</param>
    /// <param name="cancellation">An optional cancellation token</param>
    /// <param name="collation">An optional collation object</param>
    /// <param name="ignoreGlobalFilters">Set to true if you'd like to ignore any global filters for this operation</param>
    Task<DeleteResult> DeleteAsync(Expression<Func<TEntity, bool>> expression,
        CancellationToken cancellation = default, Collation? collation = null, bool ignoreGlobalFilters = false);

    /// <summary>
    /// Deletes matching entities with a filter expression
    /// <para>HINT: If the expression matches more than 100,000 entities, they will be deleted in batches of 100k.</para>
    /// <para>HINT: If these entities are referenced by one-to-many/many-to-many relationships, those references are also deleted.</para>
    /// </summary>

    /// <param name="filter">f => f.Eq(x => x.Prop, Value) &amp; f.Gt(x => x.Prop, Value)</param>
    /// <param name="cancellation">An optional cancellation token</param>
    /// <param name="collation">An optional collation object</param>
    /// <param name="ignoreGlobalFilters">Set to true if you'd like to ignore any global filters for this operation</param>
    Task<DeleteResult> DeleteAsync(Func<FilterDefinitionBuilder<TEntity>, FilterDefinition<TEntity>> filter,
        CancellationToken cancellation = default, Collation? collation = null, bool ignoreGlobalFilters = false);

    /// <summary>
    /// Deletes matching entities with a filter definition
    /// <para>HINT: If the expression matches more than 100,000 entities, they will be deleted in batches of 100k.</para>
    /// <para>HINT: If these entities are referenced by one-to-many/many-to-many relationships, those references are also deleted.</para>
    /// </summary>

    /// <param name="filter">A filter definition for matching entities to delete.</param>
    /// <param name="cancellation">An optional cancellation token</param>
    /// <param name="collation">An optional collation object</param>
    /// <param name="ignoreGlobalFilters">Set to true if you'd like to ignore any global filters for this operation</param>
    Task<DeleteResult> DeleteAsync(FilterDefinition<TEntity> filter, CancellationToken cancellation = default,
        Collation? collation = null, bool ignoreGlobalFilters = false);
    
    /// <summary>
    /// Saves a complete entity replacing an existing entity or creating a new one if it does not exist. 
    /// If ID value is null, a new entity is created. If ID has a value, then existing entity is replaced.
    /// </summary>

    /// <param name="entity">The instance to persist</param>
    /// <param name="cancellation">And optional cancellation token</param>
    Task InsertAsync(TEntity entity, CancellationToken cancellation = default);

    /// <summary>
    /// Saves a batch of complete entities replacing an existing entities or creating a new ones if they do not exist. 
    /// If ID value is null, a new entity is created. If ID has a value, then existing entity is replaced.
    /// </summary>

    /// <param name="entities">The entities to persist</param>
    /// <param name="cancellation">And optional cancellation token</param>
    Task<BulkWriteResult<TEntity>> InsertAsync(IEnumerable<TEntity> entities, CancellationToken cancellation = default);
    
    /// <summary>
    /// Starts a replace command for the given entity type
    /// <para>TIP: Only the first matched entity will be replaced</para>
    /// </summary>

    Replace<TEntity> Replace();
    
    /// <summary>
    /// Saves a complete entity replacing an existing entity or creating a new one if it does not exist. 
    /// If ID value is null, a new entity is created. If ID has a value, then existing entity is replaced.
    /// </summary>

    /// <param name="entity">The instance to persist</param>
    /// <param name="cancellation">And optional cancellation token</param>
    Task SaveAsync(TEntity entity, CancellationToken cancellation = default);
    
    /// <summary>
    /// Saves a batch of complete entities replacing an existing entities or creating a new ones if they do not exist. 
    /// If ID value is null, a new entity is created. If ID has a value, then existing entity is replaced.
    /// </summary>

    /// <param name="entities">The entities to persist</param>
    /// <param name="cancellation">And optional cancellation token</param>
    Task<BulkWriteResult<TEntity>> SaveAsync(IEnumerable<TEntity> entities, CancellationToken cancellation = default);

    /// <summary>
    /// Saves an entity partially with only the specified subset of properties. 
    /// If ID value is null, a new entity is created. If ID has a value, then existing entity is updated.
    /// <para>TIP: The properties to be saved can be specified with a 'New' expression. 
    /// You can only specify root level properties with the expression.</para>
    /// </summary>

    /// <param name="entity">The entity to save</param>
    /// <param name="members">x => new { x.PropOne, x.PropTwo }</param>
    /// <param name="cancellation">An optional cancellation token</param>
    Task<UpdateResult> SaveOnlyAsync(TEntity entity, Expression<Func<TEntity, object?>> members,
        CancellationToken cancellation = default);

    /// <summary>
    /// Saves an entity partially with only the specified subset of properties. 
    /// If ID value is null, a new entity is created. If ID has a value, then existing entity is updated.
    /// <para>TIP: The properties to be saved can be specified with an IEnumerable. 
    /// Property names must match exactly.</para>
    /// </summary>

    /// <param name="entity">The entity to save</param>
    /// <param name="propNames">new List { "PropOne", "PropTwo" }</param>
    /// <param name="cancellation">An optional cancellation token</param>
    Task<UpdateResult> SaveOnlyAsync(TEntity entity, IEnumerable<string> propNames,
        CancellationToken cancellation = default);

    /// <summary>
    /// Saves a batch of entities partially with only the specified subset of properties. 
    /// If ID value is null, a new entity is created. If ID has a value, then existing entity is updated.
    /// <para>TIP: The properties to be saved can be specified with a 'New' expression. 
    /// You can only specify root level properties with the expression.</para>
    /// </summary>

    /// <param name="entities">The batch of entities to save</param>
    /// <param name="members">x => new { x.PropOne, x.PropTwo }</param>
    /// <param name="cancellation">An optional cancellation token</param>
    Task<BulkWriteResult<TEntity>> SaveOnlyAsync(IEnumerable<TEntity> entities, Expression<Func<TEntity, object?>> members,
        CancellationToken cancellation = default);

    /// <summary>
    /// Saves a batch of entities partially with only the specified subset of properties. 
    /// If ID value is null, a new entity is created. If ID has a value, then existing entity is updated.
    /// <para>TIP: The properties to be saved can be specified with an IEnumerable. 
    /// Property names must match exactly.</para>
    /// </summary>

    /// <param name="entities">The batch of entities to save</param>
    /// <param name="propNames">new List { "PropOne", "PropTwo" }</param>
    /// <param name="cancellation">An optional cancellation token</param>
    Task<BulkWriteResult<TEntity>> SaveOnlyAsync(IEnumerable<TEntity> entities, IEnumerable<string> propNames,
        CancellationToken cancellation = default);

    /// <summary>
    /// Saves an entity partially excluding the specified subset of properties. 
    /// If ID value is null, a new entity is created. If ID has a value, then existing entity is updated.
    /// <para>TIP: The properties to be excluded can be specified with a 'New' expression. 
    /// You can only specify root level properties with the expression.</para>
    /// </summary>

    /// <param name="entity">The entity to save</param>
    /// <param name="members">x => new { x.PropOne, x.PropTwo }</param>
    /// <param name="cancellation">An optional cancellation token</param>
    Task<UpdateResult> SaveExceptAsync(TEntity entity, Expression<Func<TEntity, object?>> members,
        CancellationToken cancellation = default);

    /// <summary>
    /// Saves an entity partially excluding the specified subset of properties. 
    /// If ID value is null, a new entity is created. If ID has a value, then existing entity is updated.
    /// <para>TIP: The properties to be saved can be specified with an IEnumerable. 
    /// Property names must match exactly.</para>
    /// </summary>

    /// <param name="entity">The entity to save</param>
    /// <param name="propNames">new List { "PropOne", "PropTwo" }</param>
    /// <param name="cancellation">An optional cancellation token</param>
    Task<UpdateResult> SaveExceptAsync(TEntity entity, IEnumerable<string> propNames,
        CancellationToken cancellation = default);

    /// <summary>
    /// Saves a batch of entities partially excluding the specified subset of properties. 
    /// If ID value is null, a new entity is created. If ID has a value, then existing entity is updated.
    /// <para>TIP: The properties to be excluded can be specified with a 'New' expression. 
    /// You can only specify root level properties with the expression.</para>
    /// </summary>

    /// <param name="entities">The batch of entities to save</param>
    /// <param name="members">x => new { x.PropOne, x.PropTwo }</param>
    /// <param name="cancellation">An optional cancellation token</param>
    Task<BulkWriteResult<TEntity>> SaveExceptAsync(IEnumerable<TEntity> entities, Expression<Func<TEntity, object?>> members,
        CancellationToken cancellation = default);

    /// <summary>
    /// Saves a batch of entities partially excluding the specified subset of properties. 
    /// If ID value is null, a new entity is created. If ID has a value, then existing entity is updated.
    /// <para>TIP: The properties to be saved can be specified with an IEnumerable. 
    /// Property names must match exactly.</para>
    /// </summary>

    /// <param name="entities">The batch of entities to save</param>
    /// <param name="propNames">new List { "PropOne", "PropTwo" }</param>
    /// <param name="cancellation">An optional cancellation token</param>
    Task<BulkWriteResult<TEntity>> SaveExceptAsync(IEnumerable<TEntity> entities, IEnumerable<string> propNames,
        CancellationToken cancellation = default);

    /// <summary>
    /// Saves an entity partially while excluding some properties
    /// The properties to be excluded can be specified using the [Preserve] or [DontPreserve] attributes.
    /// </summary>

    /// <param name="entity">The entity to save</param>
    /// <param name="cancellation">An optional cancellation token</param>
    Task<UpdateResult> SavePreservingAsync(TEntity entity, CancellationToken cancellation = default);

    /// <summary>
    /// Starts an update command for the given entity type
    /// </summary>

    Update<TEntity> Update();

    /// <summary>
    /// Starts an update-and-get command for the given entity type
    /// </summary>

    UpdateAndGet<TEntity, TEntity> UpdateAndGet();

    /// <summary>
    /// Starts an update-and-get command with projection support for the given entity type
    /// </summary>

    /// <typeparam name="TProjection">The type of the end result</typeparam>
    UpdateAndGet<TEntity, TProjection> UpdateAndGet<TProjection>();

    /// <summary>
    ///     <para>
    ///         Disables a range of entities.
    ///     </para>
    /// </summary>
    /// <param name="entities">Entities to disable.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <exception cref="InvalidOperationException">Thrown when the given entities do not implement <see cref="IDisableable"/>.</exception>
    Task DisableRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
    
    /// <summary>
    ///     <para>
    ///         Disables an entity.
    ///     </para>
    /// </summary>
    /// <param name="entity">The entity to disable.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <exception cref="NotFoundException">Thrown when entity with given Id is not found.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the given entity does not implement <see cref="IDisableable"/>.</exception>
    Task DisableAsync(TEntity entity, CancellationToken cancellationToken = default);
}
