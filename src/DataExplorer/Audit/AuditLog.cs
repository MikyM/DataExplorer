using DataExplorer.Entities;

namespace DataExplorer.Audit;

/// <summary>
/// Audit log entity.
/// </summary>
[PublicAPI]
public class AuditLog : SnowflakeEntity
{
    /// <summary>
    /// Id of the user responsible for the changes.
    /// </summary>
    public string UserId { get; set; } = null!;

    /// <summary>
    /// Type of the action.
    /// </summary>
    public string Type { get; set; } = null!;

    /// <summary>
    /// Name of the table affected by the changes.
    /// </summary>
    public string TableName { get; set; } = null!;

    /// <summary>
    /// Previous values.
    /// </summary>
    public string? OldValues { get; set; }

    /// <summary>
    /// New values.
    /// </summary>
    public string? NewValues { get; set; }

    /// <summary>
    /// Affected columns.
    /// </summary>
    public string? AffectedColumns { get; set; }

    /// <summary>
    /// Primary key.
    /// </summary>
    public string PrimaryKey { get; set; } = null!;
}
