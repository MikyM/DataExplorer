using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using AttributeBasedRegistration.Extensions;
using DataExplorer.EfCore.Extensions;
using DataExplorer.EfCore.Repositories;

namespace DataExplorer.EfCore;

/// <summary>
/// Provides access to cached info about types used by <see cref="DataExplorer"/>.
/// </summary>
[PublicAPI]
public sealed class EfDataExplorerTypeCache : IEfDataExplorerTypeCache
{
    internal EfDataExplorerTypeCache(IEnumerable<Assembly> assembliesWithEntityTypes)
    {
        var entityTypes = assembliesWithEntityTypes.SelectMany(x =>
            x.GetTypes().Where(y => y is { IsClass: true, IsAbstract: false } && y.IsAssignableToWithGenerics(typeof(IEntity<>))));

        var entities = new Dictionary<Type,DataExplorerEntityInfo>();
        var repos = new Dictionary<Type,DataExplorerRepoInfo>();
        
        foreach (var entityType in entityTypes)
        {
            var idType = entityType.GetIdType();
            var isDisableable = entityType.GetIsDisableable();
            
            var crudGenericIdRepo = typeof(Repository<,>).MakeGenericType(entityType, idType);
            var crudGenericIdRepoInt = typeof(IRepository<,>).MakeGenericType(entityType, idType);
            
            var roGenericIdRepo = typeof(ReadOnlyRepository<,>).MakeGenericType(entityType, idType);
            var roGenericIdRepoInt = typeof(IReadOnlyRepository<,>).MakeGenericType(entityType, idType);

            // handle special long case
            Type? crudLongIdRepo = null;
            Type? crudLongIdRepoInt = null;
            Type? roLongIdRepo = null;
            Type? roLongIdRepoInt = null;
            if (idType == typeof(long))
            {
                crudLongIdRepo = typeof(Repository<>).MakeGenericType(entityType);
                crudLongIdRepoInt = typeof(IRepository<>).MakeGenericType(entityType);
                
                roLongIdRepo = typeof(ReadOnlyRepository<>).MakeGenericType(entityType);
                roLongIdRepoInt = typeof(IReadOnlyRepository<>).MakeGenericType(entityType);
            }

            var crudGenericIdRepoInfo = new DataExplorerRepoInfo(crudGenericIdRepo, crudGenericIdRepoInt, 
                idType, true, false);
            var roGenericIdRepoInfo = new DataExplorerRepoInfo(roGenericIdRepo, roGenericIdRepoInt,
                 idType, false, false);
            var crudLongIdRepoInfo = crudLongIdRepo is null
                ? null
                : new DataExplorerRepoInfo(crudLongIdRepo, crudLongIdRepoInt!, idType, true, true);
            var roLongIdRepoInfo = roLongIdRepo is null
                ? null
                : new DataExplorerRepoInfo(roLongIdRepo, roLongIdRepoInt!,  idType, false, true);

            var interfaces = entityType.GetInterfaces();
            var isSnowflake = entityType.GetIsSnowflake();

            var info = new DataExplorerEntityInfo(entityType, interfaces, idType, crudGenericIdRepoInfo,
                roGenericIdRepoInfo, crudLongIdRepoInfo, roLongIdRepoInfo,
                isDisableable, isSnowflake, idType == typeof(long));
            
            crudGenericIdRepoInfo.SetEntityInfo(info);
            roGenericIdRepoInfo.SetEntityInfo(info);
            crudLongIdRepoInfo?.SetEntityInfo(info);
            roLongIdRepoInfo?.SetEntityInfo(info);

            entities.Add(entityType, info);
            repos.Add(crudGenericIdRepoInt, crudGenericIdRepoInfo);
            repos.Add(roGenericIdRepoInt, roGenericIdRepoInfo);
            if (crudLongIdRepoInfo is not null && crudLongIdRepoInt is not null)
            {
                repos.Add(crudLongIdRepoInt, crudLongIdRepoInfo);
            }
            if (roLongIdRepoInfo is not null && roLongIdRepoInt is not null)
            {
                repos.Add(roLongIdRepoInt, roLongIdRepoInfo);
            }
        }

        AllowedRepoTypes = new[]
        {
            typeof(IRepository<>), typeof(IRepository<,>), typeof(IReadOnlyRepository<>), typeof(IReadOnlyRepository<,>)
        };

#if NET7_0_OR_GREATER
        EntityInfo = entities.AsReadOnly();
        RepoInfo = repos.AsReadOnly();
#else
        EntityInfo = entities;
        RepoInfo = repos;
#endif

    }
    
    /// <inheritdoc/>
    public IEnumerable<Type> AllowedRepoTypes { get; }

    /// <inheritdoc/>
    public bool TryGetEntityInfo(Type entityImplementationType, [NotNullWhen(true)] out DataExplorerEntityInfo? info)
        => EntityInfo.TryGetValue(entityImplementationType, out info);

    /// <inheritdoc/>
    public bool TryGetRepoInfo(Type repoInterfaceType, [NotNullWhen(true)] out DataExplorerRepoInfo? info)
        => RepoInfo.TryGetValue(repoInterfaceType, out info);

    /// <inheritdoc/>
    public bool IsAllowedRepoType(Type type)
        => AllowedRepoTypes.FirstOrDefault(x => x == type) is not null;

    /// <inheritdoc/>
    public IReadOnlyDictionary<Type, DataExplorerEntityInfo> EntityInfo { get; }
    /// <inheritdoc/>
    public IReadOnlyDictionary<Type, DataExplorerRepoInfo> RepoInfo { get; }
}
