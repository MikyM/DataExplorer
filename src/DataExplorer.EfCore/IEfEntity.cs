namespace DataExplorer.EfCore;

/// <summary>
/// Represents an EF entity.
/// </summary>
/// <typeparam name="TId">Type of the Id of the entity.</typeparam>
[PublicAPI]
public interface IEfEntity<TId> : IEntity<TId> where TId : IComparable, IComparable<TId>, IEquatable<TId>
{
}

/// <summary>
/// Represents an EF entity.
/// </summary>
[PublicAPI]
public interface IEfEntity : IEfEntity<long>
{
}
