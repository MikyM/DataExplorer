using DataExplorer.Abstractions.Entities;
using DataExplorer.MongoDb.Extensions;
using DataExplorer.MongoDb.Repositories;
using MikyM.Utilities.Extensions;

namespace DataExplorer.MongoDb;

/// <summary>
/// UoF Cache.
/// </summary>
internal static class UoFCache
{
    static UoFCache()
    {
        EntityTypeIdTypeDictionary ??= AppDomain.CurrentDomain.GetAssemblies().SelectMany(x =>
                x.GetTypes().Where(y => y.IsClass && !y.IsAbstract && y.IsAssignableToWithGenerics(typeof(IEntity<>))))
            .ToDictionary(x => x, x => x.GetIdType());

        CachedCrudRepos = new Dictionary<Type, Type>();
        CachedReadOnlyRepos = new Dictionary<Type, Type>();
        CachedCrudGenericIdRepos = new Dictionary<Type, Type>();
        CachedReadOnlyGenericIdRepos = new Dictionary<Type, Type>();
        
        foreach (var (entityType, idType) in EntityTypeIdTypeDictionary)
        {
            CachedCrudGenericIdRepos.TryAdd(entityType, typeof(MongoRepository<,>).MakeGenericType(entityType, idType));
            CachedReadOnlyGenericIdRepos.TryAdd(entityType, typeof(MongoReadOnlyRepository<,>).MakeGenericType(entityType, idType));

            if (idType != typeof(long))
                continue;
            
            CachedCrudRepos.TryAdd(entityType, typeof(MongoRepository<>).MakeGenericType(entityType));
            CachedReadOnlyRepos.TryAdd(entityType, typeof(MongoReadOnlyRepository<>).MakeGenericType(entityType));
        }

        AllowedRepoTypes = new[]
        {
            typeof(IMongoRepository<>), typeof(IMongoRepository<,>), typeof(IMongoReadOnlyRepository<>), typeof(IMongoReadOnlyRepository<,>)
        };
    }
    
    internal static Dictionary<Type, Type> EntityTypeIdTypeDictionary { get; }
    internal static Dictionary<Type, Type> CachedReadOnlyRepos { get; }
    internal static Dictionary<Type, Type> CachedCrudRepos { get; }
    internal static Dictionary<Type, Type> CachedReadOnlyGenericIdRepos { get; }
    internal static Dictionary<Type, Type> CachedCrudGenericIdRepos { get; }
    internal static Type[] AllowedRepoTypes { get; }
}
