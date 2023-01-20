using DataExplorer.Abstractions.Entities;
using DataExplorer.MongoDb.Extensions;
using DataExplorer.MongoDb.Repositories;
using MikyM.Utilities.Extensions;

namespace DataExplorer.MongoDb;

/// <summary>
/// Repository a util type cache.
/// </summary>
[PublicAPI]
public sealed class MongoDataExplorerTypeCache : IMongoDataExplorerTypeCache
{
    internal MongoDataExplorerTypeCache()
    {
        EntityTypeIdTypeDictionary ??= AppDomain.CurrentDomain.GetAssemblies().SelectMany(x =>
                x.GetTypes().Where(y => y.IsClass && !y.IsAbstract && y.IsAssignableToWithGenerics(typeof(IEntity<>))))
            .ToDictionary(x => x, x => x.GetIdType());
        
        var cachedCrudRepos = new Dictionary<Type, Type>();
        var cachedReadOnlyRepos = new Dictionary<Type, Type>();
        var cachedCrudGenericIdRepos = new Dictionary<Type, Type>();
        var cachedReadOnlyGenericIdRepos = new Dictionary<Type, Type>();
        
        foreach (var (entityType, idType) in EntityTypeIdTypeDictionary)
        {
            cachedCrudGenericIdRepos.TryAdd(entityType, typeof(MongoRepository<,>).MakeGenericType(entityType, idType));
            cachedReadOnlyGenericIdRepos.TryAdd(entityType, typeof(MongoReadOnlyRepository<,>).MakeGenericType(entityType, idType));

            if (idType != typeof(long))
                continue;
            
            cachedCrudRepos.TryAdd(entityType, typeof(MongoRepository<>).MakeGenericType(entityType));
            cachedReadOnlyRepos.TryAdd(entityType, typeof(MongoReadOnlyRepository<>).MakeGenericType(entityType));
        }

        AllowedRepoTypes = new[]
        {
            typeof(IMongoRepository<>), typeof(IMongoRepository<,>), typeof(IMongoReadOnlyRepository<>), typeof(IMongoReadOnlyRepository<,>)
        };

        CachedCrudRepos = cachedCrudRepos;
        CachedReadOnlyRepos = cachedReadOnlyRepos;
        CachedCrudGenericIdRepos = cachedCrudGenericIdRepos;
        CachedReadOnlyGenericIdRepos = cachedReadOnlyGenericIdRepos;
    }
    
    /// <inheritdoc/>
    public IReadOnlyDictionary<Type, Type> EntityTypeIdTypeDictionary { get; }
    
    /// <inheritdoc/>
    public IReadOnlyDictionary<Type, Type> CachedReadOnlyRepos { get; }
    
    /// <inheritdoc/>
    public IReadOnlyDictionary<Type, Type> CachedCrudRepos { get; }
    
    /// <inheritdoc/>
    public IReadOnlyDictionary<Type, Type> CachedReadOnlyGenericIdRepos { get; }
    
    /// <inheritdoc/>
    public IReadOnlyDictionary<Type, Type> CachedCrudGenericIdRepos { get; }
    
    /// <inheritdoc/>
    public IEnumerable<Type> AllowedRepoTypes { get; }
}
