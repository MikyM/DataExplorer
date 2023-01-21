using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Entities;

namespace DataExplorer.MongoDb;

/// <summary>
/// Represents a MongoDB entity.
/// </summary>
/// <typeparam name="TId">Type of the Id of the entity.</typeparam>
[PublicAPI]
public abstract class MongoEntity<TId> : Entity<TId>, IMongoEntity<TId> where TId : IComparable, IComparable<TId>, IEquatable<TId>
{
    public string GenerateNewID()
        => ObjectId.GenerateNewId().ToString();
    
    [BsonElement("entityId")]
    public override TId Id { get; protected set; } = default!;

    [BsonElement("createdAt")]
    public override DateTime? CreatedAt { get; set; }

    [BsonElement("updatedAt")]
    public override DateTime? UpdatedAt { get; set; }

    [ObjectId] 
    [BsonId]
    public string? ID 
    {
        get => Id.ToString();
        set => throw new NotSupportedException();
    }
}

/// <summary>
/// Represents a MongoDB entity.
/// </summary>
[PublicAPI]
public abstract class MongoEntity : MongoEntity<long>
{
}
