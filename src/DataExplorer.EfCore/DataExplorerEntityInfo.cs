namespace DataExplorer.EfCore;

/// <summary>
/// Represents information about an entity.
/// </summary>
[PublicAPI]
public sealed record DataExplorerEntityInfo
{
    /// <summary>
    /// Represents information about an entity.
    /// </summary>
    /// <param name="entityType">The entity type.</param>
    /// <param name="interfaces">The interfaces.</param>
    /// <param name="idType">The Id type.</param>
    /// <param name="crudGenericIdRepoInfo">The associated CRUD generic Id repo info.</param>
    /// <param name="readOnlyGenericIdRepoInfo">The associated read-only generic Id repo info.</param>
    /// <param name="crudLongIdRepoInfo">The associated CRUD long Id repo info.</param>
    /// <param name="readOnlyLongIdRepoInfo">The associated read-only long Id repo info.</param>
    /// <param name="isDisableable">Whether the entity implements <see cref="IDisableable"/></param>
    /// <param name="isSnowflake">Whether the entity implements <see cref="ISnowflakeEntity{TId}"/></param>
    /// <param name="hasLongId">Whether the entity has Id of type <see cref="long"/></param>
    internal DataExplorerEntityInfo(Type entityType,
        IEnumerable<Type> interfaces,
        Type idType,
        DataExplorerRepoInfo crudGenericIdRepoInfo,
        DataExplorerRepoInfo readOnlyGenericIdRepoInfo,
        DataExplorerRepoInfo? crudLongIdRepoInfo,
        DataExplorerRepoInfo? readOnlyLongIdRepoInfo,
        bool isDisableable,
        bool isSnowflake,
        bool hasLongId)
    {
        EntityType = entityType;
        Interfaces = interfaces;
        IdType = idType;
        CrudGenericIdRepoInfo = crudGenericIdRepoInfo;
        ReadOnlyGenericIdRepoInfo = readOnlyGenericIdRepoInfo;
        CrudLongIdRepoInfo = crudLongIdRepoInfo;
        ReadOnlyLongIdRepoInfo = readOnlyLongIdRepoInfo;
        IsDisableable = isDisableable;
        IsSnowflake = isSnowflake;
        HasLongId = hasLongId;
    }

    /// <summary>The entity type.</summary>
    public Type EntityType { get; init; }

    /// <summary>The interfaces.</summary>
    public IEnumerable<Type> Interfaces { get; init; }

    /// <summary>The Id type.</summary>
    public Type IdType { get; init; }

    /// <summary>The associated CRUD generic Id repo info.</summary>
    public DataExplorerRepoInfo CrudGenericIdRepoInfo { get; init; }

    /// <summary>The associated read-only generic Id repo info.</summary>
    public DataExplorerRepoInfo ReadOnlyGenericIdRepoInfo { get; init; }

    /// <summary>The associated CRUD long Id repo info.</summary>
    public DataExplorerRepoInfo? CrudLongIdRepoInfo { get; init; }

    /// <summary>The associated read-only long Id repo info.</summary>
    public DataExplorerRepoInfo? ReadOnlyLongIdRepoInfo { get; init; }

    /// <summary>Whether the entity implements <see cref="IDisableable"/></summary>
    public bool IsDisableable { get; init; }

    /// <summary>Whether the entity implements <see cref="ISnowflakeEntity{TId}"/></summary>
    public bool IsSnowflake { get; init; }

    /// <summary>Whether the entity has Id of type <see cref="long"/></summary>
    public bool HasLongId { get; init; }
}
