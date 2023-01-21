using DataExplorer.Abstractions.Entities;
using IEntity = MongoDB.Entities.IEntity;

namespace DataExplorer.MongoDb;

/// <summary>
/// Represents a MongoDB entity.
/// </summary>
/// <typeparam name="TId">Type of the Id of the entity.</typeparam>
[PublicAPI]
public interface IMongoEntity<TId> : IEntity<TId>, IEntity where TId : IComparable, IComparable<TId>, IEquatable<TId>
{
}

/// <summary>
/// Represents a MongoDB entity.
/// </summary>
[PublicAPI]
public interface IMongoEntity : IMongoEntity<long>
{
}
