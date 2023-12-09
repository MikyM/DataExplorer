using DataExplorer.Abstractions.DataServices;

namespace DataExplorer.EfCore.Abstractions.DataServices;

/// <summary>
/// Read-only data service.
/// </summary>
/// <typeparam name="TEntity">Type of the entity to create the service for, must derive from <see cref="IEntity"/>.</typeparam>
/// <typeparam name="TContext">Type of the <see cref="DbContext"/> to use.</typeparam>
/// <typeparam name="TId">Type of the Id of the entity.</typeparam>
[PublicAPI]
public interface IReadOnlyDataService<TEntity, TId, out TContext> : IEfCoreDataServiceBase<TContext>, IReadOnlyDataServiceBase<TEntity, TId, TContext>
    where TEntity : Entity<TId>
    where TContext : class, IEfDbContext
    where TId : IComparable, IEquatable<TId>, IComparable<TId>
{
    /// <summary>
    /// The underlying repository.
    /// </summary>
    new IReadOnlyRepository<TEntity,TId> ReadOnlyRepository { get; }
}

/// <summary>
/// Read-only data service.
/// </summary>
/// <typeparam name="TEntity">Type of the entity to create the service for, must derive from <see cref="IEntity{TId}"/>.</typeparam>
/// <typeparam name="TContext">Type of the <see cref="DbContext"/> to use.</typeparam>
[PublicAPI]
public interface IReadOnlyDataService<TEntity, out TContext> : IReadOnlyDataService<TEntity, long, TContext>
    where TEntity : Entity<long> where TContext : class, IEfDbContext
{
}
