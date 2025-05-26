using System.Linq.Expressions;
using System.Reflection;
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

    private static readonly MethodInfo ResolveMethod = typeof(ResolutionExtensions).GetMethods()
                                                           .Where(x => x is
                                                           {
                                                               Name: nameof(ResolutionExtensions.Resolve),
                                                               IsGenericMethodDefinition: true
                                                           })
                                                           .FirstOrDefault(x => x.GetParameters().Length == 1) ??
                                                       throw new InvalidOperationException(
                                                           "Could not find get required service method.");
        
    private static readonly MethodInfo ResolveKeyedMethod = typeof(ResolutionExtensions).GetMethods().Where(x =>
            x is
            {
                Name: nameof(ResolutionExtensions.ResolveKeyed),
                IsGenericMethodDefinition: true
            }).FirstOrDefault(x => x.GetParameters().Length == 2) ?? throw new InvalidOperationException("Could not find get required service method.")
                                                                           ?? throw new InvalidOperationException(
                                                                               "Could not find get required service method.");
    
    private static Func<IComponentContext, object> TranslateFactory(Expression<Func<IResolver, object>> factory)
    {
        var visitor = new ReplaceResolverExpressionVisitor<IComponentContext>((x, y) =>
        {
            var actualMethod = ResolveMethod.MakeGenericMethod(y.Method.ReturnType);

            var methodCall = Expression.Call(null, actualMethod, x);
        
            return methodCall;
        }, (x, y) =>
        {
            var actualMethod = ResolveKeyedMethod.MakeGenericMethod(y.Method.ReturnType);

            var methodCall = Expression.Call(null, actualMethod, x, y.Arguments.First(e => e.Type != typeof(IResolver)));
            
            return methodCall;
        });

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
        
        var simpleTypeBuilder = ImplementationFactory is null && ImplementationInstance is null
            ? IsOpenGeneric 
                ? null
                : _registrator.Services.RegisterType(ImplementationType)
            : null;
        
        var genericTypeBuilder = ImplementationFactory is null && ImplementationInstance is null
            ? IsOpenGeneric 
                ? _registrator.Services.RegisterGeneric(ImplementationType)
                : null
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
                    simpleTypeBuilder = simpleTypeBuilder?.As(serviceType).IfNotRegistered(serviceType);
                    genericTypeBuilder = genericTypeBuilder?.As(serviceType).IfNotRegistered(serviceType);
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
                    simpleTypeBuilder = simpleTypeBuilder?.As(serviceType);
                    genericTypeBuilder = genericTypeBuilder?.As(serviceType);
                }
            }
        }

        if (ShouldEnableInterfaceInterception)
        {
            factoryBuilder = factoryBuilder?.EnableInterfaceInterceptors();
            instanceBuilder = instanceBuilder?.EnableInterfaceInterceptors();
            simpleTypeBuilder = simpleTypeBuilder?.EnableInterfaceInterceptors();
            genericTypeBuilder = genericTypeBuilder?.EnableInterfaceInterceptors();
        }

        foreach (var interceptor in Interceptors)
        {
            factoryBuilder = factoryBuilder?.InterceptedBy(interceptor);
            instanceBuilder = instanceBuilder?.InterceptedBy(interceptor);
            simpleTypeBuilder = simpleTypeBuilder?.InterceptedBy(interceptor);
            genericTypeBuilder = genericTypeBuilder?.InterceptedBy(interceptor);
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