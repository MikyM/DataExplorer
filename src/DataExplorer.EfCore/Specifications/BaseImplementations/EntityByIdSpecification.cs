// ReSharper disable once CheckNamespace
namespace DataExplorer.EfCore.Specifications;

/// <summary>
/// A helper specification used to fetch entities by their Id.
/// </summary>
/// <typeparam name="TEntity">The entity type.</typeparam>
/// <typeparam name="TId">The Id type.</typeparam>
[PublicAPI]
public sealed class EntityByIdSpecification<TEntity, TId> : Specification<TEntity>
    where TEntity : class, IEntity<TId> where TId : IComparable, IEquatable<TId>, IComparable<TId>
{
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="id">The id.</param>
    public EntityByIdSpecification(TId id)
    {
        Where(x => x.Id.Equals(id));
    }
    
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="ids">The ids.</param>
    public EntityByIdSpecification(IEnumerable<TId> ids)
    {
        Where(x => ids.Contains(x.Id));
    }
}
