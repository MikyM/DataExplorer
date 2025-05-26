using Autofac;
using Microsoft.Extensions.DependencyInjection;

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
        Builder = configurationBase.Builder;
        ServiceCollection = configurationBase.ServiceCollection;
    }

    /// <summary>
    /// Creates an instance of the configuration class.
    /// </summary>
    protected DataExplorerConfigurationBase(ContainerBuilder builder)
    {
        Builder = builder;
    }

    /// <summary>
    /// Creates an instance of the configuration class.
    /// </summary>
    protected DataExplorerConfigurationBase(IServiceCollection serviceCollection)
    {
        ServiceCollection = serviceCollection;
    }
    
    /// <summary>
    /// Autofac's container builder.
    /// </summary>
    protected ContainerBuilder? Builder { get; private set; }
    
    /// <summary>
    /// Microsoft's service collection.
    /// </summary>
    protected IServiceCollection? ServiceCollection { get; private set; }
    
    /// <summary>
    /// Releases references to the container builder and service collection.
    /// </summary>
    internal void ReleaseRefs()
    {
        Builder = null;
        ServiceCollection = null;
    }
}
