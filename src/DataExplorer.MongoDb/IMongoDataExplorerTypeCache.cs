namespace DataExplorer.MongoDb;

/// <summary>
/// Represents a util type cache.
/// </summary>
[PublicAPI]
public interface IMongoDataExplorerTypeCache
{
    /// <summary>
    /// Entity type to Id type map.
    /// </summary>
    IReadOnlyDictionary<Type, Type> EntityTypeIdTypeDictionary { get; }
    
    /// <summary>
    /// Entity type to read-only repo implementation type with long as Id type map.
    /// </summary>
    IReadOnlyDictionary<Type, Type> CachedReadOnlyRepos { get; }
    
    /// <summary>
    /// Entity type to crud repo implementation type with long as Id type map.
    /// </summary>
    IReadOnlyDictionary<Type, Type> CachedCrudRepos { get; }
    
    /// <summary>
    /// Entity type to read-only repo implementation type with generic Id type argument map.
    /// </summary>
    IReadOnlyDictionary<Type, Type> CachedReadOnlyGenericIdRepos { get; }
    
    /// <summary>
    /// Entity type to crud repo implementation type with generic Id type argument map.
    /// </summary>
    IReadOnlyDictionary<Type, Type> CachedCrudGenericIdRepos { get; }
    
    /// <summary>
    /// Allowed repository types.
    /// </summary>
    IEnumerable<Type> AllowedRepoTypes { get; }
}
