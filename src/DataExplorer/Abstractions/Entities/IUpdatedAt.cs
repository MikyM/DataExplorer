namespace DataExplorer.Abstractions.Entities;

/// <summary>
/// Represents an entity with a UpdatedAt property.
/// </summary>
[PublicAPI]
public interface IUpdatedAt
{
    /// <summary>
    /// The last update <see cref="DateTime"/> of the entity.
    /// </summary>
    DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// Represents an entity with a UpdatedAt property.
/// </summary>
[PublicAPI]
public interface IUpdatedAtOffset
{
    /// <summary>
    /// The last update <see cref="DateTimeOffset"/> of the entity.
    /// </summary>
    DateTimeOffset? UpdatedAt { get; set; }
}
