using DataExplorer.EfCore.Extensions;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;

// ReSharper disable SuspiciousTypeConversion.Global

namespace DataExplorer.EfCore.DataContexts;

/// <summary>
/// Base definition of a EF database context.
/// </summary>
/// <inheritdoc cref="DbContext"/>
[PublicAPI]
public abstract class EfDbContext : DbContext, IEfDbContext
{
    /// <summary>
    /// Configuration.
    /// </summary>
    protected readonly IOptions<DataExplorerEfCoreConfiguration> Config;
    
    /// <summary>
    /// The time provider.
    /// </summary>
    protected readonly DataExplorerTimeProvider TimeProvider; 

    
    /// <inheritdoc />
    // This ctor is required to be able to use the context with context pooling.
    protected EfDbContext(DbContextOptions options) : base(options)
    {
        Config = this.GetService<IOptions<DataExplorerEfCoreConfiguration>>();
        TimeProvider = this.GetService<DataExplorerTimeProvider>();
    }


    /// <inheritdoc />
    protected EfDbContext(DbContextOptions options, IOptions<DataExplorerEfCoreConfiguration> config, DataExplorerTimeProvider timeProvider) : base(options)
    {
        Config = config;
        TimeProvider = timeProvider;
    }

    /// <inheritdoc/>
    public IQueryable<TEntity> ExecuteRawSql<TEntity>(string sql, params object[] parameters) where TEntity : class
        => Set<TEntity>().FromSqlRaw(sql, parameters);
    
    /// <inheritdoc/>
    public IQueryable<TEntity> ExecuteInterpolatedSql<TEntity>(FormattableString sql) where TEntity : class
        => Set<TEntity>().FromSqlInterpolated(sql);
    
    /// <inheritdoc/>
    public int ExecuteRawSql(string sql)
        => Database.ExecuteSqlRaw(sql);
    
    /// <inheritdoc/>
    public int ExecuteRawSql(string sql, params object[] parameters)
        => Database.ExecuteSqlRaw(sql, parameters);
    
    /// <inheritdoc/>
    public Task<int> ExecuteRawSqlAsync(string sql, CancellationToken cancellationToken = default)
        => Database.ExecuteSqlRawAsync(sql, cancellationToken);
    
    /// <inheritdoc/>
    public Task<int> ExecuteRawSqlAsync(string sql, CancellationToken cancellationToken = default, params object[] parameters)
        => Database.ExecuteSqlRawAsync(sql, cancellationToken, parameters);
    
    /// <inheritdoc/>
    public Task<int> ExecuteRawSqlAsync(string sql, params object[] parameters)
        => Database.ExecuteSqlRawAsync(sql, parameters);
    
    /// <inheritdoc/>
    public TEntity? FindTracked<TEntity>(params object[] keyValues) where TEntity : class
        => DbContextExtensions.FindTracked<TEntity>(this, keyValues);

    /// <summary>
    /// Gets the table's name that is mapped to given entity type.
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException">Throw when couldn't find the entity type or the table name.</exception>
    public string? GetTableName<TEntity>() where TEntity : class
        => Model.FindEntityType(typeof(TEntity))?.GetTableName() ??
           throw new InvalidOperationException($"Couldn't find table name or entity type {typeof(TEntity).Name}");
    
    /// <inheritdoc cref="DbContext.SaveChangesAsync(bool,System.Threading.CancellationToken)" />
    /// <remarks>
    /// Executes <see cref="OnBeforeSaveChangesAsync"/> if not disabled.
    /// </remarks>
    public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = default)
    {
        if (!Config.Value.DisableOnBeforeSaveChanges) 
            await OnBeforeSaveChangesAsync();
        
        return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    /// <inheritdoc cref="DbContext.SaveChangesAsync(System.Threading.CancellationToken)" />
    /// <remarks>
    /// Executes <see cref="OnBeforeSaveChangesAsync"/> if not disabled.
    /// </remarks>
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        if (!Config.Value.DisableOnBeforeSaveChanges) 
            await OnBeforeSaveChangesAsync();
        
        return await base.SaveChangesAsync(true, cancellationToken);
    }
    
    /// <summary>
    /// Gets the entries that are currently tracked by the context.
    /// </summary>
    /// <returns>Detected entries.</returns>
    /// <remarks>This calls DetectChanges and makes a collection copy.</remarks>
    protected virtual IReadOnlyList<EntityEntry> GetTrackedEntries()
    {
        ChangeTracker.DetectChanges();
        return ChangeTracker.Entries().ToList().AsReadOnly();
    }

    /// <summary>
    /// Executes an action before executing SaveChanges.
    /// </summary>
    protected virtual ValueTask OnBeforeSaveChangesAsync(IReadOnlyList<EntityEntry>? entries = null)
    {
        entries ??= GetTrackedEntries();
        
        var nowOffset = Config.Value.DateTimeStrategy switch
        {
            DateTimeStrategy.UtcNow => TimeProvider.GetUtcNow(),
            DateTimeStrategy.Now => TimeProvider.GetLocalNow(),
            _ => throw new ArgumentOutOfRangeException()
        };
        
        var now = Config.Value.DateTimeStrategy switch
        {
            DateTimeStrategy.UtcNow => nowOffset.DateTime.ToUniversalTime(),
            DateTimeStrategy.Now => nowOffset.DateTime.ToLocalTime(),
            _ => throw new ArgumentOutOfRangeException()
        };
        
        foreach (var entry in entries)
        {
            if (entry.Entity is IEntity entity)
                switch (entry.State)
                {
                    case EntityState.Added:
                        if (entity is ICreatedAt { CreatedAt: null } createdAt)
                        {
                            createdAt.CreatedAt = now;
                            entry.Property(nameof(ICreatedAt.CreatedAt)).IsModified = true;
                        }
                        if (entity is ICreatedAtOffset { CreatedAt: null } createdAtOffset)
                        {
                            createdAtOffset.CreatedAt = nowOffset;
                            entry.Property(nameof(ICreatedAtOffset.CreatedAt)).IsModified = true;
                        }
                        if (entity is IUpdatedAt { UpdatedAt: null } addedUpdatedAt)
                        {
                            addedUpdatedAt.UpdatedAt = now;
                            entry.Property(nameof(IUpdatedAt.UpdatedAt)).IsModified = true;
                        }
                        if (entity is IUpdatedAtOffset { UpdatedAt: null } addedUpdatedAtOffset)
                        {
                            addedUpdatedAtOffset.UpdatedAt = nowOffset;
                            entry.Property(nameof(IUpdatedAtOffset.UpdatedAt)).IsModified = true;
                        }
                        break;
                    case EntityState.Modified:
                        if (entity is IUpdatedAt updatedAt && !entry.Property(nameof(IUpdatedAt.UpdatedAt)).IsModified)
                        {
                            updatedAt.UpdatedAt = now;
                            entry.Property(nameof(IUpdatedAt.UpdatedAt)).IsModified = true;
                        }
                        if (entity is IUpdatedAtOffset updatedAtOffset && !entry.Property(nameof(IUpdatedAt.UpdatedAt)).IsModified)
                        {
                            updatedAtOffset.UpdatedAt = nowOffset;
                            entry.Property(nameof(IUpdatedAtOffset.UpdatedAt)).IsModified = true;
                        }
                        break;
                }
        }
        
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc/>
    Task IDataContextBase.SaveChangesAsync(CancellationToken cancellationToken)
        => SaveChangesAsync(cancellationToken);
}
