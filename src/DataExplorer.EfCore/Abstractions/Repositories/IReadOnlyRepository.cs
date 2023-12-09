using DataExplorer.EfCore.Specifications.Evaluators;

namespace DataExplorer.EfCore.Abstractions.Repositories;

/// <summary>
/// Read-only repository.
/// </summary>
/// <typeparam name="TEntity">Entity that derives from <see cref="Entity{TId}"/>.</typeparam>
/// <typeparam name="TId">Type of the Id in <typeparamref name="TEntity"/>.</typeparam>
[PublicAPI]
public interface IReadOnlyRepository<TEntity,TId> : IReadOnlyRepositoryBase<TEntity,TId> where TEntity : Entity<TId> where TId : IComparable, IEquatable<TId>, IComparable<TId>
{
    /// <summary>
    /// Current <see cref="IEfDbContext"/>.
    /// </summary>
    new IEfDbContext Context { get; }
    
    /// <summary>
    /// Specification evaluator.
    /// </summary>
    new IEfSpecificationEvaluator SpecificationEvaluator { get; }
    
    /// <summary>
    /// Current <see cref="DbSet{TEntity}"/>.
    /// </summary>
    DbSet<TEntity> Set { get; }
}

/// <summary>
/// Read-only repository.
/// </summary>
/// <typeparam name="TEntity">Entity that derives from <see cref="Entity{TId}"/>.</typeparam>
[PublicAPI]
public interface IReadOnlyRepository<TEntity> : IReadOnlyRepository<TEntity,long> where TEntity : Entity<long>
{
}
