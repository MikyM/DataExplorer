using Autofac;
using AutoMapper.Contrib.Autofac.DependencyInjection;
using AutoMapper.Extensions.ExpressionMapping;
using MikyM.Autofac.Extensions;

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
        builder.RegisterAutoMapper(opt => opt.AddExpressionMapping(), false, AppDomain.CurrentDomain.GetAssemblies());
        //register async interceptor adapter
        builder.RegisterGeneric(typeof(AsyncInterceptorAdapter<>));

        return builder;
    }

    /// <summary>
    /// Registers an interceptor with <see cref="ContainerBuilder"/>.
    /// </summary>
    /// <param name="builder">Current builder instance.</param>
    /// <param name="factoryMethod">Factory method for the registration.</param>
    /// <param name="interceptorLifetime">Lifetime of the registered interceptor.</param>
    /// <returns>Current instance of the <see cref="DataExplorerConfiguration"/></returns>
    public static ContainerBuilder AddInterceptor<T>(this ContainerBuilder builder, Func<IComponentContext, T> factoryMethod, Lifetime interceptorLifetime) where T : notnull
    {
        _ = interceptorLifetime switch
        {
            Lifetime.SingleInstance => builder.Register(factoryMethod).AsSelf().SingleInstance(),
            Lifetime.InstancePerRequest => builder.Register(factoryMethod).AsSelf().InstancePerRequest(),  
            Lifetime.InstancePerLifetimeScope => builder.Register(factoryMethod).AsSelf().InstancePerLifetimeScope(),  
            Lifetime.InstancePerMatchingLifetimeScope => throw new NotSupportedException(),
            Lifetime.InstancePerDependency => builder.Register(factoryMethod).AsSelf().InstancePerDependency(), 
            Lifetime.InstancePerOwned => throw new NotSupportedException(),
            _ => throw new ArgumentOutOfRangeException(nameof(interceptorLifetime), interceptorLifetime, null)
        }; 
        
        return builder;
    }
}
