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
