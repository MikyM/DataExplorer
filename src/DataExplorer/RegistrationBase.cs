using System.Linq.Expressions;
using AttributeBasedRegistration.Extensions;
using DataExplorer.Abstractions;
using ServiceLifetime = AttributeBasedRegistration.ServiceLifetime;

namespace DataExplorer;

[PublicAPI]
public abstract class RegistrationBase : IRegistration
{
    public IRegistrator Registrator { get; private set; }
    
    public Type ImplementationType { get; private set; }
    
    public object? ImplementationInstance { get; private set; }
    
    public Expression<Func<IResolver, object>>? ImplementationFactory { get; private set; }
    
    public bool RegisteringAsImplementedInterfaces { get; private set; }
    public bool RegisteringAsDirectAncestorInterfaces { get; private set; }
    public bool RegisteringAsSelf { get; private set; }
    public bool RegisteringAsConventionNamedInterface { get; private set; }
    
    public bool IsOpenGeneric { get; private set; }
    
    public ServiceLifetime? Lifetime { get; private set; }
    
    public bool ShouldOnlyRegisterIfNotRegistered { get; private set; }
    
    private HashSet<Type> _serviceTypes = new();
    
    public IReadOnlyList<Type> ServiceTypes => _serviceTypes.ToList().AsReadOnly();
    
    private HashSet<Type> _interceptorTypes = new();

    public IReadOnlyList<Type> Interceptors => _interceptorTypes.ToList().AsReadOnly();
    
    public bool ShouldEnableInterfaceInterception { get; protected set; }
    
    public bool ShouldEnableClassInterception { get; protected set; }
    
    public object[]? Tags { get; private set; } = [];
    public Type? Owned { get; private set; }
    
    protected RegistrationBase(IRegistrator registrator, Type implementationType)
    {
        Registrator = registrator;
        ImplementationType = implementationType;

        IsOpenGeneric = implementationType is { IsGenericType: true, IsGenericTypeDefinition: true };
    }
    
    protected RegistrationBase(IRegistrator registrator, object implementationInstance)
    {
        Registrator = registrator;
        ImplementationInstance = implementationInstance;
        ImplementationType = implementationInstance.GetType();
        
        IsOpenGeneric = ImplementationType is { IsGenericType: true, IsGenericTypeDefinition: true };
    }
    
    protected RegistrationBase(IRegistrator registrator, Expression<Func<IResolver, object>> factory, Type implementationType)
    {
        Registrator = registrator;
        ImplementationFactory = factory;
        ImplementationType = implementationType;
        
        IsOpenGeneric = ImplementationType is { IsGenericType: true, IsGenericTypeDefinition: true };
    }
    
    private void VerifyRegistration()
    {
        if (ImplementationType is null && ImplementationInstance is null && ImplementationFactory is null)
        {
            throw new InvalidOperationException("Registration registration requires an implementation instance, an implementation type or a factory");
        }

        if (Lifetime is null)
        {
            throw new InvalidOperationException("Registration registration requires a lifetime, default value is not accepted");
        }

        if (ServiceTypes.Count == 0)
        {
            throw new InvalidOperationException("At least one service type must be registered");
        }

        if (Interceptors.Count != 0 && !ShouldEnableInterfaceInterception)
        {
            throw new InvalidOperationException("Interceptors require enabling interface interception");
        }

        if (Interceptors.Any(x => !x.IsInterface))
        {
            throw new InvalidOperationException("Interception only supports interface interception");
        }

        if (ImplementationType is not null && (ImplementationType.IsAbstract || ImplementationType.IsInterface))
        {
            throw new InvalidOperationException($"Implementation type cannot be abstract or an interface: {ImplementationType.Name}");
        }
    }

    public abstract IRegistrator RegisterImplementation();

    public IRegistrator Register()
    {
        VerifyRegistration();

        RegisterImplementation();
        
        return Registrator;
    }

    public IRegistration IfNotRegistered()
    {
        ShouldOnlyRegisterIfNotRegistered = false;
        
        return this;
    }

    public IRegistration AsImplementedInterfaces()
    {
        RegisteringAsImplementedInterfaces = true;

        var implemented = ImplementationType.GetInterfaces();

        foreach (var implementedInterface in implemented)
        {
            _serviceTypes.Add(implementedInterface);
        }
        
        return this;
    }
    
    public IRegistration AsConventionNamedInterface()
    {
        RegisteringAsConventionNamedInterface = true;
        
        var conv = ImplementationType.GetInterfaceByNamingConvention();

        if (conv is null)
        {
            throw new InvalidOperationException($"No convention named interface found on type {ImplementationType.Name}");
        }
        
        _serviceTypes.Add(conv);
        
        return this;
    }
    
    public IRegistration AsDirectAncestorInterfaces()
    {
        RegisteringAsDirectAncestorInterfaces = true;

        var direct = ImplementationType.GetDirectInterfaceAncestors();

        foreach (var directType in direct)
        {
            _interceptorTypes.Add(directType);
        }
        
        return this;
    }
    
    public IRegistration AsSelf()
    {
        RegisteringAsSelf = true;

        _serviceTypes.Add(ImplementationType);
        
        return this;
    }

    public IRegistration WithLifetime(ServiceLifetime lifetime)
    {
        Lifetime = lifetime;
        
        return this;
    }
    
    public virtual IRegistration WithMatchingLifetime(object[] tags)
    {
        Lifetime = ServiceLifetime.InstancePerMatchingLifetimeScope;
        Tags = tags;
        
        return this;
    }
    
    public virtual IRegistration WithOwnedLifetime(Type ownedBy)
    {
        Lifetime = ServiceLifetime.InstancePerOwned;
        
        Owned = ownedBy;
        
        return this;
    }

    public IRegistration As(Type serviceType)
    {
        _serviceTypes.Add(serviceType);
        
        return this;
    }

    public virtual IRegistration InterceptedBy(Type interceptorType)
    {
        _interceptorTypes.Add(interceptorType);
        
        return this;
    }

    public virtual IRegistration EnableInterfaceInterceptors()
    {
        ShouldEnableInterfaceInterception = true;
        
        return this;
    }
    
    public virtual IRegistration EnableClassInterceptors()
    {
        ShouldEnableClassInterception = true;
        
        return this;
    }
}