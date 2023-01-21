using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Entities;

namespace DataExplorer.MongoDb;

/// <summary>
/// Represents a MongoDB entity.
/// </summary>
/// <typeparam name="TId">Type of the Id of the entity.</typeparam>
[PublicAPI]
public abstract class MongoEntity<TId> : Entity<TId>, IMongoEntity<TId>, IEquatable<IMongoEntity<TId>> where TId : IComparable, IComparable<TId>, IEquatable<TId>
{
    public virtual string GenerateNewID()
        => ObjectId.GenerateNewId().ToString();
    
    [BsonElement("entityId")]
    public override TId Id { get; protected set; } = default!;

    [BsonElement("createdAt")]
    public override DateTime? CreatedAt { get; set; }

    [BsonElement("updatedAt")]
    public override DateTime? UpdatedAt { get; set; }

    [ObjectId] 
    [BsonId]
    public virtual string? ID { get; set; }
    
    /// <inheritdoc />
    public bool Equals(IMongoEntity<TId>? other)
    {
        if (other is null || ID is null || other.ID is null)
            return false;
        
        if (ReferenceEquals(this, other))
            return true;

        if (ID.Equals(default) || other.ID.Equals(default))
            return false;

        return ID.Equals(other.ID);
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        if (obj is not MongoEntity<TId> other || ID is null || other.ID is null)
            return false;

        if (ReferenceEquals(this, other))
            return true;

        if (ID.Equals(default) || other.ID.Equals(default))
            return false;

        return ID.Equals(other.ID);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static bool operator ==(MongoEntity<TId>? a, MongoEntity<TId>? b)
    {
        if (a  is null && b  is null)
            return true;

        if (a  is null || b  is null)
            return false;

        return a.Equals(b);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static bool operator !=(MongoEntity<TId> a, MongoEntity<TId> b)
    {
        return !(a == b);
    }

    /// <inheritdoc />
    public override int GetHashCode()
        // ReSharper disable once NonReadonlyMemberInGetHashCode
        => (GetType() + ID).GetHashCode();
}

/// <summary>
/// Represents a MongoDB entity.
/// </summary>
[PublicAPI]
public abstract class MongoEntity : MongoEntity<long>
{
}
