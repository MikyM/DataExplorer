using DataExplorer.MongoDb.Abstractions.DataContexts;
using MongoDB.Entities;

namespace DataExplorer.MongoDb.DataContexts;

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
