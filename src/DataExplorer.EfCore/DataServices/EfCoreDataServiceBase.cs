using AutoMapper;
using DataExplorer.Abstractions.DataServices;
using DataExplorer.Abstractions.UnitOfWork;
using DataExplorer.EfCore.Abstractions;
using DataExplorer.EfCore.Abstractions.DataServices;
using DataExplorer.EfCore.Gridify;
using Microsoft.EntityFrameworkCore.Storage;
using Remora.Results;

namespace DataExplorer.EfCore.DataServices;

/// <inheritdoc cref="IEfCoreDataServiceBase{TContext}"/>
public abstract class EfCoreDataServiceBase<TContext> : IEfCoreDataServiceBase<TContext> where TContext : class, IEfDbContext
{
    /// <inheritdoc/>
    public IMapper Mapper => UnitOfWork.Mapper;

    /// <inheritdoc/>
    public IGridifyMapperProvider GridifyMapperProvider => UnitOfWork.GridifyMapperProvider;

    /// <inheritdoc/>
    public IUnitOfWork<TContext> UnitOfWork { get; }
    
    private bool _disposed;

    /// <summary>
    /// Creates a new instance of <see cref="EfCoreDataServiceBase{TContext}"/>.
    /// </summary>
    /// <param name="uof">Instance of <see cref="IUnitOfWork{TContext}"/>.</param>
    protected EfCoreDataServiceBase(IUnitOfWork<TContext> uof)
    {
        UnitOfWork = uof;
    }

    /// <inheritdoc />
    public virtual async Task<Result> CommitAsync(string auditUserId, CancellationToken cancellationToken = default)
    {
        try
        {
            await UnitOfWork.CommitAsync(auditUserId, cancellationToken).ConfigureAwait(false);
            return Result.FromSuccess();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    /// <inheritdoc />
    public virtual async Task<Result> CommitAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await UnitOfWork.CommitAsync(cancellationToken).ConfigureAwait(false);
            return Result.FromSuccess();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    /// <inheritdoc />
    public virtual async Task<Result<int>> CommitWithCountAsync(string auditUserId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await UnitOfWork.CommitWithCountAsync(auditUserId, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    /// <inheritdoc />
    public virtual async Task<Result<int>> CommitWithCountAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await UnitOfWork.CommitWithCountAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    /// <inheritdoc />
    public virtual async Task<Result> RollbackAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await UnitOfWork.RollbackAsync(cancellationToken).ConfigureAwait(false);
            return Result.FromSuccess();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    /// <inheritdoc />
    public virtual async Task<Result<IDbContextTransaction>> UseExplicitTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return Result<IDbContextTransaction>.FromSuccess(await UnitOfWork.UseExplicitTransactionAsync(cancellationToken).ConfigureAwait(false));
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    /// <inheritdoc />
    public TContext Context => UnitOfWork.Context;

    IUnitOfWorkBase IDataServiceBase<TContext>.UnitOfWork => UnitOfWork;


    // Public implementation of Dispose pattern callable by consumers.
    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    // Protected implementation of Dispose pattern.
    /// <summary>
    /// Dispose action
    /// </summary>
    /// <param name="disposing">Whether disposing</param>
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) 
            return;

        if (disposing) 
            UnitOfWork.Dispose();

        _disposed = true;
    }

    /// <summary>
    /// Wraps a call in a try catch block.
    /// </summary>
    /// <param name="func">Func to wrap.</param>
    /// <param name="shouldConfigureAwaitFalse">Whether to use ConfigureAwait(false).</param>
    /// <typeparam name="TResult">Result.</typeparam>
    /// <returns>Result of the call.</returns>
    public async Task<Result<TResult>> WrapAsync<TResult>(Func<Task<TResult>> func, bool shouldConfigureAwaitFalse = true)
    {
        try
        {
            return await func.Invoke().ConfigureAwait(!shouldConfigureAwaitFalse);
        }
        catch (Exception ex)
        {
            return new ExceptionError(ex);
        }
    }
    
    /// <summary>
    /// Wraps a call in a try catch block.
    /// </summary>
    /// <param name="func">Func to wrap.</param>
    /// <param name="shouldConfigureAwaitFalse">Whether to use ConfigureAwait(false).</param>
    /// <returns>Result of the call.</returns>
    public async Task<Result> WrapAsync(Func<Task> func, bool shouldConfigureAwaitFalse = true)
    {
        try
        {
            await func.Invoke().ConfigureAwait(!shouldConfigureAwaitFalse);
            return Result.FromSuccess();
        }
        catch (Exception ex)
        {
            return new ExceptionError(ex);
        }
    }
}
