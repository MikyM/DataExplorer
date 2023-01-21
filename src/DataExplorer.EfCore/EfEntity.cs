namespace DataExplorer.EfCore;

/// <summary>
/// Represents an EF entity.
/// </summary>
/// <typeparam name="TId">Type of the Id of the entity.</typeparam>
[PublicAPI]
public abstract class EfEntity<TId> : Entity<TId>, IEfEntity<TId> where TId : IComparable, IComparable<TId>, IEquatable<TId>
{
}

/// <summary>
/// Represents an EF entity.
/// </summary>
[PublicAPI]
public abstract class EfEntity : EfEntity<long>, IEfEntity
{
}
