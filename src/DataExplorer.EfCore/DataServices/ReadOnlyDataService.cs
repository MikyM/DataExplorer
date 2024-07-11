using System.Linq.Expressions;
using DataExplorer.Abstractions.DataServices;
using DataExplorer.Abstractions.Specifications;
using DataExplorer.EfCore.Abstractions.DataServices;
using Remora.Results;

namespace DataExplorer.EfCore.DataServices;

/// <summary>
/// Read-only data service.
/// </summary>
/// <inheritdoc cref="IReadOnlyDataService{TEntity,TContext}"/>
[PublicAPI]
public class ReadOnlyDataService<TEntity, TId, TContext> : EfCoreDataServiceBase<TContext>,
    IReadOnlyDataService<TEntity, TId, TContext>
    where TEntity : Entity<TId>
    where TContext : class, IEfDbContext
    where TId : IComparable, IEquatable<TId>, IComparable<TId>
{
    /// <summary>
    /// Creates a new instance of <see cref="IReadOnlyDataService{TEntity,TId,TContext}"/>.
    /// </summary>
    /// <param name="uof">Instance of <see cref="IUnitOfWork"/>.</param>
    public ReadOnlyDataService(IUnitOfWork<TContext> uof) : base(uof)
    {
    }

    /// <summary>
    /// Gets the base repository for this data service.
    /// </summary>
    internal virtual IBaseRepository BaseRepositoryInternal => UnitOfWork.GetRepository<IReadOnlyRepository<TEntity,TId>>();
    
    /// <inheritdoc />
    IReadOnlyRepositoryBase<TEntity, TId> IReadOnlyDataServiceBase<TEntity, TId, TContext>.ReadOnlyRepository => ReadOnlyRepository;
    
    /// <summary>
    /// Gets the base repository for this data service.
    /// </summary>
    protected IBaseRepository BaseRepository => BaseRepositoryInternal;
    
    /// <summary>
    /// Gets the read-only version of the <see cref="BaseRepository"/> (essentially casts it for you).
    /// </summary>
    public IReadOnlyRepository<TEntity,TId> ReadOnlyRepository =>
        (IReadOnlyRepository<TEntity,TId>)BaseRepository;
    

    /// <inheritdoc />
    public virtual async Task<Result<TEntity>> GetAsync(params object[] keyValues)
    {
        try
        {
            var entity = await ReadOnlyRepository.GetAsync(keyValues);
            return entity is null ? new NotFoundError() : entity;
        }
        catch (Exception ex)
        {
            return new ExceptionError(ex);
        }
    }

    /// <inheritdoc />
    public virtual async Task<Result<TGetResult>> GetAsync<TGetResult>(object?[]? keyValues, CancellationToken cancellationToken = default) where TGetResult : class
    {       
        try
        {
            var entity = await ReadOnlyRepository.GetAsync(keyValues, cancellationToken);
            if (entity is null)
                return new NotFoundError();
            
            return Mapper.Map<TGetResult>(entity);
        }
        catch (Exception ex)
        {
            return new ExceptionError(ex);
        }
    }

    /// <inheritdoc />
    public virtual async Task<Result<TEntity>> GetAsync(object?[]? keyValues, CancellationToken cancellationToken)
    {       
        try
        {
            var entity = await ReadOnlyRepository.GetAsync(keyValues, cancellationToken);
            return entity is null ? new NotFoundError() : entity;
        }
        catch (Exception ex)
        {
            return new ExceptionError(ex);
        }
    }

    /// <inheritdoc />
    public Result<IAsyncEnumerable<TEntity>> AsAsyncEnumerable(ISpecification<TEntity> specification)
    {
        try
        {
            return Result<IAsyncEnumerable<TEntity>>.FromSuccess(ReadOnlyRepository.AsAsyncEnumerable(specification));
        }
        catch (Exception ex)
        {
            return new ExceptionError(ex);
        }
    }

    /// <inheritdoc />
    public virtual async Task<Result<TEntity>> GetSingleBySpecAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
    {
        try
        {
            var entity = await ReadOnlyRepository.GetSingleBySpecAsync(specification, cancellationToken);
            return entity is null ? new NotFoundError() : entity;
        }
        catch (Exception ex)
        {
            return new ExceptionError(ex);
        }
    }

    /// <inheritdoc />
    public virtual async Task<Result<TGetResult>> GetSingleBySpecAsync<TGetResult>(ISpecification<TEntity> specification, CancellationToken cancellationToken = default) where TGetResult : class
    {
        try
        {
            var res = await GetSingleBySpecAsync(specification, cancellationToken);
            return !res.IsDefined(out var entity) ? Result<TGetResult>.FromError(res) : Mapper.Map<TGetResult>(entity);
        }
        catch (Exception ex)
        {
            return new ExceptionError(ex);
        }
    }

    /// <inheritdoc />
    public virtual async Task<Result<TGetProjectedResult>> GetSingleBySpecAsync<TGetProjectedResult>(ISpecification<TEntity, TGetProjectedResult> specification, CancellationToken cancellationToken = default)
    {
        try
        {
            var entity = await ReadOnlyRepository.GetSingleBySpecAsync(specification, cancellationToken);
            return entity is null ? new NotFoundError() : entity;
        }
        catch (Exception ex)
        {
            return new ExceptionError(ex);
        }
    }
    
    /// <inheritdoc />
    public virtual Task<Result<TEntity>> GetSingleAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
        => GetSingleBySpecAsync(specification, cancellationToken);

    /// <inheritdoc />
    public virtual Task<Result<TGetResult>> GetSingleAsync<TGetResult>(ISpecification<TEntity> specification, CancellationToken cancellationToken = default) where TGetResult : class
        => GetSingleBySpecAsync<TGetResult>(specification, cancellationToken);

    /// <inheritdoc />
    public virtual Task<Result<TGetProjectedResult>> GetSingleAsync<TGetProjectedResult>(
        ISpecification<TEntity, TGetProjectedResult> specification, CancellationToken cancellationToken = default)
        => GetSingleBySpecAsync(specification, cancellationToken);

    /// <inheritdoc />
    public virtual async Task<Result<IReadOnlyList<TEntity>>> GetBySpecAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
    {
        try
        {
            return Result<IReadOnlyList<TEntity>>.FromSuccess(await ReadOnlyRepository.GetBySpecAsync(specification, cancellationToken));
        }
        catch (Exception ex)
        {
            return new ExceptionError(ex);
        }
    }

    /// <inheritdoc />
    public virtual async Task<Result<IReadOnlyList<TGetResult>>> GetBySpecAsync<TGetResult>(ISpecification<TEntity> specification, CancellationToken cancellationToken = default) where TGetResult : class
    {
        try
        {
            var res = await GetBySpecAsync(specification, cancellationToken);
            return !res.IsDefined(out var def)
                ? Result<IReadOnlyList<TGetResult>>.FromError(res)
                : Result<IReadOnlyList<TGetResult>>.FromSuccess(Mapper.Map<IReadOnlyList<TGetResult>>(def));
        }
        catch (Exception ex)
        {
            return new ExceptionError(ex);
        }
    }

    /// <inheritdoc />
    public virtual Task<Result<IReadOnlyList<TGetProjectedResult>>> GetBySpecAsync<TGetProjectedResult>(ISpecification<TEntity, TGetProjectedResult> specification, CancellationToken cancellationToken = default)
        => WrapAsync(() => ReadOnlyRepository.GetBySpecAsync(specification, cancellationToken));

    /// <inheritdoc />
    public virtual Task<Result<IReadOnlyList<TEntity>>> GetAsync(ISpecification<TEntity> specification,
        CancellationToken cancellationToken = default)
        => GetBySpecAsync(specification, cancellationToken);

    /// <inheritdoc />
    public virtual Task<Result<IReadOnlyList<TGetResult>>> GetAsync<TGetResult>(ISpecification<TEntity> specification, CancellationToken cancellationToken = default) where TGetResult : class
        => GetBySpecAsync<TGetResult>(specification, cancellationToken);


    /// <inheritdoc />
    public virtual Task<Result<IReadOnlyList<TGetProjectedResult>>> GetAsync<TGetProjectedResult>(
        ISpecification<TEntity, TGetProjectedResult> specification, CancellationToken cancellationToken = default)
        => GetBySpecAsync(specification, cancellationToken);

    /// <inheritdoc />
    public virtual async Task<Result<IReadOnlyList<TGetResult>>> GetAllAsync<TGetResult>(bool shouldProject = false,
        CancellationToken cancellationToken = default)
        where TGetResult : class
    {
        try
        {
            if (shouldProject)
                return Result<IReadOnlyList<TGetResult>>.FromSuccess(await ReadOnlyRepository.GetAllAsync<TGetResult>(cancellationToken));

            return Result<IReadOnlyList<TGetResult>>.FromSuccess(Mapper.Map<IReadOnlyList<TGetResult>>(
                await ReadOnlyRepository.GetAllAsync(cancellationToken)));
        }
        catch (Exception ex)
        {
            return new ExceptionError(ex);
        }
    }

    /// <inheritdoc />
    public virtual async Task<Result<IReadOnlyList<TEntity>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return Result<IReadOnlyList<TEntity>>.FromSuccess(await ReadOnlyRepository.GetAllAsync(cancellationToken));
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    /// <inheritdoc />
    public virtual async Task<Result<long>> LongCountAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
    {
        try
        {
            return Result<long>.FromSuccess(await ReadOnlyRepository.LongCountAsync(specification, cancellationToken));
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    /// <inheritdoc />
    public virtual async Task<Result<long>> LongCountAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return Result<long>.FromSuccess(await ReadOnlyRepository.LongCountAsync(cancellationToken));
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    /// <inheritdoc />
    public async Task<Result<bool>> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        try
        {
            return Result<bool>.FromSuccess(await ReadOnlyRepository.AnyAsync(predicate, cancellationToken));
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    /// <inheritdoc />
    public async Task<Result<bool>> AnyAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
    {
        try
        {
            return Result<bool>.FromSuccess(await ReadOnlyRepository.AnyAsync(specification, cancellationToken));
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
}

/// <summary>
/// Read-only data service.
/// </summary>
/// <inheritdoc cref="IReadOnlyDataService{TEntity,TContext}"/>
[PublicAPI]
public class ReadOnlyDataService<TEntity, TContext> : ReadOnlyDataService<TEntity, long, TContext>, IReadOnlyDataService<TEntity, TContext>
    where TEntity : Entity<long>
    where TContext : class, IEfDbContext
{
    /// <summary>
    /// Creates a new instance of <see cref="IReadOnlyDataService{TEntity,TContext}"/>.
    /// </summary>
    /// <param name="uof">Instance of <see cref="IUnitOfWork"/>.</param>
    public ReadOnlyDataService(IUnitOfWork<TContext> uof) : base(uof)
    {
    }
    
    /// <inheritdoc />
    internal override IBaseRepository BaseRepositoryInternal => UnitOfWork.GetRepository<IReadOnlyRepository<TEntity>>();
}
