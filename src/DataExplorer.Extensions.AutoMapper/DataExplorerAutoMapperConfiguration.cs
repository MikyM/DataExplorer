using DataExplorer.Abstractions;
using JetBrains.Annotations;
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
    public DataExplorerAutoMapperConfiguration(IRegistrator registrator) : base(registrator)
    {
    }

    /// <inheritdoc/>
    public DataExplorerAutoMapperConfiguration Value => this;

    internal IRegistrator GetRegistrator() => Registrator;
}