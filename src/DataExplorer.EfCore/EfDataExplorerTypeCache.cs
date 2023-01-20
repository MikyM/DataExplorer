using DataExplorer.EfCore.Extensions;
using DataExplorer.EfCore.Repositories;
using MikyM.Utilities.Extensions;

namespace DataExplorer.EfCore;

/// <summary>
/// Repository a util type cache.
/// </summary>
[PublicAPI]
public sealed class EfDataExplorerTypeCache : IEfDataExplorerTypeCache
{
    internal EfDataExplorerTypeCache()
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
            cachedCrudGenericIdRepos.TryAdd(entityType, typeof(Repository<,>).MakeGenericType(entityType, idType));
            cachedReadOnlyGenericIdRepos.TryAdd(entityType, typeof(ReadOnlyRepository<,>).MakeGenericType(entityType, idType));

            if (idType != typeof(long))
                continue;
            
            cachedCrudRepos.TryAdd(entityType, typeof(Repository<>).MakeGenericType(entityType));
            cachedReadOnlyRepos.TryAdd(entityType, typeof(ReadOnlyRepository<>).MakeGenericType(entityType));
        }

        AllowedRepoTypes = new[]
        {
            typeof(IRepository<>), typeof(IRepository<,>), typeof(IReadOnlyRepository<>), typeof(IReadOnlyRepository<,>)
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
