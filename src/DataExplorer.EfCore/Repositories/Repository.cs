using System.Linq.Expressions;
using AutoMapper;
using DataExplorer.Abstractions.Specifications;
using DataExplorer.EfCore.Abstractions.Specifications;
using DataExplorer.EfCore.Specifications.Evaluators;
using DataExplorer.Gridify;
using Microsoft.EntityFrameworkCore.ChangeTracking;
// ReSharper disable SuspiciousTypeConversion.Global

namespace DataExplorer.EfCore.Repositories;

/// <summary>
/// Repository.
/// </summary>
/// <inheritdoc cref="IRepository{TEntity}"/>
[PublicAPI]
public class Repository<TEntity,TId> : ReadOnlyRepository<TEntity,TId>, IRepository<TEntity,TId>
    where TEntity : Entity<TId> where TId : IComparable, IEquatable<TId>, IComparable<TId>
{
    internal Repository(IEfDbContext context, IEfSpecificationEvaluator specificationEvaluator, IMapper mapper,
        IGridifyMapperProvider gridifyMapperProvider) : base(context,
        specificationEvaluator, mapper, gridifyMapperProvider)
    {
    }
    
#if NET7_0_OR_GREATER 
    /// <inheritdoc />
    public virtual Task<int> ExecuteDeleteAsync(ISpecification<TEntity> specification,
        CancellationToken cancellationToken = default)
        => ApplySpecification(specification).ExecuteDeleteAsync(cancellationToken);

    /// <inheritdoc />
    public virtual Task<int> ExecuteDeleteAsync(Func<TEntity,bool> predicate,
        CancellationToken cancellationToken = default)
        => Set.Where(predicate).AsQueryable().ExecuteDeleteAsync(cancellationToken);
    
    /// <inheritdoc />
    public virtual Task<int> ExecuteDeleteAsync(CancellationToken cancellationToken = default)
        => Set.ExecuteDeleteAsync(cancellationToken);

    /// <inheritdoc />
    public virtual Task<int> ExecuteUpdateAsync(IUpdateSpecification<TEntity> specification,
        CancellationToken cancellationToken = default)
    {
        var (query, evaluatedCalls) = ApplySpecification(specification);
        return query.ExecuteUpdateAsync(evaluatedCalls, cancellationToken);
    }
#endif

    /// <inheritdoc />
    public virtual void Add(TEntity entity)
        => Set.Add(entity);

    /// <inheritdoc />
    public virtual async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
        => await Set.AddAsync(entity, cancellationToken);

    /// <inheritdoc />
    public virtual void AddRange(IEnumerable<TEntity> entities)
        => Set.AddRange(entities);
    
    /// <inheritdoc />
    public virtual void AddRange(params TEntity[] entities)
        => Set.AddRange(entities);
    
    /// <inheritdoc />
    public virtual Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        => Set.AddRangeAsync(entities, cancellationToken);
    
    /// <inheritdoc />
    public virtual Task AddRangeAsync(params TEntity[] entities)
        => Set.AddRangeAsync(entities);

    /// <inheritdoc />
    void IRepositoryBase<TEntity, TId>.BeginUpdate(TEntity entity, bool shouldSwapAttached)
        => BeginUpdateWithEntityEntry(entity, shouldSwapAttached);

    /// <inheritdoc />
    void IRepositoryBase<TEntity, TId>.BeginUpdateRange(IEnumerable<TEntity> entities, bool shouldSwapAttached)
        => BeginUpdateRangeWithEntityEntries(entities, shouldSwapAttached);

    /// <inheritdoc />
    public virtual EntityEntry<TEntity> BeginUpdateWithEntityEntry(TEntity entity, bool shouldSwapAttached = false)
    {
        var local = Set.Local.FirstOrDefault(entry => entry.Id.Equals(entity.Id));

        if (local is not null && shouldSwapAttached)
        {
            Context.Entry(local).State = EntityState.Detached;
        }
        else if (local is not null && !shouldSwapAttached)
        {
            return Context.Entry(local);
        }

        return Context.Attach(entity);
    }

    /// <inheritdoc />
    public virtual IReadOnlyList<EntityEntry<TEntity>> BeginUpdateRangeWithEntityEntries(IEnumerable<TEntity> entities, bool shouldSwapAttached = false)
    {
        var entries = new List<EntityEntry<TEntity>>();

        foreach (var entity in entities)
        {
            var local = Set.Local.FirstOrDefault(entry => entry.Id.Equals(entity.Id));

            if (local is not null && shouldSwapAttached)
            {
                Context.Entry(local).State = EntityState.Detached;
            }
            else if (local is not null && !shouldSwapAttached)
            {
                entries.Add(Context.Entry(local));
                continue;
            }

            entries.Add(Context.Attach(entity));
        }

        return entries.AsReadOnly();
    }
    
    /// <inheritdoc />
    public virtual void Delete(TEntity entity)
    {
        Set.Remove(entity);
    }

#if NET7_0_OR_GREATER
    /// <inheritdoc />
    public virtual async Task<bool> DeleteAsync(TId id, CancellationToken cancellationToken = default)
    {
        var res = await ExecuteDeleteAsync(x => x.Id.Equals(id), cancellationToken);
        return res == 1;
    }

    /// <inheritdoc />
    public virtual async Task<long> DeleteRangeAsync(IEnumerable<TId> ids, CancellationToken cancellationToken = default)
    {
        var res = await ExecuteDeleteAsync(x => ids.Contains(x.Id), cancellationToken);
        return res;
    }
#endif
    
    /// <inheritdoc />
    public virtual void DeleteRange(IEnumerable<TEntity> entities)
        => Set.RemoveRange(entities);

    /// <inheritdoc />
    public virtual void Disable(TEntity entity)
    {
        if (entity is not IDisableable disableableEntity)
            throw new InvalidOperationException("Can't disable an entity that isn't disableable.");
        
        BeginUpdateWithEntityEntry(entity);
        disableableEntity.IsDisabled = true;
    }

    /// <inheritdoc />
    public virtual void DisableRange(IEnumerable<TEntity> entities)
    {
        var list = entities.ToList();
        
        if (list.FirstOrDefault() is not IDisableable)
            throw new InvalidOperationException("Can't disable an entity that isn't disableable.");
        
        BeginUpdateRangeWithEntityEntries(list);
        foreach (var entity in list) 
            ((IDisableable)entity).IsDisabled = true;
    }

    /// <inheritdoc />
    public virtual void Detach(TEntity entity, bool recursive = false)
    {
        var entry = Context.Entry(entity);
        entry.State = EntityState.Detached;

        if (recursive)
            RecursivelyDetachEntryNavs(entry);
    }

    /// <summary>
    /// Recursively detaches all reachable entities.
    /// </summary>
    /// <param name="entityEntry">Base entry.</param>
    protected virtual void RecursivelyDetachEntryNavs(EntityEntry entityEntry)
    {
        foreach (var entry in entityEntry.Navigations)
        {
            if (entry.CurrentValue is null)
                continue;

            switch (entry.CurrentValue)
            {
                case IEnumerable<IEntityBase> navs:
                {
                    foreach (var nav in navs)
                    {
                        var enumerableEntry = Context.Entry(nav);
                        
                        if (enumerableEntry.State is EntityState.Detached)
                            continue;
                        
                        enumerableEntry.State = EntityState.Detached;
                        
                        RecursivelyDetachEntryNavs(enumerableEntry);
                    }
                    
                    break;
                }
                case IEntityBase nav:
                {
                    var singularEntry = Context.Entry(nav);
                    
                    if (singularEntry.State is EntityState.Detached)
                        break;
                    
                    singularEntry.State = EntityState.Detached;
                    
                    RecursivelyDetachEntryNavs(singularEntry);
                    
                    break;
                }
            }
        }
    }

#if NET7_0_OR_GREATER 
    /// <summary>
    ///     Applies the encapsulated logic by the update specification to the underlying query and returns the evaluated property calls.
    /// </summary>
    /// <param name="specification">The encapsulated query logic.</param>
    /// <returns>The filtered query after applying the specification and evaluated property calls.</returns>
    protected virtual (IQueryable<TEntity> Query, Expression<Func<SetPropertyCalls<TEntity>, SetPropertyCalls<TEntity>>>
        EvaluatedCalls) ApplySpecification(IUpdateSpecification<TEntity> specification)
        => SpecificationEvaluator.GetQuery(Set.AsQueryable(), specification); 
#endif
}

/// <summary>
/// Repository.
/// </summary>
/// <inheritdoc cref="IRepository{TEntity}"/>
[PublicAPI]
public class Repository<TEntity> : Repository<TEntity, long>, IRepository<TEntity> where TEntity : Entity<long>
{
    internal Repository(IEfDbContext context, IEfSpecificationEvaluator specificationEvaluator, IMapper mapper,
        IGridifyMapperProvider gridifyMapperProvider) : base(context, specificationEvaluator, mapper,
        gridifyMapperProvider)
    {
    }
}
