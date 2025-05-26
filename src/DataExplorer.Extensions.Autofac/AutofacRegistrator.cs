using System.Linq.Expressions;
using Autofac;
using DataExplorer.Abstractions;
using JetBrains.Annotations;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

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

    public IRegistrator DescribeOptions<TOptions>(TOptions instance) where TOptions : class
    {
        var options = Options.Create(instance);
        
        Services.RegisterInstance(options).As<IOptions<TOptions>>().SingleInstance().IfNotRegistered(typeof(IOptions<TOptions>));
        Services.Register(x => x.Resolve<IOptions<TOptions>>().Value).As<TOptions>().SingleInstance().IfNotRegistered(typeof(TOptions));
        
        return this;
    }

    public IRegistrator DescribeOptions<TOptions>(Action<TOptions> action, TOptions instance) where TOptions : class
    {
        action(instance);
        
        var options = Options.Create(instance);
        
        Services.RegisterInstance(options).As<IOptions<TOptions>>().SingleInstance().IfNotRegistered(typeof(IOptions<TOptions>));
        Services.Register(x => x.Resolve<IOptions<TOptions>>().Value).As<TOptions>().SingleInstance().IfNotRegistered(typeof(TOptions));
        
        return this;
    }

    public IRegistrator DescribeHostedService<THostedService>() where THostedService : class, IHostedService
    {
        Services.RegisterType<THostedService>().As<IHostedService>().SingleInstance();
        return this;
    }

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