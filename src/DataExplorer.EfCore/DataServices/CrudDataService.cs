using DataExplorer.Abstractions.Repositories;
using DataExplorer.EfCore.Abstractions;
using DataExplorer.EfCore.Abstractions.DataServices;
using DataExplorer.EfCore.Specifications;
using Remora.Results;

// ReSharper disable ClassWithVirtualMembersNeverInherited.Global

namespace DataExplorer.EfCore.DataServices;

/// <summary>
/// CRUD data service.
/// </summary>
/// <inheritdoc cref="ICrudDataService{TEntity,TContext}"/>
[PublicAPI]
public class CrudDataService<TEntity, TId, TContext> : ReadOnlyDataService<TEntity, TId, TContext>,
    ICrudDataService<TEntity, TId, TContext>
    where TEntity : Entity<TId>
    where TContext : class, IEfDbContext
    where TId : IComparable, IEquatable<TId>, IComparable<TId>
{
    /// <summary>
    /// Creates a new instance of <see cref="CrudDataService{TEntity,TId,TContext}"/>.
    /// </summary>
    /// <param name="uof">Unit of work instance.</param>
    public CrudDataService(IUnitOfWork<TContext> uof) : base(uof)
    {
    }

    /// <inheritdoc />
    internal override IRepositoryBase BaseRepositoryInternal => UnitOfWork.GetRepository<IRepository<TEntity, TId>>();

    /// <summary>
    /// Gets the CRUD version of the <see cref="ReadOnlyDataService{TEntity,TId,TContext}.BaseRepository"/> (essentially casts it for you).
    /// </summary>
    protected IRepository<TEntity, TId> Repository => (IRepository<TEntity, TId>)BaseRepository;

    /// <inheritdoc />
    public virtual async Task<Result<TId?>> AddAsync<TPost>(TPost entry, bool shouldSave = false,
        CancellationToken cancellationToken = default)
        where TPost : class
        => await AddAsync(entry, shouldSave, null, cancellationToken).ConfigureAwait(false);

    /// <inheritdoc />
    public async Task<Result<IReadOnlyList<TId>>> AddAsync<TPost>(IEnumerable<TPost> entries, bool shouldSave, string? userId,
        CancellationToken cancellationToken = default) where TPost : class
        => await AddRangeAsync(entries, shouldSave, userId, cancellationToken).ConfigureAwait(false);

    /// <inheritdoc />
    public async Task<Result<IReadOnlyList<TId>>> AddAsync<TPost>(IEnumerable<TPost> entries, bool shouldSave = false, CancellationToken cancellationToken = default) where TPost : class
        => await AddRangeAsync(entries, shouldSave, null, cancellationToken).ConfigureAwait(false);

    /// <inheritdoc />
    public virtual async Task<Result<int>> ExecuteUpdateAsync(IUpdateSpecification<TEntity> specification, CancellationToken cancellationToken = default)
    {
        try
        {
            return await Repository.ExecuteUpdateAsync(specification, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    /// <inheritdoc />
    public virtual async Task<Result<int>> ExecuteDeleteAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await Repository.ExecuteDeleteAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    /// <inheritdoc />
    public virtual async Task<Result<int>> ExecuteDeleteAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
    {
        try
        {
            return await Repository.ExecuteDeleteAsync(specification, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    /// <inheritdoc />
    public virtual async Task<Result<TId?>> AddAsync<TPost>(TPost entry, bool shouldSave, string? userId,
        CancellationToken cancellationToken = default)
        where TPost : class
    {
        try
        {
            TEntity entity;
            if (entry is TEntity rootEntity)
            {
                entity = rootEntity;
                await Repository.AddAsync(entity, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                entity = Mapper.Map<TEntity>(entry);
                await Repository.AddAsync(entity, cancellationToken).ConfigureAwait(false);
            }

            if (!shouldSave)
                return default;

            Result commitResult;
            if (userId is null)
                commitResult = await CommitAsync(cancellationToken).ConfigureAwait(false);
            else
                commitResult = await CommitAsync(userId, cancellationToken).ConfigureAwait(false);
            
            return !commitResult.IsSuccess ? Result<TId?>.FromError(commitResult) : entity.Id;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    /// <inheritdoc />
    public virtual async Task<Result<IReadOnlyList<TId>>> AddRangeAsync<TPost>(IEnumerable<TPost> entries,
        bool shouldSave = false, CancellationToken cancellationToken = default)
        where TPost : class
        => await AddRangeAsync(entries, shouldSave, null, cancellationToken).ConfigureAwait(false);

    /// <inheritdoc />
    public virtual async Task<Result<IReadOnlyList<TId>>> AddRangeAsync<TPost>(IEnumerable<TPost> entries,
        bool shouldSave, string? userId, CancellationToken cancellationToken = default)
        where TPost : class
    {
        try
        {
            List<TEntity> entities;

            if (entries is IEnumerable<TEntity> rootEntities)
            {
                entities = rootEntities.ToList();
                await Repository.AddRangeAsync(entities, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                entities = Mapper.Map<List<TEntity>>(entries);
                await Repository.AddRangeAsync(entities, cancellationToken).ConfigureAwait(false);
            }

            if (!shouldSave)
                return new List<TId>().AsReadOnly();

            Result commitResult;
            if (userId is null)
                commitResult = await CommitAsync(cancellationToken).ConfigureAwait(false);
            else
                commitResult = await CommitAsync(userId, cancellationToken).ConfigureAwait(false);

            return !commitResult.IsSuccess 
                ? Result<IReadOnlyList<TId>>.FromError(commitResult) 
                : Result<IReadOnlyList<TId>>.FromSuccess(entities.Select(e => e.Id).ToList().AsReadOnly());
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    /// <inheritdoc />
    public virtual Result BeginUpdate<TPatch>(TPatch entry, bool shouldSwapAttached = false) where TPatch : class
    {
        try
        {
            switch (entry)
            {
                case null:
                    return new ArgumentNullError(nameof(entry));
                case TEntity rootEntity:
                    Repository.BeginUpdate(rootEntity, shouldSwapAttached);
                    break;
                default:
                    Repository.BeginUpdate(Mapper.Map<TEntity>(entry), shouldSwapAttached);
                    break;
            }

            return Result.FromSuccess();
        }
        catch (Exception ex)
        {
            return new ExceptionError(ex);
        }
    }

    /// <inheritdoc />
    public Result BeginUpdate<TPatch>(IEnumerable<TPatch> entries, bool shouldSwapAttached = false) where TPatch : class
        => BeginUpdateRange(entries, shouldSwapAttached);

    /// <inheritdoc />
    public virtual Result BeginUpdateRange<TPatch>(IEnumerable<TPatch> entries, bool shouldSwapAttached = false)
        where TPatch : class
    {
        try
        {
            switch (entries)
            {
                case null:
                    return new ArgumentNullError(nameof(entries));
                case IEnumerable<TEntity> rootEntities:
                    Repository.BeginUpdateRange(rootEntities, shouldSwapAttached);
                    break;
                default:
                    Repository.BeginUpdateRange(Mapper.Map<IEnumerable<TEntity>>(entries), shouldSwapAttached);
                    break;
            }

            return Result.FromSuccess();
        }
        catch (Exception ex)
        {
            return new ExceptionError(ex);
        }
    }

    /// <inheritdoc />
    public virtual async Task<Result> DeleteAsync<TDelete>(TDelete entry, bool shouldSave = false,
        CancellationToken cancellationToken = default) where TDelete : class
        => await DeleteAsync(entry, shouldSave, null, cancellationToken).ConfigureAwait(false);

    /// <inheritdoc />
    public virtual async Task<Result> DeleteAsync<TDelete>(TDelete entry, bool shouldSave,
        string? userId, CancellationToken cancellationToken = default) where TDelete : class
    {
        try
        {
            switch (entry)
            {
                case null:
                    return new ArgumentNullError(nameof(entry));
                case TEntity rootEntity:
                    Repository.Delete(rootEntity);
                    break;
                default:
                    Repository.Delete(Mapper.Map<TEntity>(entry));
                    break;
            }

            if (!shouldSave)
                return Result.FromSuccess();

            return userId is null 
                ? await CommitAsync(cancellationToken).ConfigureAwait(false)
                : await CommitAsync(userId, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            return new ExceptionError(ex);
        }
    }

    /// <inheritdoc />
    public virtual async Task<Result> DeleteAsync(TId id, bool shouldSave = false,
        CancellationToken cancellationToken = default)
        => await DeleteAsync(id, shouldSave, null, cancellationToken).ConfigureAwait(false);

    /// <inheritdoc />
    public async Task<Result> DeleteAsync<TDelete>(IEnumerable<TDelete> entries, bool shouldSave, CancellationToken cancellationToken = default) where TDelete : class
        => await DeleteRangeAsync(entries, shouldSave, cancellationToken).ConfigureAwait(false);

    /// <inheritdoc />
    public async Task<Result> DeleteAsync(IEnumerable<TId> ids, bool shouldSave, CancellationToken cancellationToken = default)
        => await DeleteRangeAsync(ids, shouldSave, cancellationToken).ConfigureAwait(false);

    /// <inheritdoc />
    public async Task<Result> DeleteAsync<TDelete>(IEnumerable<TDelete> entries, bool shouldSave, string? userId,
        CancellationToken cancellationToken = default) where TDelete : class
        => await DeleteRangeAsync(entries, shouldSave, userId, cancellationToken).ConfigureAwait(false);

    /// <inheritdoc />
    public async Task<Result> DeleteAsync(IEnumerable<TId> ids, bool shouldSave, string? userId, CancellationToken cancellationToken = default)
        => await DeleteRangeAsync(ids, shouldSave, userId, cancellationToken).ConfigureAwait(false);

    /// <inheritdoc />
    public virtual async Task<Result> DeleteAsync(TId id, bool shouldSave, string? userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            Repository.Delete(id);

            if (!shouldSave)
                return Result.FromSuccess();

            return userId is null 
                ? await CommitAsync(cancellationToken).ConfigureAwait(false)
                : await CommitAsync(userId, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            return new ExceptionError(ex);
        }
    }

    /// <inheritdoc />
    public virtual async Task<Result> DeleteRangeAsync(IEnumerable<TId> ids, bool shouldSave = false,
        CancellationToken cancellationToken = default)
        => await DeleteRangeAsync(ids, shouldSave, null, cancellationToken).ConfigureAwait(false);

    /// <inheritdoc />
    public virtual async Task<Result> DeleteRangeAsync(IEnumerable<TId> ids, bool shouldSave,
        string? userId, CancellationToken cancellationToken = default)
    {
        try
        {
            Repository.DeleteRange(ids);

            if (!shouldSave)
                return Result.FromSuccess();

            return userId is null 
                ? await CommitAsync(cancellationToken).ConfigureAwait(false)
                : await CommitAsync(userId, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            return new ExceptionError(ex);
        }
    }

    /// <inheritdoc />
    public virtual async Task<Result> DeleteRangeAsync<TDelete>(IEnumerable<TDelete> entries, bool shouldSave = false,
        CancellationToken cancellationToken = default)
        where TDelete : class
        => await DeleteRangeAsync(entries, shouldSave, null, cancellationToken).ConfigureAwait(false);

    /// <inheritdoc />
    public virtual async Task<Result> DeleteRangeAsync<TDelete>(IEnumerable<TDelete> entries, bool shouldSave,
        string? userId, CancellationToken cancellationToken = default)
        where TDelete : class
    {
        try
        {
            switch (entries)
            {
                case null:
                    throw new ArgumentNullException(nameof(entries));
                case IEnumerable<TEntity> rootEntities:
                    Repository.DeleteRange(rootEntities);
                    break;
                default:
                    Repository.DeleteRange(Mapper.Map<IEnumerable<TEntity>>(entries));
                    break;
            }

            if (!shouldSave)
                return Result.FromSuccess();

            return userId is null 
                ? await CommitAsync(cancellationToken).ConfigureAwait(false)
                : await CommitAsync(userId, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            return new ExceptionError(ex);
        }
    }

    /// <inheritdoc />
    public virtual async Task<Result> DisableAsync(TId id, bool shouldSave = false,
        CancellationToken cancellationToken = default)
        => await DisableAsync(id, shouldSave, null, cancellationToken).ConfigureAwait(false);

    /// <inheritdoc />
    public async Task<Result> DisableAsync<TDisable>(IEnumerable<TDisable> entries, bool shouldSave, string? userId,
        CancellationToken cancellationToken = default) where TDisable : class
        => await DisableRangeAsync(entries, shouldSave, userId, cancellationToken).ConfigureAwait(false);

    /// <inheritdoc />
    public async Task<Result> DisableAsync<TDisable>(IEnumerable<TDisable> entries, bool shouldSave = false,
        CancellationToken cancellationToken = default) where TDisable : class
        => await DisableRangeAsync(entries, shouldSave, cancellationToken).ConfigureAwait(false);

    /// <inheritdoc />
    public async Task<Result> DisableAsync(IEnumerable<TId> ids, bool shouldSave, string? userId, CancellationToken cancellationToken = default)
        => await DisableRangeAsync(ids, shouldSave, userId, cancellationToken).ConfigureAwait(false);

    /// <inheritdoc />
    public async Task<Result> DisableAsync(IEnumerable<TId> ids, bool shouldSave = false, CancellationToken cancellationToken = default)
        => await DisableRangeAsync(ids, shouldSave, cancellationToken).ConfigureAwait(false);

    /// <inheritdoc />
    public virtual async Task<Result> DisableAsync(TId id, bool shouldSave, string? userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await Repository.DisableAsync(id, cancellationToken).ConfigureAwait(false);

            if (!shouldSave)
                return Result.FromSuccess();

            return userId is null 
                ? await CommitAsync(cancellationToken).ConfigureAwait(false)
                : await CommitAsync(userId, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            return new ExceptionError(ex);
        }
    }

    /// <inheritdoc />
    public virtual async Task<Result> DisableAsync<TDisable>(TDisable entry, bool shouldSave = false,
        CancellationToken cancellationToken = default) where TDisable : class
        => await DisableAsync(entry, shouldSave, null, cancellationToken).ConfigureAwait(false);

    /// <inheritdoc />
    public virtual async Task<Result> DisableAsync<TDisable>(TDisable entry, bool shouldSave,
        string? userId, CancellationToken cancellationToken = default) where TDisable : class
    {
        try
        {
            switch (entry)
            {
                case null:
                    return new ArgumentNullError(nameof(entry));
                case TEntity rootEntity:
                    Repository.Disable(rootEntity);
                    break;
                default:
                    Repository.Disable(Mapper.Map<TEntity>(entry));
                    break;
            }

            if (!shouldSave)
                return Result.FromSuccess();

            return userId is null 
                ? await CommitAsync(cancellationToken).ConfigureAwait(false)
                : await CommitAsync(userId, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            return new ExceptionError(ex);
        }
    }

    /// <inheritdoc />
    public virtual async Task<Result> DisableRangeAsync(IEnumerable<TId> ids, bool shouldSave = false,
        CancellationToken cancellationToken = default)
        => await DisableRangeAsync(ids, shouldSave, null, cancellationToken).ConfigureAwait(false);

    /// <inheritdoc />
    public virtual async Task<Result> DisableRangeAsync(IEnumerable<TId> ids, bool shouldSave,
        string? userId, CancellationToken cancellationToken = default)
    {
        try
        {
            await Repository.DisableRangeAsync(ids, cancellationToken);

            if (!shouldSave)
                return Result.FromSuccess();

            return userId is null 
                ? await CommitAsync(cancellationToken).ConfigureAwait(false)
                : await CommitAsync(userId, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            return new ExceptionError(ex);
        }
    }

    /// <inheritdoc />
    public virtual Result Detach<TDetach>(TDetach entry) where TDetach : class
    {
        try
        {
            switch (entry)
            {
                case null:
                    throw new ArgumentNullException(nameof(entry));
                case TEntity rootEntity:
                    Repository.Detach(rootEntity);
                    break;
                default:
                    Repository.Detach(Mapper.Map<TEntity>(entry));
                    break;
            }

            return Result.FromSuccess();
        }
        catch (Exception ex)
        {
            return new ExceptionError(ex);
        }
    }

    /// <inheritdoc />
    public virtual async Task<Result> DisableRangeAsync<TDisable>(IEnumerable<TDisable> entries,
        bool shouldSave = false, CancellationToken cancellationToken = default)
        where TDisable : class
        => await DisableRangeAsync(entries, shouldSave, null, cancellationToken).ConfigureAwait(false);

    /// <inheritdoc />
    public virtual async Task<Result> DisableRangeAsync<TDisable>(IEnumerable<TDisable> entries,
        bool shouldSave, string? userId, CancellationToken cancellationToken = default)
        where TDisable : class
    {
        try
        {
            switch (entries)
            {
                case null:
                    throw new ArgumentNullException(nameof(entries));
                case IEnumerable<TEntity> rootEntities:
                    Repository.DisableRange(rootEntities);
                    break;
                default:
                    Repository.DisableRange(Mapper.Map<IEnumerable<TEntity>>(entries));
                    break;
            }

            if (!shouldSave)
                return Result.FromSuccess();

            return userId is null 
                ? await CommitAsync(cancellationToken).ConfigureAwait(false)
                : await CommitAsync(userId, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            return new ExceptionError(ex);
        }
    }
}

/// <summary>
/// CRUD data service.
/// </summary>
/// <inheritdoc cref="ICrudDataService{TEntity,TContext}"/>
[PublicAPI]
public class CrudDataService<TEntity, TContext> : CrudDataService<TEntity, long, TContext>, ICrudDataService<TEntity, TContext>
    where TEntity : Entity<long> where TContext : class, IEfDbContext
{
    /// <summary>
    /// Creates a new instance of <see cref="CrudDataService{TEntity,TContext}"/>.
    /// </summary>
    /// <param name="uof">Unit of work instance.</param>
    public CrudDataService(IUnitOfWork<TContext> uof) : base(uof)
    {
    }
    
    /// <inheritdoc />
    internal override IRepositoryBase BaseRepositoryInternal => UnitOfWork.GetRepository<IRepository<TEntity>>();
    
    /// <summary>
    /// Gets the CRUD version of the <see cref="ReadOnlyDataService{TEntity,TId,TContext}.BaseRepository"/> (essentially casts it for you).
    /// </summary>
    protected new IRepository<TEntity> Repository => (IRepository<TEntity>)BaseRepository;
    
    /// <summary>
    /// Gets the read-only version of the <see cref="ReadOnlyDataService{TEntity,TId,TContext}.BaseRepository"/> (essentially casts it for you).
    /// </summary>
    protected new IReadOnlyRepository<TEntity> ReadOnlyRepository => (IReadOnlyRepository<TEntity>)BaseRepository;
}
