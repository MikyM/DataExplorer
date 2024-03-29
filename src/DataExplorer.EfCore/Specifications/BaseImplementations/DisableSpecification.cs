﻿// ReSharper disable VirtualMemberCallInConstructor
#if NET7_0_OR_GREATER
// ReSharper disable once CheckNamespace
namespace DataExplorer.EfCore.Specifications;

/// <summary>
/// A helper specification used to disable entities implementing <see cref="IDisableable"/>.
/// </summary>
/// <typeparam name="TEntity">The entity type.</typeparam>
/// <typeparam name="TId">The Id type.</typeparam>
[PublicAPI]
public class DisableSpecification<TEntity, TId> : UpdateSpecification<TEntity>
    where TEntity : class, IDisableable, IEntity<TId> where TId : IComparable, IEquatable<TId>, IComparable<TId>
{
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="id">The id.</param>
    public DisableSpecification(TId id)
    {
        Where(x => x.Id.Equals(id));
        Modify(x => x.SetProperty(p => p.IsDisabled, true));
    }
    
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="ids">The ids.</param>
    public DisableSpecification(IEnumerable<TId> ids)
    {
        Where(x => ids.Contains(x.Id));
        Modify(x => x.SetProperty(p => p.IsDisabled, true));
    }
}

/// <summary>
/// A helper specification used to disable entities implementing <see cref="IDisableable"/>.
/// </summary>
/// <typeparam name="TEntity">The entity type.</typeparam>
[PublicAPI]
public sealed class DisableSpecification<TEntity> : DisableSpecification<TEntity, long>
    where TEntity : class, IDisableable, IEntity<long>
{
    public DisableSpecification(long id) : base(id)
    {
    }
    public DisableSpecification(IEnumerable<long> ids) : base(ids)
    {
    }
}

#endif
