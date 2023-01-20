namespace DataExplorer.Abstractions.Entities;

/// <summary>
/// Represents an entity that is soft deleted.
/// </summary>
[PublicAPI]
public interface IDisableableEntity
{
    /// <summary>
    /// Whether this entity is currently disabled.
    /// </summary>
    bool IsDisabled { get; set; }
}
