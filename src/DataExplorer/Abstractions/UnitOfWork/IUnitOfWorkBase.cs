using AutoMapper;
using DataExplorer.Abstractions.DataContexts;
using DataExplorer.Abstractions.Repositories;
using DataExplorer.Abstractions.Specifications.Evaluators;
using DataExplorer.Gridify;

namespace DataExplorer.Abstractions.UnitOfWork;

/// <summary>
/// Represents a base Unit of Work.
/// </summary>
/// <remarks>This also works as a factory for <see cref="IBaseRepository"/>.</remarks>
[PublicAPI]
public interface IUnitOfWorkBase : IDisposable
{
    /// <summary>
    /// Gets a repository of a given type.
    /// </summary>
    /// <typeparam name="TRepository">Type of the repository to get.</typeparam>
    /// <returns>Wanted repository</returns>
    TRepository GetRepository<TRepository>() where TRepository : class, IBaseRepository;

    /// <summary>
    /// Commits pending changes to the underlying database.
    /// </summary>
    /// <returns>Number of affected rows.</returns>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task CommitAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Rolls the current transaction back.
    /// </summary>
    /// <returns>Task representing the asynchronous operation.</returns>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task RollbackAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// The current data context.
    /// </summary>
    IDataContextBase Context { get; }
    
    /// <summary>
    /// Mapper instance.
    /// </summary>
    IMapper Mapper { get; }
    
    /// <summary>
    /// Specification evaluator instance.
    /// </summary>
    ISpecificationEvaluator SpecificationEvaluator { get; }
    
    /// <summary>
    /// Gridify mapper provider instance.
    /// </summary>
    IGridifyMapperProvider GridifyMapperProvider { get; }
}
