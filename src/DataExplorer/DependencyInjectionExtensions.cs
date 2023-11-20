using AttributeBasedRegistration.Autofac;
using Autofac;
using AutoMapper.Contrib.Autofac.DependencyInjection;
using DataExplorer.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ServiceLifetime = AttributeBasedRegistration.ServiceLifetime;

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
    /// <param name="builder">Current instance of <see cref="ContainerBuilder"/>.</param>
    /// <param name="options"><see cref="Action"/> that configures DAL.</param>
    public static ContainerBuilder AddDataExplorer(this ContainerBuilder builder, Action<DataExplorerConfiguration> options)
    {
        var config = new DataExplorerConfiguration(builder);
        options.Invoke(config);

        // register automapper
        builder.RegisterAutoMapper(config.AutoMapperConfiguration, false, config.AutoMapperProfileAssembliesAccessor().ToArray());
        //register async interceptor adapter
        builder.RegisterGeneric(typeof(AsyncInterceptorAdapter<>));
        // register instance factory
        builder.RegisterType<CachedInstanceFactory>().As<ICachedInstanceFactory>().SingleInstance();

#if NET8_0
        // register the time provider conditionally
        builder.RegisterInstance(TimeProvider.System).As<TimeProvider>().SingleInstance().IfNotRegistered(typeof(TimeProvider)); 
#endif


        return builder;
    }
    
    /// <summary>
    /// Adds Data Explorer to the application.
    /// </summary>
    /// <param name="serviceCollection">Current instance of <see cref="IServiceCollection"/>.</param>
    /// <param name="options"><see cref="Action"/> that configures DAL.</param>
    public static IServiceCollection AddDataExplorer(this IServiceCollection serviceCollection,
        Action<DataExplorerConfiguration> options)
    {
        var config = new DataExplorerConfiguration(serviceCollection);
        options.Invoke(config);
        
        // register automapper
        serviceCollection.AddAutoMapper(config.AutoMapperConfiguration, config.AutoMapperProfileAssembliesAccessor(), Microsoft.Extensions.DependencyInjection.ServiceLifetime.Scoped);
        // register async interceptor adapter
        serviceCollection.AddSingleton(typeof(AsyncInterceptorAdapter<>));
        // register instance factory
        serviceCollection.AddSingleton<ICachedInstanceFactory,CachedInstanceFactory>();
        
#if NET8_0
        // register the time provider conditionally
        serviceCollection.TryAddSingleton(TimeProvider.System);
#endif

        config.ReleaseRefs();

        return serviceCollection;
    }

    /// <summary>
    /// Registers an interceptor with <see cref="ContainerBuilder"/>.
    /// </summary>
    /// <param name="builder">Current builder instance.</param>
    /// <param name="factoryMethod">Factory method for the registration.</param>
    /// <param name="interceptorLifetime">Lifetime of the registered interceptor.</param>
    /// <returns>Current instance of the <see cref="DataExplorerConfiguration"/></returns>
    public static ContainerBuilder AddInterceptor<T>(this ContainerBuilder builder, Func<IComponentContext, T> factoryMethod, ServiceLifetime interceptorLifetime) where T : notnull
    {
        _ = interceptorLifetime switch
        {
            ServiceLifetime.SingleInstance => builder.Register(factoryMethod).AsSelf().SingleInstance(),
            ServiceLifetime.InstancePerRequest => builder.Register(factoryMethod).AsSelf().InstancePerRequest(),  
            ServiceLifetime.InstancePerLifetimeScope => builder.Register(factoryMethod).AsSelf().InstancePerLifetimeScope(),  
            ServiceLifetime.InstancePerMatchingLifetimeScope => throw new NotSupportedException(),
            ServiceLifetime.InstancePerDependency => builder.Register(factoryMethod).AsSelf().InstancePerDependency(), 
            ServiceLifetime.InstancePerOwned => throw new NotSupportedException(),
            _ => throw new ArgumentOutOfRangeException(nameof(interceptorLifetime), interceptorLifetime, null)
        }; 
        
        return builder;
    }
}
