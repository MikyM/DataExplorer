using Autofac;
using DataExplorer.Abstractions.Mapper;
using DataExplorer.Abstractions.Specifications.Evaluators;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using IAutoMapper = AutoMapper.IMapper;

// ReSharper disable PossibleMultipleEnumeration

namespace DataExplorer.Extensions.AutoMapper;

/// <summary>
///  Extensions do the <see cref="DataExplorerConfiguration"/>.
/// </summary>
[PublicAPI]
public static class DataExplorerConfigurationExtensions
{
    /// <summary>
    /// Adds AutoMapper as the mapper used for DataExplorer operations.
    /// </summary>
    /// <param name="configuration">The basic configuration.</param>
    /// <param name="options">Optional configuration of the extension.</param>
    /// <returns>Current instance of the basic configuration,</returns>
    public static DataExplorerConfiguration UseAutoMapper(this DataExplorerConfiguration configuration, Action<DataExplorerAutoMapperConfiguration>? options = null)
    {
        var config = new DataExplorerAutoMapperConfiguration(configuration);
        
        options ??= _ => { };
        
        options.Invoke(config);
        
        var builder = config.GetContainerBuilder();
        var serviceCollection = config.GetServiceCollection();

        builder?.Register(x => new AutoMapperBridge(x.Resolve<IAutoMapper>())).As<IMapper>().SingleInstance();
        builder?.Register(x => new AutoMapperProjectionEvaluator(x.Resolve<IAutoMapper>())).As<IProjectionEvaluator>().SingleInstance();

        serviceCollection?.AddSingleton<IMapper>(x => new AutoMapperBridge(x.GetRequiredService<IAutoMapper>()));
        serviceCollection?.AddSingleton<IProjectionEvaluator>(x => new AutoMapperProjectionEvaluator(x.GetRequiredService<IAutoMapper>()));

        return configuration;
    }
}