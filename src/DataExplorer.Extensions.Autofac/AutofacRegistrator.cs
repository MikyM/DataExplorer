using System.Linq.Expressions;
using Autofac;
using DataExplorer.Abstractions;
using JetBrains.Annotations;

namespace DataExplorer.Extensions.Autofac;

[PublicAPI]
public class AutofacRegistrator : IRegistrator
{
    public AutofacRegistrator(ContainerBuilder services)
    {
        Services = services;
    }
    
    public ContainerBuilder Services { get; }


    public IRegistration Describe(Type implementationType)
        => new AutofacRegistration(this, implementationType);

    public IRegistration DescribeOpenGeneric(Type implementationType)
        => new AutofacRegistration(this, implementationType);

    public IRegistration DescribeInstance(object instance)
        => new AutofacRegistration(this, instance);

    public IRegistration DescribeFactory(Expression<Func<IResolver, object>> factory, Type implementationType)
        => new AutofacRegistration(this, factory, implementationType);

    public bool SupportsDecoration => true;
    
    public IRegistration DescribeDecorator(Type decoratorType, Type decoratedService)
    {
        throw new NotImplementedException();
    }

    public IRegistration DescribeOpenGenericDecorator(Type decoratorType, Type decoratedService)
    {
        throw new NotImplementedException();
    }

    public bool SupportsInterception => true;
    public bool SupportsNonPublicConstructors => true;
}