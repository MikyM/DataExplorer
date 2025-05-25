using System.Linq.Expressions;
using DataExplorer.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DataExplorer;

/// <summary>
/// Implementation of <see cref="IRegistrator"/> based on Microsoft's DI.
/// </summary>
[PublicAPI]
public sealed class MicrosoftRegistration : RegistrationBase
{
    private MicrosoftRegistrator _registrator;
    
    public MicrosoftRegistration(MicrosoftRegistrator registrator, Type implementationType) : base(registrator, implementationType)
    {
        _registrator = registrator;
    }

    public MicrosoftRegistration(MicrosoftRegistrator registrator, object implementationInstance) : base(registrator, implementationInstance)
    {
        _registrator = registrator;
    }

    public MicrosoftRegistration(MicrosoftRegistrator registrator, Expression<Func<IResolver, object>> factory, Type implementationType) : base(registrator, factory, implementationType)
    {
        _registrator = registrator;
    }

    private static Func<IServiceProvider, object> TranslateFactory(Expression<Func<IResolver, object>> factory)
    {
        var visitor = new FactoryExpressionVisitor();

        var modified = visitor.Visit(factory);

        var compiled = ((LambdaExpression)modified).Compile();
        
        return (Func<IServiceProvider, object>)compiled;
    }

    public override IRegistrator RegisterImplementation()
    {
        if (Lifetime is null)
        {
            throw new InvalidOperationException("Lifetime is required");
        }
        
        if (ShouldOnlyRegisterIfNotRegistered)
        {
            foreach (var serviceType in ServiceTypes)
            {
                if (ImplementationFactory is not null)
                {
                    _registrator.Services.TryAdd(ServiceDescriptor.Describe(serviceType, TranslateFactory(ImplementationFactory), ToMicrosoftLifetime(Lifetime.Value)));
                }
                else if (ImplementationInstance is not null)
                {
                    _registrator.Services.TryAdd(ServiceDescriptor.Describe(serviceType, _ => ImplementationInstance, ToMicrosoftLifetime(Lifetime.Value)));
                }
                else if (ImplementationType is not null)
                {
                    _registrator.Services.TryAdd(ServiceDescriptor.Describe(serviceType, ImplementationType, ToMicrosoftLifetime(Lifetime.Value)));
                }
            }
        }
        else
        {
            foreach (var serviceType in ServiceTypes)
            {
                if (ImplementationFactory is not null)
                {
                    _registrator.Services.Add(ServiceDescriptor.Describe(serviceType, TranslateFactory(ImplementationFactory), ToMicrosoftLifetime(Lifetime.Value)));
                }
                else if (ImplementationInstance is not null)
                {
                    _registrator.Services.Add(ServiceDescriptor.Describe(serviceType, _ => ImplementationInstance, ToMicrosoftLifetime(Lifetime.Value)));
                }
                else if (ImplementationType is not null)
                {
                    _registrator.Services.Add(ServiceDescriptor.Describe(serviceType, ImplementationType, ToMicrosoftLifetime(Lifetime.Value)));
                }
            }
        }

        return _registrator;
    }
    
    public static ServiceLifetime ToMicrosoftLifetime(AttributeBasedRegistration.ServiceLifetime lifetime)
        => lifetime switch
        {
            AttributeBasedRegistration.ServiceLifetime.SingleInstance => ServiceLifetime.Singleton,
            AttributeBasedRegistration.ServiceLifetime.InstancePerRequest => ServiceLifetime.Scoped,
            AttributeBasedRegistration.ServiceLifetime.InstancePerLifetimeScope =>  ServiceLifetime.Scoped,
            AttributeBasedRegistration.ServiceLifetime.InstancePerMatchingLifetimeScope => throw new NotSupportedException(),
            AttributeBasedRegistration.ServiceLifetime.InstancePerDependency => ServiceLifetime.Transient,
            AttributeBasedRegistration.ServiceLifetime.InstancePerOwned => throw new NotSupportedException(),
            _ => throw new ArgumentOutOfRangeException(nameof(lifetime), lifetime, null)
        };


    public override IRegistration EnableClassInterceptors()
    {
        throw new InvalidOperationException("Class interceptors are not supported");
    }

    public override IRegistration EnableInterfaceInterceptors()
    {
        throw new InvalidOperationException("Interface interceptors are not supported");
    }

    public override IRegistration InterceptedBy(Type interceptorType)
    {
        throw new InvalidOperationException("Interception is not supported");
    }

    public override IRegistration WithMatchingLifetime(object[] tags)
    {
        throw new InvalidOperationException("Matching lifetime is not supported");
    }
    
    public override IRegistration WithOwnedLifetime(Type owned)
    {
        throw new InvalidOperationException("Owned lifetime is not supported");
    }
}