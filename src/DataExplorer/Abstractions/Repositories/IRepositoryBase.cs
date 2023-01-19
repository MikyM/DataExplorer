using DataExplorer.Abstractions.DataContexts;

namespace DataExplorer.Abstractions.Repositories;

/// <summary>
/// Defines a base repository.
/// </summary>
[PublicAPI]
public interface IRepositoryBase
{
    /// <summary>
    /// The type of the entity held in this repository.
    /// </summary>
    Type EntityType { get; }
    
    /// <summary>
    /// The current data context.
    /// </summary>
    IDataContextBase Context { get; }
}
