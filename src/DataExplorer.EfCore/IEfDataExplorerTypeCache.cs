using System.Diagnostics.CodeAnalysis;

namespace DataExplorer.EfCore;

/// <summary>
/// Provides access to cached info about types used by <see cref="DataExplorer"/>.
/// </summary>
[PublicAPI]
public interface IEfDataExplorerTypeCache
{
    /// <summary>
    /// Entity info.
    /// </summary>
    IReadOnlyDictionary<Type, DataExplorerEntityInfo> EntityInfo { get; }
    
    /// <summary>
    /// Repo info.
    /// </summary>
    IReadOnlyDictionary<Type, DataExplorerRepoInfo> RepoInfo { get; }
    
    /// <summary>
    /// Allowed repository types.
    /// </summary>
    IEnumerable<Type> AllowedRepoTypes { get; }
    
    /// <summary>
    /// Tries to get entity type info.
    /// </summary>
    /// <param name="entityImplementationType">The type.</param>
    /// <param name="info">The info.</param>
    /// <returns>True if info was found, otherwise false.</returns>
    bool TryGetEntityInfo(Type entityImplementationType, [NotNullWhen(true)] out DataExplorerEntityInfo? info);
    
    /// <summary>
    /// Tries to get repo type info.
    /// </summary>
    /// <param name="repoInterfaceType">The type.</param>
    /// <param name="info">The info.</param>
    /// <returns>True if info was found, otherwise false.</returns>
    bool TryGetRepoInfo(Type repoInterfaceType, [NotNullWhen(true)] out DataExplorerRepoInfo? info);
    
    /// <summary>
    /// Determines whether the given type is an allowed repo type.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>True if it's a supported type, otherwise false.</returns>
    bool IsAllowedRepoType(Type type);
}
