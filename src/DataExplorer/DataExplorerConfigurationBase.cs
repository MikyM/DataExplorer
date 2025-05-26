using DataExplorer.Abstractions;

namespace DataExplorer;

/// <summary>
/// Base configuration of the data explorer.
/// </summary>
[PublicAPI]
public abstract class DataExplorerConfigurationBase
{
    /// <summary>
    /// Creates an instance of the configuration class.
    /// </summary>
    protected DataExplorerConfigurationBase(DataExplorerConfigurationBase configurationBase)
    {
        Registrator = configurationBase.Registrator;
    }
    

    /// <summary>
    /// Creates an instance of the configuration class.
    /// </summary>
    protected DataExplorerConfigurationBase(IRegistrator registrator)
    {
        Registrator = registrator;
    }
    
    
    /// <summary>
    /// Gets the service used to register dependencies within DI container.
    /// </summary>
    protected IRegistrator Registrator { get; private set; }
}
