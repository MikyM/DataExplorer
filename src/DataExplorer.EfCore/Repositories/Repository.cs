using AutoMapper;
using DataExplorer.EfCore.Gridify;
using DataExplorer.EfCore.Specifications;
using DataExplorer.Exceptions;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using ISpecificationEvaluator = DataExplorer.EfCore.Specifications.Evaluators.ISpecificationEvaluator;

namespace DataExplorer.EfCore.Repositories;

/// <summary>
/// Repository.
/// </summary>
/// <inheritdoc cref="IRepository{TEntity}"/>
[PublicAPI]
public class Repository<TEntity,TId> : ReadOnlyRepository<TEntity,TId>, IRepository<TEntity,TId>
    where TEntity : Entity<TId> where TId : IComparable, IEquatable<TId>, IComparable<TId>
{
    internal Repository(IEfDbContext context, ISpecificationEvaluator specificationEvaluator, IMapper mapper,
        IGridifyMapperProvider gridifyMapperProvider) : base(context,
        specificationEvaluator, mapper, gridifyMapperProvider)
    {
    }

    /// <inheritdoc />
    public virtual async Task<int> ExecuteDeleteAsync(Specifications.ISpecification<TEntity> specification,
        CancellationToken cancellationToken = default)
        => await ApplySpecification(specification).ExecuteDeleteAsync(cancellationToken).ConfigureAwait(false);
    
    /// <inheritdoc />
    public virtual async Task<int> ExecuteDeleteAsync(CancellationToken cancellationToken = default)
        => await Set.ExecuteDeleteAsync(cancellationToken).ConfigureAwait(false);
    
    /// <inheritdoc />
    public virtual async Task<int> ExecuteUpdateAsync(IUpdateSpecification<TEntity> specification,
        CancellationToken cancellationToken = default)
        => await ApplyUpdateSpecificationAsync<TEntity>(specification).ConfigureAwait(false);

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
    public virtual async Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        => await Set.AddRangeAsync(entities, cancellationToken);
    
    /// <inheritdoc />
    public virtual async Task AddRangeAsync(params TEntity[] entities)
        => await Set.AddRangeAsync(entities);

    /// <inheritdoc />
    public virtual EntityEntry<TEntity> BeginUpdate(TEntity entity, bool shouldSwapAttached = false)
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
    public virtual IReadOnlyList<EntityEntry<TEntity>> BeginUpdateRange(IEnumerable<TEntity> entities, bool shouldSwapAttached = false)
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
    
    /// <inheritdoc />
    public virtual void Delete(TId id)
    {
        var entity = Context.FindTracked<TEntity>(id) ?? (TEntity) Activator.CreateInstance(typeof(TEntity), id)!;
        Set.Remove(entity);
    }

    /// <inheritdoc />
    public virtual void DeleteRange(IEnumerable<TEntity> entities)
        => Set.RemoveRange(entities);

    /// <inheritdoc />
    public virtual void DeleteRange(IEnumerable<TId> ids)
    {
        var entities = ids.Select(id =>
                Context.FindTracked<TEntity>(id) ?? (TEntity) Activator.CreateInstance(typeof(TEntity), id)!)
            .ToList();
        Set.RemoveRange(entities);
    }

    /// <inheritdoc />
    public virtual void Disable(TEntity entity)
    {
        if (entity is not IDisableableEntity)
            throw new InvalidOperationException("Can't disable an entity that isn't disableable.");
        
        BeginUpdate(entity);
        ((IDisableableEntity)entity).IsDisabled = true;
    }

    /// <inheritdoc />
    public virtual async Task DisableAsync(TId id, CancellationToken cancellationToken = default)
    {
        var entity = await GetAsync(new object[] { id }, cancellationToken).ConfigureAwait(false);
        
        if (entity is not IDisableableEntity)
            throw new InvalidOperationException("Can't disable an entity that isn't disableable.");
        
        BeginUpdate(entity ?? throw new NotFoundException());
        ((IDisableableEntity)entity).IsDisabled = true;
    }

    /// <inheritdoc />
    public virtual void DisableRange(IEnumerable<TEntity> entities)
    {
        var list = entities.ToList();
        
        if (list.FirstOrDefault() is not IDisableableEntity)
            throw new InvalidOperationException("Can't disable an entity that isn't disableable.");
        
        BeginUpdateRange(list);
        foreach (var entity in list) 
            ((IDisableableEntity)entity).IsDisabled = true;
    }

    /// <inheritdoc />
    public virtual async Task DisableRangeAsync(IEnumerable<TId> ids, CancellationToken cancellationToken = default)
    {
        var entities = await Set
            .Join(ids, ent => ent.Id, id => id, (ent, id) => ent)
            .ToListAsync(cancellationToken).ConfigureAwait(false);
        
        if (entities.FirstOrDefault() is not IDisableableEntity)
            throw new InvalidOperationException("Can't disable an entity that isn't disableable.");

        BeginUpdateRange(entities);
        entities.ForEach(x => ((IDisableableEntity)x).IsDisabled = true);
    }

    /// <inheritdoc />
    public virtual void Detach(TEntity entity)
    {
        var entry = Context.Entry(entity);
        entry.State = EntityState.Detached;
        
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
                    var list = navs.ToList();

                    if (list.All(x => Context.Entry(x).State is EntityState.Detached))
                        break;

                    foreach (var nav in list.Where(x => Context.Entry(x).State is not EntityState.Detached))
                    {
                        var enumerableEntry = Context.Entry(nav);
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
    
    /// <summary>
    ///     Filters all entities of <typeparamref name="TEntity" />, that matches the encapsulated query logic of the
    ///     <paramref name="specification" />, from the database.
    ///     <para>
    ///         Projects each entity into a new form, being <typeparamref name="TResult" />.
    ///     </para>
    /// </summary>
    /// <typeparam name="TResult">The type of the value returned by the projection.</typeparam>
    /// <param name="specification">The encapsulated query logic.</param>
    /// <returns>The filtered projected entities as an <see cref="IQueryable{T}" />.</returns>
    protected virtual async Task<int> ApplyUpdateSpecificationAsync<TResult>(
        IUpdateSpecification<TEntity> specification)
        => await SpecificationEvaluator.EvaluateUpdateAsync(Set.AsQueryable(), specification).ConfigureAwait(false);
}

/// <summary>
/// Repository.
/// </summary>
/// <inheritdoc cref="IRepository{TEntity}"/>
[PublicAPI]
public class Repository<TEntity> : Repository<TEntity, long>, IRepository<TEntity> where TEntity : Entity<long>
{
    internal Repository(IEfDbContext context, ISpecificationEvaluator specificationEvaluator, IMapper mapper,
        IGridifyMapperProvider gridifyMapperProvider) : base(context, specificationEvaluator, mapper,
        gridifyMapperProvider)
    {
    }
}
