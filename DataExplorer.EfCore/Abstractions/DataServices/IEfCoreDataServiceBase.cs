using AutoMapper;
using DataExplorer.Abstractions.DataServices;
using DataExplorer.EfCore.Gridify;
using Remora.Results;

namespace DataExplorer.EfCore.Abstractions.DataServices;

/// <summary>
/// Base data service for Entity Framework Core.
/// </summary>
/// <typeparam name="TContext">Type that derives from <see cref="DbContext"/>.</typeparam>
[PublicAPI]
public interface IEfCoreDataServiceBase<out TContext> : IDataServiceBase<TContext> where TContext : class, IEfDbContext
{
    /// <summary>
    /// Mapper.
    /// </summary>
    IMapper Mapper { get; }
    /// <summary>
    /// Gridify mapper provider.
    /// </summary>
    IGridifyMapperProvider GridifyMapperProvider { get; }
    /// <summary>
    /// Current Unit of Work.
    /// </summary>
    new IUnitOfWork<TContext> UnitOfWork { get; }
    /// <summary>
    /// Begins a transaction.
    /// </summary>
    /// <returns>Task with a <see cref="Result"/> representing the async operation.</returns>
    Task<Result> BeginTransactionAsync(CancellationToken cancellationToken = default);

    /// <inheritdoc cref="IDataServiceBase{TContext}.CommitAsync(string,CancellationToken)"/>
    /// <returns>Number of affected rows.</returns>
    Task<Result<int>> CommitWithCountAsync(string auditUserId, CancellationToken cancellationToken = default);

    /// <inheritdoc cref="IDataServiceBase{TContext}.CommitAsync(CancellationToken)"/>
    /// <returns>Number of affected rows.</returns>
    Task<Result<int>> CommitWithCountAsync(CancellationToken cancellationToken = default);
}
