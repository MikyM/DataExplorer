using System.Linq.Expressions;
using Microsoft.Extensions.Hosting;

namespace DataExplorer.Abstractions;

/// <summary>
/// Represents a DI registration service.
/// </summary>
public interface IRegistrator
{
    /// <summary>
    /// Registers given instance as a service within the DI container.
    /// </summary>
    /// <param name="factory">The factory to register.</param>
    /// <param name="implementationType">Implementation type.</param>
    IRegistration DescribeFactory(Expression<Func<IResolver, object>> factory, Type implementationType);
    
    /// <summary>
    /// Registers given service and implementation within the DI container.
    /// </summary>
    /// <param name="implementationType">The implementation type.</param>
    IRegistration Describe(Type implementationType);
    
    /// <summary>
    /// Registers given service and implementation within the DI container.
    /// </summary>
    /// <param name="implementationType">The implementation type.</param>
    IRegistration DescribeOpenGeneric(Type implementationType);

    /// <summary>
    /// Registers given instance as a service within the DI container.
    /// </summary>
    /// <param name="instance">The instance to register.</param>
    IRegistration DescribeInstance(object instance);

    /// <summary>
    /// Registers given instance as a service within the DI container.
    /// </summary>
    /// <param name="instance">The instance to register.</param>
    IRegistrator DescribeOptions<TOptions>(TOptions instance) where TOptions : class;

    /// <summary>
    /// Registers given instance as a service within the DI container.
    /// </summary>
    /// <param name="action">The instance to register.</param>
    /// <param name="instance">Instance.</param>
    IRegistrator DescribeOptions<TOptions>(Action<TOptions> action, TOptions instance) where TOptions : class;
    
    /// <summary>
    /// Registers a hosted service.
    /// </summary>
    /// <typeparam name="THostedService">The service to register.</typeparam>
    IRegistrator DescribeHostedService<THostedService>() where THostedService : class, IHostedService;

    /// <summary>
    /// Gets whether the registrator supports decorators.
    /// </summary>
    bool SupportsDecoration { get; }
    
    /// <summary>
    /// Registers a decorator.
    /// </summary>
    /// <param name="decoratorType"></param>
    /// <param name="decoratedService"></param>
    IRegistration DescribeDecorator(Type decoratorType, Type decoratedService);
    
    /// <summary>
    /// Registers a decorator.
    /// </summary>
    /// <param name="decoratorType"></param>
    /// <param name="decoratedService"></param>
    IRegistration DescribeOpenGenericDecorator(Type decoratorType, Type decoratedService);
    
    /// <summary>
    /// Gets whether the registration supports interceptors.
    /// </summary>
    bool SupportsInterception { get; }
    
    /// <summary>
    /// Gets whether the registration supports non-public constructors.
    /// </summary>
    bool SupportsNonPublicConstructors { get; }
}