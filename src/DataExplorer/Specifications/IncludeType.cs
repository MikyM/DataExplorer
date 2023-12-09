namespace DataExplorer.Specifications;

/// <summary>
/// Defines the type of include.
/// </summary>
[PublicAPI]
public enum IncludeType
{
    /// <summary>
    /// Direct include.
    /// </summary>
    Include = 1,
    /// <summary>
    /// Then include.
    /// </summary>
    ThenInclude = 2
}
