using System.Linq.Expressions;
using AttributeBasedRegistration.Autofac;
using Autofac;
using Autofac.Extras.DynamicProxy;
using Castle.DynamicProxy;
using DataExplorer.Abstractions;
using JetBrains.Annotations;

namespace DataExplorer.Extensions.Autofac;

/// <summary>
/// Implementation of <see cref="IRegistrator"/> based on Microsoft's DI.
/// </summary>
[PublicAPI]
public sealed class AutofacRegistration : RegistrationBase
{
    private AutofacRegistrator _registrator;
    
    public AutofacRegistration(AutofacRegistrator registrator, Type implementationType) : base(registrator, implementationType)
    {
        _registrator = registrator;
    }

    public AutofacRegistration(AutofacRegistrator registrator, object implementationInstance) : base(registrator, implementationInstance)
    {
        _registrator = registrator;
    }

    public AutofacRegistration(AutofacRegistrator registrator, Expression<Func<IResolver, object>> factory, Type implementationType) : base(registrator, factory, implementationType)
    {
        _registrator = registrator;
    }
    
    private static Func<IComponentContext, object> TranslateFactory(Expression<Func<IResolver, object>> factory)
    {
        var visitor = new FactoryExpressionVisitor();

        var modified = visitor.Visit(factory);

        var compiled = ((LambdaExpression)modified).Compile();
        
        return (Func<IComponentContext, object>)compiled;
    }

    public override IRegistrator RegisterImplementation()
    {
        if (Lifetime is null)
        {
            throw new InvalidOperationException("Lifetime is required");
        }

        var factoryBuilder = ImplementationFactory is not null 
            ? _registrator.Services.Register(TranslateFactory(ImplementationFactory))
            : null;
        
        var instanceBuilder = ImplementationInstance is not null 
            ? _registrator.Services.RegisterInstance(ImplementationInstance)
            : null;
        
        var typeBuilder = ImplementationFactory is null && ImplementationInstance is null
            ? _registrator.Services.RegisterType(ImplementationType)
            : null;
        
        if (ShouldOnlyRegisterIfNotRegistered)
        {   
            foreach (var serviceType in ServiceTypes)
            {
                if (ImplementationFactory is not null)
                {
                    factoryBuilder = factoryBuilder?.As(serviceType).IfNotRegistered(serviceType);
                }
                else if (ImplementationInstance is not null)
                {
                    instanceBuilder = instanceBuilder?.As(serviceType).IfNotRegistered(serviceType);
                }
                else if (ImplementationType is not null)
                {
                    typeBuilder = typeBuilder?.As(serviceType).IfNotRegistered(serviceType);
                }
            }
        }
        else
        {
            foreach (var serviceType in ServiceTypes)
            {
                if (ImplementationFactory is not null)
                {
                    factoryBuilder = factoryBuilder?.As(serviceType);
                }
                else if (ImplementationInstance is not null)
                {
                    instanceBuilder = instanceBuilder?.As(serviceType);
                }
                else if (ImplementationType is not null)
                {
                    typeBuilder = typeBuilder?.As(serviceType);
                }
            }
        }

        if (ShouldEnableInterfaceInterception)
        {
            factoryBuilder = factoryBuilder?.EnableInterfaceInterceptors();
            instanceBuilder = instanceBuilder?.EnableInterfaceInterceptors();
            typeBuilder = typeBuilder?.EnableInterfaceInterceptors();
        }

        foreach (var interceptor in Interceptors)
        {
            factoryBuilder = factoryBuilder?.InterceptedBy(interceptor);
            instanceBuilder = instanceBuilder?.InterceptedBy(interceptor);
            typeBuilder = typeBuilder?.InterceptedBy(interceptor);
        }

        return _registrator;
    }

    public override IRegistration EnableClassInterceptors()
    {
        throw new InvalidOperationException("Class interceptors are not supported");
    }

    public override IRegistration InterceptedBy(Type interceptorType)
    {
        var actual = IsAsyncInterceptor(interceptorType)
                ? typeof(AsyncInterceptorAdapter<>).MakeGenericType(interceptorType)
                : interceptorType;
        
        return base.InterceptedBy(actual);
    }
    
    /// <summary>
    /// Whether given interceptor is an async interceptor.
    /// </summary>
    private static bool IsAsyncInterceptor(Type interceptorCandidate) => interceptorCandidate.GetInterfaces().Any(x => x == typeof(IAsyncInterceptor));
}