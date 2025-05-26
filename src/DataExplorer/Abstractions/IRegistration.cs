using AttributeBasedRegistration;

namespace DataExplorer.Abstractions;

public interface IRegistration
{
    IRegistrator Register();
    
    IRegistration IfNotRegistered();
    
    IRegistration AsImplementedInterfaces();
    
    IRegistration WithLifetime(ServiceLifetime lifetime);
    
    IRegistration As(Type serviceType);
    
    IRegistration InterceptedBy(Type interceptorType);
        
    /// <summary>
    /// Enables interface interceptors.
    /// </summary>
    /// <returns></returns>
    IRegistration EnableInterfaceInterceptors();
    
    /// <summary>
    /// Enables interface interceptors.
    /// </summary>
    /// <returns></returns>
    IRegistration EnableClassInterceptors();

    IRegistration AsConventionNamedInterface();
    IRegistration AsDirectAncestorInterfaces();
    IRegistration AsSelf();
    IRegistration WithMatchingLifetime(object[] tags);
    IRegistration WithOwnedLifetime(Type ownedBy);
}