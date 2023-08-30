using DataExplorer.EfCore.Extensions;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Options;
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
    /// Detected changes. This will be null prior to calling SaveChangesAsync.
    /// </summary>
    protected List<EntityEntry>? DetectedChanges;

    /// <inheritdoc />
    protected EfDbContext(DbContextOptions options) : base(options)
    {
        Config = this.GetService<IOptions<DataExplorerEfCoreConfiguration>>();
    }

    /// <inheritdoc />
    protected EfDbContext(DbContextOptions options, IOptions<DataExplorerEfCoreConfiguration> config) : base(options)
    {
        Config = config;
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
    public async Task<int> ExecuteRawSqlAsync(string sql, CancellationToken cancellationToken = default)
        => await Database.ExecuteSqlRawAsync(sql, cancellationToken);
    /// <inheritdoc/>
    public async Task<int> ExecuteRawSqlAsync(string sql, CancellationToken cancellationToken = default, params object[] parameters)
        => await Database.ExecuteSqlRawAsync(sql, cancellationToken, parameters);
    /// <inheritdoc/>
    public async Task<int> ExecuteRawSqlAsync(string sql, params object[] parameters)
        => await Database.ExecuteSqlRawAsync(sql, parameters);
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
    /// Executes <see cref="OnBeforeSaveChanges"/> if not disabled.
    /// </remarks>
    public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = default)
    {
        if (!Config.Value.DisableOnBeforeSaveChanges) 
            OnBeforeSaveChanges();
        
        return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc cref="DbContext.SaveChangesAsync(System.Threading.CancellationToken)" />
    /// <remarks>
    /// Executes <see cref="OnBeforeSaveChanges"/> if not disabled.
    /// </remarks>
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        if (!Config.Value.DisableOnBeforeSaveChanges) 
            OnBeforeSaveChanges();
        
        return await base.SaveChangesAsync(true, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Executes an action before executing SaveChanges.
    /// </summary>
    protected virtual void OnBeforeSaveChanges(List<EntityEntry>? entries = null)
    {
        if (entries is null)
        {
            ChangeTracker.DetectChanges();
            entries = ChangeTracker.Entries().ToList();
        }

        var now = Config.Value.DateTimeStrategy switch
        {
            DateTimeStrategy.UtcNow => DateTime.UtcNow,
            DateTimeStrategy.Now => DateTime.Now,
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
                        if (entity is IUpdatedAt { UpdatedAt: null } addedUpdatedAt)
                        {
                            addedUpdatedAt.UpdatedAt = now;
                            entry.Property(nameof(IUpdatedAt.UpdatedAt)).IsModified = true;
                        }
                        break;
                    case EntityState.Modified:
                        if (entity is IUpdatedAt updatedAt && !entry.Property(nameof(IUpdatedAt.UpdatedAt)).IsModified)
                        {
                            updatedAt.UpdatedAt = now;
                            entry.Property(nameof(IUpdatedAt.UpdatedAt)).IsModified = true;
                        }
                        break;
                }
        }
    }
}
