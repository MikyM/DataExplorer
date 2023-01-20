using DataExplorer.MongoDb.Abstractions.DataContexts;
using MongoDB.Entities;

namespace DataExplorer.MongoDb.DataContexts;

/// <inheritdoc cref="IMongoDbContext"/>
/// <summary>
/// Represents a MongoDB database context.
/// </summary>
[PublicAPI]
public abstract class MongoDbContext : DBContext, IMongoDbContext
{
    public MongoDbConnectionSettings ConnectionSettings { get; }

    protected MongoDbContext(MongoDbConnectionSettings connectionSettings)
    {
        ConnectionSettings = connectionSettings;
        
        DB.InitAsync(connectionSettings.Database, connectionSettings.MongoClientSettings)
            .GetAwaiter()
            .GetResult();

        ModifiedBy = connectionSettings.ModifiedBy;
    }
}
