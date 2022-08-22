using DataExplorer.Abstractions.Entities;
// ReSharper disable VirtualMemberCallInConstructor

namespace DataExplorer.Entities;

/// <summary>
/// Defines a base entity with <see cref="long"/> as Id.
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
/// Defines a generic base entity.
/// </summary>
public abstract class Entity<TId> : EntityBase, IEntity<TId>, IEquatable<Entity<TId>> where TId : IComparable, IComparable<TId>, IEquatable<TId>
{
    /// <summary>
    /// Base entity constructor.
    /// </summary>
    protected Entity()
    {
        CreatedAt ??= DateTime.UtcNow;
        UpdatedAt ??= CreatedAt;
        Id = default!;
    }

    /// <summary>
    /// Base entity constructor.
    /// </summary>
    protected Entity(TId id)
    {
        CreatedAt ??= DateTime.UtcNow;
        UpdatedAt ??= CreatedAt;
        Id = id;
    }


    /// <inheritdoc />
    public virtual TId Id { get; protected set; }

    /// <inheritdoc />
    public virtual DateTime? CreatedAt { get; set; }

    /// <inheritdoc />
    public virtual DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Returns the string representation of the Id of this entity.
    /// </summary>
    /// <returns>The string representation of the Id of this entity.</returns>
    public override string? ToString()
        => Id.ToString();

    /// <inheritdoc />
    public bool Equals(Entity<TId>? other)
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
}
