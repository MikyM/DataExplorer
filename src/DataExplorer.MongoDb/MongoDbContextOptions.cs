using DataExplorer.MongoDb.Abstractions.DataContexts;

namespace DataExplorer.MongoDb;

/// <summary>
/// The MongoDB connection settings.
/// </summary>
[PublicAPI]
public abstract class MongoDbContextOptions
{
    /// <summary>
    /// The MongoDB connection settings.
    /// </summary>
    /// <param name="database">Database name.</param>
    /// <param name="mongoClientSettings">Mongo client settings.</param>
    /// <param name="modifiedBy">Modified by.</param>
    /// <param name="contextType">Db Context destination.</param>
    protected MongoDbContextOptions(string database, MongoClientSettings mongoClientSettings,
        ModifiedBy? modifiedBy, Type contextType)
    {
        Database = database;
        MongoClientSettings = mongoClientSettings;
        ModifiedBy = modifiedBy;
        ContextType = contextType;
    }

    /// <summary>
    /// Constructor.
    /// </summary>
    internal MongoDbContextOptions()
    {
    }

    /// <summary>Database name.</summary>
    public string Database { get; set; } = string.Empty;

    /// <summary>Mongo client settings.</summary>
    public MongoClientSettings MongoClientSettings { get; set; } = new();

    /// <summary>Modified by.</summary>
    public ModifiedBy? ModifiedBy { get; set; }

    /// <summary>Db Context destination.</summary>
    public Type ContextType { get; internal set; } = null!;
}

/// <summary>
/// The MongoDB connection settings.
/// </summary>
[PublicAPI]
public sealed class MongoDbContextOptions<TMongoContext> : MongoDbContextOptions where TMongoContext : IMongoDbContext
{
    /// <summary>
    /// The MongoDB connection settings.
    /// </summary>
    /// <param name="database">Database name.</param>
    /// <param name="mongoClientSettings">Mongo client settings.</param>
    /// <param name="modifiedBy">Modified by.</param>
    /// <param name="contextType">Db Context destination.</param>
    internal MongoDbContextOptions(string database, MongoClientSettings mongoClientSettings,
        ModifiedBy? modifiedBy, Type contextType) : base(database, mongoClientSettings, modifiedBy, contextType)
    {
    }

    internal MongoDbContextOptions()
    {
    }
}
