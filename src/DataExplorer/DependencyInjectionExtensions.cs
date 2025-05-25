using DataExplorer.Abstractions.Mapper;
using DataExplorer.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DataExplorer;

/// <summary>
/// DI extensions.
/// </summary>
[PublicAPI]
public static class DependencyInjectionExtensions
{
    /// <summary>
    /// Adds Data Explorer to the application.
    /// </summary>
    /// <param name="serviceCollection">Current instance of <see cref="IServiceCollection"/>.</param>
    /// <param name="options"><see cref="Action"/> that configures DAL.</param>
    public static IServiceCollection AddDataExplorer(this IServiceCollection serviceCollection,
        Action<DataExplorerConfiguration> options)
    {
        var registrator = new MicrosoftRegistrator(serviceCollection);
        
        var config = new DataExplorerConfiguration(registrator);
        
        options.Invoke(config);
        
        serviceCollection.TryAddSingleton<IMapper, DefaultMapper>();
        
        // register instance factory
        serviceCollection.AddSingleton<ICachedInstanceFactory,CachedInstanceFactory>();

        serviceCollection.AddOptions<DataExplorerConfiguration>().Configure(options);
        serviceCollection.AddSingleton(config);
        
#if NET8_0_OR_GREATER
        // register the time provider conditionally
        serviceCollection.TryAddSingleton(TimeProvider.System);
        serviceCollection.AddSingleton<DataExplorerTimeProvider,DataExplorerTimeProvider.DependencyDataExplorerTimeProvider>();
#else
        serviceCollection.AddSingleton(DataExplorerTimeProvider.Instance);
#endif
        return serviceCollection;
    }
}
