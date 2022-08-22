namespace DataExplorer.Abstractions.Entities;

/// <summary>
/// Defines an entity that is soft deleted.
/// </summary>
[PublicAPI]
public interface IDisableableEntity
{
    /// <summary>
    /// Whether the entity is currently disabled.
    /// </summary>
    bool IsDisabled { get; set; }
}
