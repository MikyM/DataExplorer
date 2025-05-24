using Autofac;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DataExplorer.Extensions.AutoMapper;

/// <summary>
/// Represents the extension's configuration.
/// </summary>
[PublicAPI]
public class DataExplorerAutoMapperConfiguration : DataExplorerConfigurationBase, IOptions<DataExplorerAutoMapperConfiguration>
{
    /// <summary>
    /// Creates an instance of the configuration class.
    /// </summary>
    public DataExplorerAutoMapperConfiguration(DataExplorerConfigurationBase configurationBase) : base(configurationBase)
    {
    }
    
    /// <summary>
    /// Creates an instance of the configuration class.
    /// </summary>
    public DataExplorerAutoMapperConfiguration(IServiceCollection serviceCollection) : base(serviceCollection)
    {
    }
    
    /// <summary>
    /// Creates an instance of the configuration class.
    /// </summary>
    public DataExplorerAutoMapperConfiguration(ContainerBuilder builder) : base(builder)
    {
    }
    
    /// <summary>
    /// Gets the container builder.
    /// </summary>
    internal ContainerBuilder? GetContainerBuilder()
        => Builder;
    
    /// <summary>
    /// Gets the service collection.
    /// </summary>
    internal IServiceCollection? GetServiceCollection()
        => ServiceCollection;

    /// <inheritdoc/>
    public DataExplorerAutoMapperConfiguration Value => this;
}