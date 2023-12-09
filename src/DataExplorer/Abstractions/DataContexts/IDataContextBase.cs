namespace DataExplorer.Abstractions.DataContexts;

/// <summary>
/// Represents a base data context.
/// </summary>
[PublicAPI]
public interface IDataContextBase
{
    /// <summary>
    /// Saves the changes made to the data context to the underlying database.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
