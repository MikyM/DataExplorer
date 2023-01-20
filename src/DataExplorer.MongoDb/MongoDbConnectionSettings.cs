using DataExplorer.MongoDb.Abstractions.DataContexts;
using MongoDB.Driver;
using MongoDB.Entities;

namespace DataExplorer.MongoDb;

/// <summary>
/// The MongoDB connection settings.
/// </summary>
/// <param name="Database">Database name.</param>
/// <param name="MongoClientSettings">Mongo client settings.</param>
/// <param name="ModifiedBy">Modified by.</param>
[PublicAPI]
public abstract record MongoDbConnectionSettings(string Database, MongoClientSettings MongoClientSettings,
    ModifiedBy? ModifiedBy);

/// <summary>
/// The MongoDB connection settings.
/// </summary>
/// <param name="Database">Database name.</param>
/// <param name="MongoClientSettings">Mongo client settings.</param>
/// <param name="ModifiedBy">Modified by.</param>
/// <typeparam name="TMongoContext">The context of the database these settings are for.</typeparam>
[PublicAPI]
public record MongoDbConnectionSettings<TMongoContext>(string Database, MongoClientSettings MongoClientSettings,
    ModifiedBy? ModifiedBy) : MongoDbConnectionSettings(Database, MongoClientSettings, ModifiedBy)
    where TMongoContext : class, IMongoDbContext;