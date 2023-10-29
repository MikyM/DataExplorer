namespace DataExplorer;

/// <summary>
/// Pagination filter.
/// </summary>
[PublicAPI]
public sealed record PaginationFilter
{
    /// <summary>
    /// Base constructor.
    /// </summary>
    public PaginationFilter()
    {
    }

    /// <summary>
    /// Base constructor.
    /// </summary>
    /// <param name="pageNumber">Page number.</param>
    /// <param name="pageSize">Page size.</param>
    public PaginationFilter(int pageNumber, int pageSize)
    {
        PageNumber = pageNumber < 1 ? 1 : pageNumber;
        PageSize = pageSize > 10 ? 10 : pageSize;
    }

    /// <summary>
    /// Page number.
    /// </summary>
    public int PageNumber { get; init; }
    /// <summary>
    /// Page size.
    /// </summary>
    public int PageSize { get; init; }
}
