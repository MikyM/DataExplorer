using DataExplorer.MongoDb.Abstractions.DataContexts;

namespace DataExplorer.MongoDb.DataContexts;

/// <inheritdoc cref="IMongoDbContext"/>
/// <summary>
/// Represents a MongoDB database context.
/// </summary>
[PublicAPI]
public abstract class MongoDbContext : DBContext, IMongoDbContext
{
    /// <inheritdoc/>
    public MongoDbContextOptions Options { get; }
    /// <inheritdoc/>
    public IMongoDatabase MongoDatabase => DB.Database(Options.Database);
    /// <inheritdoc/>
    public IMongoDatabase GetDatabaseFor<TEntity>() where TEntity : IEntity => DB.Database<TEntity>();
    
    protected MongoDbContext(MongoDbContextOptions connectionSettings)
    {
        Options = connectionSettings;
        
        DB.InitAsync(connectionSettings.Database, connectionSettings.MongoClientSettings)
            .GetAwaiter()
            .GetResult();
        
        ModifiedBy = connectionSettings.ModifiedBy;
    }

    /// <summary>
    /// Returns a new instance of the supplied IMongoEntity type
    /// </summary>
    /// <typeparam name="T">Any class that implements IMongoEntity</typeparam>
    public T Entity<T>() where T : IMongoEntity, new()
        => new();

    /// <summary>
    /// Returns a new instance of the supplied IMongoEntity type with the ID set to the supplied value
    /// </summary>
    /// <typeparam name="T">Any class that implements IMongoEntity</typeparam>
    /// <param name="id">The ID to set on the returned instance</param>
    public T Entity<T>(string id) where T : IMongoEntity, new()
        => DB.Entity<T>(id);
}
