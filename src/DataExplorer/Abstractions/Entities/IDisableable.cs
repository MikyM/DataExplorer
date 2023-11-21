namespace DataExplorer.Abstractions.Entities;

/// <summary>
/// Represents an entity that is soft deleted.
/// </summary>
[PublicAPI]
public interface IDisableable
{
    /// <summary>
    /// Whether this entity is currently disabled.
    /// </summary>
    bool IsDisabled { get; set; }
}
