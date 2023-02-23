using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using DataExplorer.Abstractions.Entities;
// ReSharper disable VirtualMemberCallInConstructor

namespace DataExplorer.Entities;

/// <summary>
/// Represents a base entity with <see cref="long"/> as Id.
/// </summary>
[PublicAPI]
public abstract class Entity : Entity<long>, IEntity
{
    /// <summary>
    /// Base entity constructor.
    /// </summary>
    protected Entity()
    {
    }

    /// <summary>
    /// Base entity constructor.
    /// </summary>
    protected Entity(long id)
        : base(id)
    {
    }
}

/// <summary>
/// Represents a generic base entity.
/// </summary>
public abstract class Entity<TId> : EntityBase, IEntity<TId>, IEquatable<IEntity<TId>> where TId : IComparable, IComparable<TId>, IEquatable<TId>
{
    /// <summary>
    /// Base entity constructor.
    /// </summary>
    protected Entity()
    {
    }

    /// <summary>
    /// Base entity constructor.
    /// </summary>
    protected Entity(TId id)
    {
        Id = id;
    }

    /// <inheritdoc />
    public sealed override void SetId(object id)
        => SetId((TId)id);
    
    /// <inheritdoc />
    public virtual TId Id { get; protected set; } = default!;

    /// <summary>
    /// Returns the string representation of the Id of this entity.
    /// </summary>
    /// <returns>The string representation of the Id of this entity.</returns>
    public override string? ToString()
        => Id.ToString();
    
    /// <inheritdoc />
    public virtual void SetId(TId id)
        => Id = id;

    /// <inheritdoc />
    [JsonIgnore][XmlIgnore]
    public sealed override bool HasValidId 
        => !Id.Equals(default);

    /// <inheritdoc />
    public bool Equals(IEntity<TId>? other)
    {
        if (other is null)
            return false;
        
        if (ReferenceEquals(this, other))
            return true;

        if (GetUnproxiedType(this) != GetUnproxiedType(other))
            return false;

        if (Id.Equals(default) || other.Id.Equals(default))
            return false;

        return Id.Equals(other.Id);
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        if (obj is not Entity<TId> other)
            return false;

        if (ReferenceEquals(this, other))
            return true;

        if (GetUnproxiedType(this) != GetUnproxiedType(other))
            return false;

        if (Id.Equals(default) || other.Id.Equals(default))
            return false;

        return Id.Equals(other.Id);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static bool operator ==(Entity<TId>? a, Entity<TId>? b)
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
    public static bool operator !=(Entity<TId> a, Entity<TId> b)
    {
        return !(a == b);
    }

    /// <inheritdoc />
    public override int GetHashCode()
        // ReSharper disable once NonReadonlyMemberInGetHashCode
        => (GetUnproxiedType(this).ToString() + Id).GetHashCode();


    private static Type GetUnproxiedType(object? obj)
    {
        const string efCoreProxyPrefix = "Castle.Proxies.";
        const string nHibernateProxyPostfix = "Proxy";

        var type = obj!.GetType();
        var typeString = type.ToString();

        if (typeString.Contains(efCoreProxyPrefix) || typeString.EndsWith(nHibernateProxyPostfix))
            return type.BaseType!;

        return type;
    }
}

/// <inheritdoc />
[PublicAPI]
public abstract class EntityBase : IEntityBase
{
    /// <inheritdoc />
    public abstract bool HasValidId { get; }

    /// <inheritdoc />
    public abstract void SetId(object id);
}
