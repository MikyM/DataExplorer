namespace DataExplorer.EfCore.Abstractions.DataContexts;

/// <summary>
/// Represents an auditable <see cref="IEfDbContext"/>.
/// </summary>
[PublicAPI]
public interface IAuditableEfDbContext : IEfDbContext
{
}
