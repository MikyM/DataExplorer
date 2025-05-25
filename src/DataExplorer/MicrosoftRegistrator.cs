using System.Linq.Expressions;
using DataExplorer.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace DataExplorer;

[PublicAPI]
public class MicrosoftRegistrator : IRegistrator
{
    public MicrosoftRegistrator(IServiceCollection services)
    {
        Services = services;
    }
    
    public IServiceCollection Services { get; }


    public IRegistration Describe(Type implementationType)
        => new MicrosoftRegistration(this, implementationType);

    public IRegistration DescribeOpenGeneric(Type implementationType)
        => new MicrosoftRegistration(this, implementationType);

    public IRegistration DescribeInstance(object instance)
        => new MicrosoftRegistration(this, instance);

    public IRegistration DescribeFactory(Expression<Func<IResolver, object>> factory, Type implementationType)
        => new MicrosoftRegistration(this, factory, implementationType);

    public bool SupportsDecoration => false;
    
    public IRegistration DescribeDecorator(Type decoratorType, Type decoratedService)
    {
        throw new NotImplementedException();
    }

    public IRegistration DescribeOpenGenericDecorator(Type decoratorType, Type decoratedService)
    {
        throw new NotImplementedException();
    }

    public bool SupportsInterception => false;
    public bool SupportsNonPublicConstructors => false;
}