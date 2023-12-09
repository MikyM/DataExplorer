using AutoMapper;
using DataExplorer.Abstractions.DataContexts;
using DataExplorer.Abstractions.UnitOfWork;
using DataExplorer.Gridify;
using Remora.Results;

namespace DataExplorer.Abstractions.DataServices;

/// <summary>
/// Represents a base data service.
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
    /// Mapper.
    /// </summary>
    IMapper Mapper { get; }
    
    /// <summary>
    /// Gridify mapper provider.
    /// </summary>
    IGridifyMapperProvider GridifyMapperProvider { get; }
    
    /// <summary>
    /// Commits pending changes.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<Result> CommitAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Rolls the current transaction back.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task with a <see cref="Result"/> representing the async operation.</returns>
    Task<Result> RollbackAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Wraps a call in a try catch block.
    /// </summary>
    /// <param name="func">Func to wrap.</param>
    /// <param name="shouldConfigureAwaitFalse">Whether to use ConfigureAwait(false).</param>
    /// <typeparam name="TResult">Result.</typeparam>
    /// <returns>Result of the call.</returns>
    Task<Result<TResult>> WrapAsync<TResult>(Func<Task<TResult>> func, bool shouldConfigureAwaitFalse = true);

    /// <summary>
    /// Wraps a call in a try catch block.
    /// </summary>
    /// <param name="func">Func to wrap.</param>
    /// <param name="shouldConfigureAwaitFalse">Whether to use ConfigureAwait(false).</param>
    /// <returns>Result of the call.</returns>
    Task<Result> WrapAsync(Func<Task> func, bool shouldConfigureAwaitFalse = true);
}
