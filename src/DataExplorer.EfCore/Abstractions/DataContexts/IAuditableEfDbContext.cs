namespace DataExplorer.EfCore.Abstractions.DataContexts;

/// <summary>
/// Defines an auditable <see cref="IEfDbContext"/>.
/// </summary>
[PublicAPI]
public interface IAuditableEfDbContext : IEfDbContext
{
}
