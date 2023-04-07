using System.Reflection;
using DataExplorer.MongoDb.Repositories;
using MikyM.Utilities.Extensions;

namespace DataExplorer.MongoDb;

/// <summary>
/// Repository a util type cache.
/// </summary>
[PublicAPI]
public sealed class MongoDataExplorerTypeCache : IMongoDataExplorerTypeCache
{
    internal MongoDataExplorerTypeCache(IEnumerable<Assembly> assembliesWithEntities)
    {
        EntityTypes ??= assembliesWithEntities.SelectMany(x =>
            x.GetTypes().Where(y => y.IsClass && !y.IsAbstract && y.IsAssignableToWithGenerics(typeof(IMongoEntity)))).ToList().AsReadOnly();

        var cachedCrudRepos = new Dictionary<Type, Type>();
        var cachedReadOnlyRepos = new Dictionary<Type, Type>();

        foreach (var type in EntityTypes)
        {
            cachedCrudRepos.TryAdd(type, typeof(MongoRepository<>).MakeGenericType(type));
            cachedReadOnlyRepos.Add(type, typeof(MongoReadOnlyRepository<>).MakeGenericType(type));
        }

        AllowedRepoTypes = new[]
        {
            typeof(IMongoRepository<>), typeof(IMongoReadOnlyRepository<>)
        };

        CachedCrudRepos = cachedCrudRepos;
        CachedReadOnlyRepos = cachedReadOnlyRepos;
    }
    
    /// <inheritdoc/>
    public IReadOnlyList<Type> EntityTypes { get; }

    /// <inheritdoc/>
    public IReadOnlyDictionary<Type,Type> CachedReadOnlyRepos { get; }
    
    /// <inheritdoc/>
    public IReadOnlyDictionary<Type,Type> CachedCrudRepos { get; }

    /// <inheritdoc/>
    public IEnumerable<Type> AllowedRepoTypes { get; }
}
