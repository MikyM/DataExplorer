namespace DataExplorer.Abstractions.Entities;

/// <summary>
/// Represents an entity with a CreatedAt property.
/// </summary>
[PublicAPI]
public interface ICreatedAt
{
    /// <summary>
    /// The creation <see cref="DateTime"/> of the entity.
    /// </summary>
    DateTime? CreatedAt { get; set; }
}

/// <summary>
/// Represents an entity with a CreatedAt property.
/// </summary>
[PublicAPI]
public interface ICreatedAtOffset
{
    /// <summary>
    /// The creation <see cref="DateTimeOffset"/> of the entity.
    /// </summary>
    DateTimeOffset? CreatedAt { get; set; }
}
