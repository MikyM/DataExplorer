using AutoMapper;
using DataExplorer.Exceptions;
using DataExplorer.MongoDb.Abstractions.DataContexts;
using MongoDB.Entities;

namespace DataExplorer.MongoDb.Repositories;

/// <summary>
/// Repository.
/// </summary>
/// <inheritdoc cref="IMongoRepository{TEntity}"/>
[PublicAPI]
public class MongoRepository<TEntity,TId> : MongoReadOnlyRepository<TEntity,TId>, IMongoRepository<TEntity,TId>
    where TEntity : Entity<TId>, IEntity where TId : IComparable, IEquatable<TId>, IComparable<TId>
{
    internal MongoRepository(IMongoDbContext context, /*ISpecificationEvaluator specificationEvaluator,*/ IMapper mapper) : base(context,
        /*specificationEvaluator,*/ mapper)
    {
    }
    
    /*
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
    }*/
    public Task DisableAsync(TId id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public void DisableRange(IEnumerable<TEntity> entities)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Repository.
/// </summary>
/// <inheritdoc cref="IMongoRepository{TEntity}"/>
[PublicAPI]
public class MongoRepository<TEntity> : MongoRepository<TEntity, long>, IMongoRepository<TEntity> where TEntity : Entity<long>, IEntity
{
    internal MongoRepository(IMongoDbContext context, /*ISpecificationEvaluator specificationEvaluator,*/ IMapper mapper) : base(context, mapper)
    {
    }
}
