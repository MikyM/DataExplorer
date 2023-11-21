namespace DataExplorer.EfCore.Extensions;

/// <summary>
/// Type extensions.
/// </summary>
[UsedImplicitly]
internal static class TypeExtensions
{
    internal static Type GetIdType(this Type type)
    {
        var generic = type.GetInterfaces()
            .First(x => x.IsInterface && x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEntity<>));

        return generic.GenericTypeArguments.First();
    }
    
    internal static bool GetIsDisableable(this Type type)
        => type.GetInterfaces().FirstOrDefault(x => x == typeof(IDisableable)) is not null;
    
    internal static bool GetIsSnowflake(this Type type)
        => type.GetInterfaces().FirstOrDefault(x => x == typeof(ISnowflakeEntity)) is not null;
    
    internal static Type? GetEntityTypeFromRepoType(this Type type)
        => type.GetGenericArguments().FirstOrDefault();

    internal static bool GetIsCrudFromRepoType(this Type type)
    {
        var generic = type.GetGenericTypeDefinition();
        return generic == typeof(IRepository<,>) || generic == typeof(IRepository<>);
    }
}
