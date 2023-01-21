using AutoMapper;
using DataExplorer.Abstractions.DataServices;
using DataExplorer.Abstractions.UnitOfWork;
using DataExplorer.MongoDb.Abstractions;
using DataExplorer.MongoDb.Abstractions.DataContexts;
using DataExplorer.MongoDb.Abstractions.DataServices;

namespace DataExplorer.MongoDb.DataServices;

/// <inheritdoc cref="IMongoDataServiceBase{TContext}"/>
public abstract class MongoDataServiceBase<TContext> : IMongoDataServiceBase<TContext> where TContext : class, IMongoDbContext
{
    /// <inheritdoc/>
    public IMapper Mapper => UnitOfWork.Mapper;
    
    /// <inheritdoc/>
    public IMongoUnitOfWork<TContext> UnitOfWork { get; }
    
    private bool _disposed;

    /// <summary>
    /// Creates a new instance of <see cref="MongoDataServiceBase{TContext}"/>.
    /// </summary>
    /// <param name="uof">Instance of <see cref="IMongoUnitOfWork{TContext}"/>.</param>
    protected MongoDataServiceBase(IMongoUnitOfWork<TContext> uof)
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
    public virtual async Task<Result> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await UnitOfWork.UseTransactionAsync(cancellationToken).ConfigureAwait(false);
            return Result.FromSuccess();
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
