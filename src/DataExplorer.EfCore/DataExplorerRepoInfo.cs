namespace DataExplorer.EfCore;

/// <summary>
/// Represents a repository info.
/// </summary>
[PublicAPI]
public sealed record DataExplorerRepoInfo
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    internal DataExplorerRepoInfo(Type repoImplType,
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        Type repoInterface,
        Type idType,
        bool isCrud,
        bool isLongIdVariation)
    {
        RepoImplType = repoImplType;
        RepoInterface = repoInterface;
        IdType = idType;
        IsCrud = isCrud;
        IsLongIdVariation = isLongIdVariation;
    }

    internal void SetEntityInfo(DataExplorerEntityInfo entityInfo)
    {
        EntityInfo = entityInfo;
    }

    /// <summary>
    /// The associated entity info.
    /// </summary>
    public DataExplorerEntityInfo EntityInfo { get; private set; }
    /// <summary>
    /// The repository implementation type.
    /// </summary>
    public Type RepoImplType { get; init; }
    /// <summary>
    /// The repository interface type.
    /// </summary>
    public Type RepoInterface { get; init; }
    /// <summary>
    /// The Id type.
    /// </summary>
    public Type IdType { get; init; }
    /// <summary>
    /// Whether this is a CRUD repository.
    /// </summary>
    public bool IsCrud { get; init; }
    /// <summary>
    /// Whether this is a long Id variation repository, ie. IRepository{T} or IReadOnlyRepository{T}.
    /// </summary>
    public bool IsLongIdVariation { get; init; }
}
