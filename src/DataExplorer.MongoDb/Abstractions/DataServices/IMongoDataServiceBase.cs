using AutoMapper;
using DataExplorer.Abstractions.DataServices;
using DataExplorer.MongoDb.Abstractions.DataContexts;

namespace DataExplorer.MongoDb.Abstractions.DataServices;

/// <summary>
/// Base data service for MongoDB.
/// </summary>
/// <typeparam name="TContext">Type that derives from <see cref="MongoDbContext"/>.</typeparam>
[PublicAPI]
public interface IMongoDataServiceBase<out TContext> : IDataServiceBase<TContext> where TContext : class, IMongoDbContext
{
    /// <summary>
    /// Mapper.
    /// </summary>
    IMapper Mapper { get; }
    
    /// <summary>
    /// Current Unit of Work.
    /// </summary>
    new IMongoUnitOfWork<TContext> UnitOfWork { get; }
    
    /// <summary>
    /// Begins a transaction.
    /// </summary>
    /// <returns>Task with a <see cref="Result"/> representing the async operation.</returns>
    Task<Result> BeginTransactionAsync(CancellationToken cancellationToken = default);
}
