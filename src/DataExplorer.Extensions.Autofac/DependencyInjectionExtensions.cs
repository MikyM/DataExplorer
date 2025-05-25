using AttributeBasedRegistration;
using AttributeBasedRegistration.Autofac;
using Autofac;
using DataExplorer.Abstractions.Mapper;
using DataExplorer.Utilities;
using JetBrains.Annotations;
using Microsoft.Extensions.Options;

namespace DataExplorer.Extensions.Autofac;

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
        var config = new DataExplorerConfiguration(new AutofacRegistrator(builder));
        
        options.Invoke(config);

        builder.RegisterType<DefaultMapper>().As<IMapper>().SingleInstance().IfNotRegistered(typeof(IMapper));
        
        //register async interceptor adapter
        builder.RegisterGeneric(typeof(AsyncInterceptorAdapter<>));
        // register instance factory
        builder.RegisterType<CachedInstanceFactory>().As<ICachedInstanceFactory>().SingleInstance();

        builder.RegisterInstance(config).AsSelf().As<IOptions<DataExplorerConfiguration>>().SingleInstance();

#if NET8_0_OR_GREATER
        // register the time provider conditionally
        builder.RegisterInstance(TimeProvider.System).As<TimeProvider>().SingleInstance().IfNotRegistered(typeof(TimeProvider)); 
        builder.RegisterType<DataExplorerTimeProvider.DependencyDataExplorerTimeProvider>().As<DataExplorerTimeProvider>().SingleInstance();
#else
        builder.RegisterInstance(DataExplorerTimeProvider.Instance).As<DataExplorerTimeProvider>().SingleInstance();
#endif

        return builder;
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
