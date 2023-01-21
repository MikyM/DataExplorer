using MongoDB.Bson.Serialization.Attributes;

namespace DataExplorer.MongoDb;

/// <summary>
/// Represents a MongoDB entity.
/// </summary>
[PublicAPI]
public abstract class MongoEntity : IMongoEntity, IEquatable<IMongoEntity>, IEquatable<MongoEntity>
{
    public virtual string GenerateNewID()
        => ObjectId.GenerateNewId().ToString();

    [ObjectId] 
    [BsonId]
    public virtual string? ID { get; set; }
    
    /// <inheritdoc />
    public bool Equals(IMongoEntity? other)
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
    public bool Equals(MongoEntity? other)
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
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((MongoEntity)obj);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static bool operator ==(MongoEntity? a, MongoEntity? b)
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
    public static bool operator !=(MongoEntity a, MongoEntity b)
    {
        return !(a == b);
    }

    /// <inheritdoc />
    public override int GetHashCode()
        // ReSharper disable once NonReadonlyMemberInGetHashCode
    {
        return (ID != null ? ID.GetHashCode() : 0);
    }
}
