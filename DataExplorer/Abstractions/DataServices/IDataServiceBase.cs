using DataExplorer.Abstractions.DataContexts;
using DataExplorer.Abstractions.UnitOfWork;
using Remora.Results;

namespace DataExplorer.Abstractions.DataServices;

/// <summary>
/// Defines a base data service.
/// </summary>
[PublicAPI]
public interface IDataServiceBase<out TContext> : IDisposable where TContext : IDataContextBase
{
    /// <summary>
    /// Current database context.
    /// </summary>
    TContext Context { get; }
    /// <summary>
    /// Current Unit of Work.
    /// </summary>
    IUnitOfWorkBase UnitOfWork { get; }
    /// <summary>
    /// Commits pending changes.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<Result> CommitAsync(CancellationToken cancellationToken = default);
    /// <summary>
    /// Commits pending changes with specifying user that is responsible for them.
    /// </summary>
    /// <param name="auditUserId">Id of the user that's responsible for the changes.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<Result> CommitAsync(string auditUserId, CancellationToken cancellationToken = default);
    /// <summary>
    /// Rolls the current transaction back.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task with a <see cref="Result"/> representing the async operation.</returns>
    Task<Result> RollbackAsync(CancellationToken cancellationToken = default);
}
