using DataExplorer.Abstractions.Entities;
using IEntity = MongoDB.Entities.IEntity;

namespace DataExplorer.MongoDb;

/// <summary>
/// Represents a MongoDB entity.
/// </summary>
[PublicAPI]
public interface IMongoEntity : IEntity, IDataExplorerEntity
{
}
