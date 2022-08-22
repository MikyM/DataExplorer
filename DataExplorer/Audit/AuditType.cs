namespace DataExplorer.Audit;

/// <summary>
/// Audit log action type.
/// </summary>
[PublicAPI]
public enum AuditType
{
    None = 0,
    Create = 1,
    Update = 2,
    Disable = 3,
    Delete = 4
}
