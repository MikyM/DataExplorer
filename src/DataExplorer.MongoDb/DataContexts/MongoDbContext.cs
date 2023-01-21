﻿using DataExplorer.MongoDb.Abstractions.DataContexts;

namespace DataExplorer.MongoDb.DataContexts;

/// <inheritdoc cref="IMongoDbContext"/>
/// <summary>
/// Represents a MongoDB database context.
/// </summary>
[PublicAPI]
public abstract class MongoDbContext : DBContext, IMongoDbContext
{
    public MongoDbContextOptions Options { get; }
    
    protected MongoDbContext(MongoDbContextOptions connectionSettings)
    {
        Options = connectionSettings;
        
        DB.InitAsync(connectionSettings.Database, connectionSettings.MongoClientSettings)
            .GetAwaiter()
            .GetResult();

        ModifiedBy = connectionSettings.ModifiedBy;
    }
}