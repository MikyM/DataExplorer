using System.Linq.Expressions;
using DataExplorer.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

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

    public IRegistrator DescribeOptions<TOptions>(TOptions instance) where TOptions : class
    {
        var options = Options.Create(instance);
        
        Services.TryAddSingleton(typeof(IOptions<TOptions>), x => options);
        Services.TryAddSingleton(instance);
        
        return this;
    }

    public IRegistrator DescribeOptions<TOptions>(Action<TOptions> action, TOptions instance) where TOptions : class
    {
        Services.AddOptions<TOptions>().Configure(action);
        Services.TryAddSingleton(instance);
        
        return this;
    }

    public IRegistrator DescribeHostedService<THostedService>() where THostedService : class, IHostedService
    {
        Services.AddHostedService<THostedService>();
        return this;
    }

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