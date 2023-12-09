namespace DataExplorer.Specifications;

/// <summary>
/// Defines the type of order.
/// </summary>
[PublicAPI]
public enum OrderType
{
    /// <summary>
    /// Order by.
    /// </summary>
    OrderBy = 1,
    /// <summary>
    /// Order by descending.
    /// </summary>
    OrderByDescending = 2,
    /// <summary>
    /// Then by.
    /// </summary>
    ThenBy = 3,
    /// <summary>
    /// Then by descending.
    /// </summary>
    ThenByDescending = 4
}
