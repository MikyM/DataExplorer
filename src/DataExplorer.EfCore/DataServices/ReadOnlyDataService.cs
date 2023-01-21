using System.Linq.Expressions;
using DataExplorer.Abstractions.Repositories;
using DataExplorer.EfCore.Abstractions;
using DataExplorer.EfCore.Abstractions.DataServices;
using Gridify;
using Remora.Results;

namespace DataExplorer.EfCore.DataServices;

/// <summary>
/// Read-only data service.
/// </summary>
/// <inheritdoc cref="IReadOnlyDataService{TEntity,TContext}"/>
[PublicAPI]
public class ReadOnlyDataService<TEntity, TId, TContext> : EfCoreDataServiceBase<TContext>,
    IReadOnlyDataService<TEntity, TId, TContext>
    where TEntity : EfEntity<TId>
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
    internal virtual IRepositoryBase BaseRepositoryInternal => UnitOfWork.GetRepository<IReadOnlyRepository<TEntity,TId>>();
    
    /// <summary>
    /// Gets the base repository for this data service.
    /// </summary>
    protected IRepositoryBase BaseRepository => BaseRepositoryInternal;
    
    /// <summary>
    /// Gets the read-only version of the <see cref="BaseRepository"/> (essentially casts it for you).
    /// </summary>
    protected IReadOnlyRepository<TEntity,TId> ReadOnlyRepository =>
        (IReadOnlyRepository<TEntity,TId>)BaseRepository;

    /// <inheritdoc />
    public virtual async Task<Result<TEntity>> GetAsync(params object[] keyValues)
    {
        try
        {
            var entity = await ReadOnlyRepository.GetAsync(keyValues).ConfigureAwait(false);
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
            var entity = await ReadOnlyRepository.GetAsync(keyValues, cancellationToken).ConfigureAwait(false);
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
            var entity = await ReadOnlyRepository.GetAsync(keyValues, cancellationToken).ConfigureAwait(false);
            return entity is null ? new NotFoundError() : entity;
        }
        catch (Exception ex)
        {
            return new ExceptionError(ex);
        }
    }

    /// <inheritdoc />
    public virtual async Task<Result<Paging<TEntity>>> GetByGridifyQueryAsync(IGridifyQuery gridifyQuery,
        CancellationToken cancellationToken = default)
    {
        try
        {
           return await ReadOnlyRepository.GetByGridifyQueryAsync(gridifyQuery, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            return new ExceptionError(ex);
        }
    }

    /// <inheritdoc />
    public virtual async Task<Result<Paging<TEntity>>> GetByGridifyQueryAsync(IGridifyQuery gridifyQuery,
        IGridifyMapper<TEntity> gridifyMapper, CancellationToken cancellationToken = default)
    {
        try
        {
            return await ReadOnlyRepository.GetByGridifyQueryAsync(gridifyQuery, gridifyMapper, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            return new ExceptionError(ex);
        }
    }
    
    /// <inheritdoc />
    public virtual async Task<Result<Paging<TResult>>> GetByGridifyQueryAsync<TResult>(IGridifyQuery gridifyQuery, ResultTransformation resultTransformation,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (resultTransformation is ResultTransformation.ProjectTo)
                return await ReadOnlyRepository.GetByGridifyQueryAsync<TResult>(gridifyQuery, cancellationToken).ConfigureAwait(false);
            
            var sub = await ReadOnlyRepository.GetByGridifyQueryAsync(gridifyQuery, cancellationToken).ConfigureAwait(false);
            return new Paging<TResult>(sub.Count, Mapper.Map<IEnumerable<TResult>>(sub.Data));
        }
        catch (Exception ex)
        {
            return new ExceptionError(ex);
        }
    }

    /// <inheritdoc />
    public virtual async Task<Result<Paging<TResult>>> GetByGridifyQueryAsync<TResult>(IGridifyQuery gridifyQuery, ResultTransformation resultTransformation,
        IGridifyMapper<TEntity> gridifyMapper, CancellationToken cancellationToken = default)
    {
        try
        {
            if (resultTransformation is ResultTransformation.ProjectTo)
                return await ReadOnlyRepository.GetByGridifyQueryAsync<TResult>(gridifyQuery, gridifyMapper, cancellationToken).ConfigureAwait(false);
            
            var sub = await ReadOnlyRepository.GetByGridifyQueryAsync(gridifyQuery, gridifyMapper, cancellationToken).ConfigureAwait(false);
            return new Paging<TResult>(sub.Count, Mapper.Map<IEnumerable<TResult>>(sub.Data));
        }
        catch (Exception ex)
        {
            return new ExceptionError(ex);
        }
    }

    /// <inheritdoc />
    public virtual async Task<Result<TEntity>> GetSingleBySpecAsync(Specifications.ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
    {
        try
        {
            var entity = await ReadOnlyRepository.GetSingleBySpecAsync(specification, cancellationToken).ConfigureAwait(false);
            return entity is null ? new NotFoundError() : entity;
        }
        catch (Exception ex)
        {
            return new ExceptionError(ex);
        }
    }

    /// <inheritdoc />
    public virtual async Task<Result<TGetResult>> GetSingleBySpecAsync<TGetResult>(Specifications.ISpecification<TEntity> specification, CancellationToken cancellationToken = default) where TGetResult : class
    {
        try
        {
            var res = await GetSingleBySpecAsync(specification, cancellationToken).ConfigureAwait(false);
            return !res.IsDefined(out var entity) ? Result<TGetResult>.FromError(res) : Mapper.Map<TGetResult>(entity);
        }
        catch (Exception ex)
        {
            return new ExceptionError(ex);
        }
    }

    /// <inheritdoc />
    public virtual async Task<Result<TGetProjectedResult>> GetSingleBySpecAsync<TGetProjectedResult>(Specifications.ISpecification<TEntity, TGetProjectedResult> specification, CancellationToken cancellationToken = default)
    {
        try
        {
            var entity = await ReadOnlyRepository.GetSingleBySpecAsync(specification, cancellationToken).ConfigureAwait(false);
            return entity is null ? new NotFoundError() : entity;
        }
        catch (Exception ex)
        {
            return new ExceptionError(ex);
        }
    }

    /// <inheritdoc />
    public virtual async Task<Result<IReadOnlyList<TEntity>>> GetBySpecAsync(Specifications.ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
    {
        try
        {
            return Result<IReadOnlyList<TEntity>>.FromSuccess(await ReadOnlyRepository.GetBySpecAsync(specification, cancellationToken).ConfigureAwait(false));
        }
        catch (Exception ex)
        {
            return new ExceptionError(ex);
        }
    }

    /// <inheritdoc />
    public virtual async Task<Result<IReadOnlyList<TGetResult>>> GetBySpecAsync<TGetResult>(Specifications.ISpecification<TEntity> specification, CancellationToken cancellationToken = default) where TGetResult : class
    {
        try
        {
            var res = await GetBySpecAsync(specification, cancellationToken).ConfigureAwait(false);
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
    public virtual async Task<Result<IReadOnlyList<TGetProjectedResult>>> GetBySpecAsync<TGetProjectedResult>(Specifications.ISpecification<TEntity, TGetProjectedResult> specification, CancellationToken cancellationToken = default)
        => await WrapAsync(async () => await ReadOnlyRepository.GetBySpecAsync(specification, cancellationToken).ConfigureAwait(false));

    /// <inheritdoc />
    public virtual async Task<Result<IReadOnlyList<TGetResult>>> GetAllAsync<TGetResult>(bool shouldProject = false,
        CancellationToken cancellationToken = default)
        where TGetResult : class
    {
        try
        {
            if (shouldProject)
                return Result<IReadOnlyList<TGetResult>>.FromSuccess(await ReadOnlyRepository.GetAllAsync<TGetResult>(cancellationToken).ConfigureAwait(false));

            return Result<IReadOnlyList<TGetResult>>.FromSuccess(Mapper.Map<IReadOnlyList<TGetResult>>(
                await ReadOnlyRepository.GetAllAsync(cancellationToken).ConfigureAwait(false)));
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
            return Result<IReadOnlyList<TEntity>>.FromSuccess(await ReadOnlyRepository.GetAllAsync(cancellationToken).ConfigureAwait(false));
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    /// <inheritdoc />
    public virtual async Task<Result<long>> LongCountAsync(Specifications.ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
    {
        try
        {
            return Result<long>.FromSuccess(await ReadOnlyRepository.LongCountAsync(specification, cancellationToken).ConfigureAwait(false));
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
            return Result<long>.FromSuccess(await ReadOnlyRepository.LongCountAsync(cancellationToken).ConfigureAwait(false));
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
            return Result<bool>.FromSuccess(await ReadOnlyRepository.AnyAsync(predicate, cancellationToken).ConfigureAwait(false));
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    /// <inheritdoc />
    public async Task<Result<bool>> AnyAsync(Specifications.ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
    {
        try
        {
            return Result<bool>.FromSuccess(await ReadOnlyRepository.AnyAsync(specification, cancellationToken).ConfigureAwait(false));
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
    where TEntity : EfEntity<long>
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
    internal override IRepositoryBase BaseRepositoryInternal => UnitOfWork.GetRepository<IReadOnlyRepository<TEntity>>();
    
    /// <summary>
    /// Gets the read-only version of the <see cref="ReadOnlyDataService{TEntity,TId,TContext}.BaseRepository"/> (essentially casts it for you).
    /// </summary>
    protected new IReadOnlyRepository<TEntity> ReadOnlyRepository =>
        (IReadOnlyRepository<TEntity>)BaseRepository;
}
