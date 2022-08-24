using System.Reflection;
using AttributeBasedRegistration;
using Autofac;
using DataExplorer.EfCore.Abstractions;
using DataExplorer.EfCore.Abstractions.DataContexts;
using DataExplorer.EfCore.DataContexts;
using DataExplorer.EfCore.Specifications.Evaluators;
using DataExplorer.EfCore.Specifications.Validators;
using Microsoft.Extensions.DependencyInjection;

namespace DataExplorer.EfCore;

/// <summary>
/// Configuration for EFCore Data Explorer.
/// </summary>
[PublicAPI]
public class DataExplorerEfCoreConfiguration
{
    /// <summary>
    /// Creates an instance of the configuration class.
    /// </summary>
    /// <param name="builder"></param>
    public DataExplorerEfCoreConfiguration(ContainerBuilder builder)
    {
        Builder = builder;
    }

    /// <summary>
    /// Creates an instance of the configuration class.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="serviceCollection"></param>
    public DataExplorerEfCoreConfiguration(ContainerBuilder? builder, IServiceCollection? serviceCollection)
    {
        Builder = builder;
        ServiceCollection = serviceCollection;
    }
    
    /// <summary>
    /// Creates an instance of the configuration class.
    /// </summary>
    /// <param name="serviceCollection"></param>
    public DataExplorerEfCoreConfiguration(IServiceCollection serviceCollection)
    {
        ServiceCollection = serviceCollection;
    }

    internal readonly ContainerBuilder? Builder;
    internal readonly IServiceCollection? ServiceCollection;
    
    /// <summary>
    /// Disables the insertion of audit log entries.
    /// </summary>
    /// 
    public bool DisableOnBeforeSaveChanges { get; set; }
    
    private Dictionary<string, Func<IUnitOfWork, Task>>? _onBeforeSaveChangesActions;

    /// <summary>
    /// Whether to cache include expressions (queries are evaluated faster).
    /// </summary>
    public bool EnableIncludeCache { get; set; } = false;

    /// <summary>
    /// Disables the insertion of audit log entries.
    /// </summary>

    /// <summary>
    /// Actions to execute before each commit.
    /// </summary>
    public Dictionary<string, Func<IUnitOfWork, Task>>? OnBeforeSaveChangesActions
         => _onBeforeSaveChangesActions;
    
    /// <summary>
    /// Gets or sets the default lifetime for base generic data services.
    /// </summary>
    public Lifetime BaseGenericDataServiceLifetime { get; set; } = Lifetime.InstancePerLifetimeScope;
    /// <summary>
    /// Gets or sets the default lifetime for custom data services that implement or derive from base data services.
    /// </summary>
    public Lifetime DataServiceLifetime { get; set; } = Lifetime.InstancePerLifetimeScope;
    
    /// <summary>
    /// Gets data interceptor registration delegates.
    /// </summary>
    public Dictionary<Type, DataInterceptorConfiguration> DataInterceptors { get; private set; } = new();

    /// <summary>
    /// Adds an on before save changes action for a given context.
    /// </summary>
    /// <param name="action">Action to perform</param>
    /// <typeparam name="TContext">Type of the context for the action.</typeparam>
    /// <exception cref="NotSupportedException">Throw when trying to register second action for same context type.</exception>
    public void AddOnBeforeSaveChangesAction<TContext>(Func<IUnitOfWork, Task> action)
        where TContext : AuditableEfDbContext
    {
        _onBeforeSaveChangesActions ??= new Dictionary<string, Func<IUnitOfWork, Task>>();
        
        if (_onBeforeSaveChangesActions.TryGetValue(typeof(TContext).Name, out _))
            throw new NotSupportedException("Multiple actions for same context aren't supported");
        
        _onBeforeSaveChangesActions.Add(typeof(TContext).Name, action);
    }

    /// <summary>
    /// Adds a given custom evaluator that implements <see cref="IEvaluator"/> interface.
    /// </summary>
    /// <typeparam name="TEvaluator">Type to register.</typeparam>
    /// <returns>Current <see cref="DataExplorerEfCoreConfiguration"/> instance.</returns>
    public DataExplorerEfCoreConfiguration AddEvaluator<TEvaluator>() where TEvaluator : class, IEvaluator
    {
        Builder?.RegisterType(typeof(TEvaluator))
            .As<IEvaluator>()
            .FindConstructorsWith(new AllConstructorsFinder())
            .SingleInstance();
        
        ServiceCollection?.AddSingleton(typeof(IEvaluator), _ => Activator.CreateInstance(
            typeof(TEvaluator),
            BindingFlags.Instance
            | BindingFlags.Public
            | BindingFlags.NonPublic,
            null,
            Array.Empty<object>(),
            null
        )!);


        return this;
    }

    /// <summary>
    /// Adds a given custom evaluator that implements <see cref="IEvaluator"/> interface.
    /// </summary>
    /// <param name="evaluator">Type of the custom evaluator.</param>
    /// <returns>Current <see cref="DataExplorerEfCoreConfiguration"/> instance.</returns>
    public DataExplorerEfCoreConfiguration AddEvaluator(Type evaluator)
    {
        if (evaluator.GetInterface(nameof(IEvaluator)) is null)
            throw new NotSupportedException("Registered evaluator did not implement IEvaluator interface");

        Builder?.RegisterType(evaluator)
            .As<IEvaluator>()
            .FindConstructorsWith(new AllConstructorsFinder())
            .SingleInstance();
        
        ServiceCollection?.AddSingleton(typeof(IEvaluator), _ => Activator.CreateInstance(
            evaluator,
            BindingFlags.Instance
            | BindingFlags.Public
            | BindingFlags.NonPublic,
            null,
            Array.Empty<object>(),
            null
        )!);

        return this;
    }

    /// <summary>
    /// Adds all evaluators that implement <see cref="IInMemoryEvaluator"/> from all assemblies.
    /// </summary>
    /// <returns>Current <see cref="DataExplorerEfCoreConfiguration"/> instance.</returns>
    public DataExplorerEfCoreConfiguration AddInMemoryEvaluators()
    {
        if (Builder is not null)
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                Builder?.RegisterAssemblyTypes(assembly)
                    .Where(x => x.GetInterface(nameof(IInMemoryEvaluator)) is not null)
                    .As<IInMemoryEvaluator>()
                    .FindConstructorsWith(new AllConstructorsFinder())
                    .SingleInstance();
            }
        
        if (ServiceCollection is not null)
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes().Where(x => x.GetInterface(nameof(IInMemoryEvaluator)) is not null))
                {
                    ServiceCollection.AddSingleton(typeof(IInMemoryEvaluator), _ => Activator.CreateInstance(
                        type,
                        BindingFlags.Instance
                        | BindingFlags.Public
                        | BindingFlags.NonPublic,
                        null,
                        Array.Empty<object>(),
                        null
                    )!);
                }
            }

        return this;
    }

    /// <summary>
    /// Adds all validators that implement <see cref="IValidator"/> from all assemblies.
    /// </summary>
    /// <returns>Current <see cref="DataExplorerEfCoreConfiguration"/> instance.</returns>
    public DataExplorerEfCoreConfiguration AddValidators()
    {
        if (Builder is not null)
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                Builder?.RegisterAssemblyTypes(assembly)
                    .Where(x => x.GetInterface(nameof(IValidator)) is not null)
                    .As<IValidator>()
                    .FindConstructorsWith(new AllConstructorsFinder())
                    .SingleInstance();
            }
        
        if (ServiceCollection is not null)
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes().Where(x => x.GetInterface(nameof(IValidator)) is not null))
                {
                    ServiceCollection.AddSingleton(typeof(IValidator), _ => Activator.CreateInstance(
                        type,
                        BindingFlags.Instance
                        | BindingFlags.Public
                        | BindingFlags.NonPublic,
                        null,
                        Array.Empty<object>(),
                        null
                    )!);
                }
            }

        return this;
    }

    /// <summary>
    /// Adds all evaluators that implement <see cref="IEvaluator"/> from all assemblies.
    /// </summary>
    /// <returns>Current <see cref="DataExplorerEfCoreConfiguration"/> instance.</returns>
    public DataExplorerEfCoreConfiguration AddEvaluators()
    {
        if (Builder is not null)
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly == typeof(IncludeEvaluator).Assembly)
                    continue;

                Builder?.RegisterAssemblyTypes(assembly)
                    .Where(x => x.GetInterface(nameof(IEvaluator)) is not null)
                    .As<IEvaluator>()
                    .FindConstructorsWith(new AllConstructorsFinder())
                    .SingleInstance();
            }
        
        if (ServiceCollection is not null)
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly == typeof(IncludeEvaluator).Assembly)
                    continue;
                
                foreach (var type in assembly.GetTypes().Where(x => x.GetInterface(nameof(IEvaluator)) is not null))
                {
                    ServiceCollection.AddSingleton(typeof(IEvaluator), _ => Activator.CreateInstance(
                        type,
                        BindingFlags.Instance
                        | BindingFlags.Public
                        | BindingFlags.NonPublic,
                        null,
                        Array.Empty<object>(),
                        null
                    )!);
                }
            }

        return this;
    }
    
    /// <summary>
    /// Adds the interface of a database context as a service.
    /// </summary>
    /// <returns>Current <see cref="DataExplorerEfCoreConfiguration"/> instance.</returns>
    public DataExplorerEfCoreConfiguration AddDbContext<TContextInterface, TContextImplementation>(Lifetime lifetime = Lifetime.InstancePerLifetimeScope) where TContextInterface : class, IEfDbContext
        where TContextImplementation : EfDbContext, TContextInterface
    {

        switch (lifetime)
        {
            case Lifetime.SingleInstance:
                Builder?.Register(x => x.Resolve<TContextImplementation>()).As<TContextInterface>()
                    .SingleInstance();
                ServiceCollection?.AddSingleton<TContextInterface>(x => x.GetRequiredService<TContextImplementation>());
                break;
            case Lifetime.InstancePerRequest:
                Builder?.Register(x => x.Resolve<TContextImplementation>()).As<TContextInterface>()
                    .InstancePerRequest();
                ServiceCollection?.AddScoped<TContextInterface>(x => x.GetRequiredService<TContextImplementation>());
                break;
            case Lifetime.InstancePerLifetimeScope:
                Builder?.Register(x => x.Resolve<TContextImplementation>()).As<TContextInterface>()
                    .InstancePerLifetimeScope();
                ServiceCollection?.AddScoped<TContextInterface>(x => x.GetRequiredService<TContextImplementation>());
                break;
            case Lifetime.InstancePerMatchingLifetimeScope:
                throw new NotSupportedException();
            case Lifetime.InstancePerDependency:
                Builder?.Register(x => x.Resolve<TContextImplementation>()).As<TContextInterface>()
                    .InstancePerDependency();
                ServiceCollection?.AddTransient<TContextInterface>(x => x.GetRequiredService<TContextImplementation>());
                break;
            case Lifetime.InstancePerOwned:
                throw new NotSupportedException();
            default:
                throw new ArgumentOutOfRangeException(nameof(lifetime), lifetime, null);
        }

        return this;
    }
    
    /// <summary>
    /// Marks an interceptor of a given type to be used for intercepting base data services.
    /// </summary>
    /// <param name="interceptor">Type of the interceptor.</param>
    /// <param name="configuration">Interceptor configuration.</param>
    /// <returns>Current instance of the <see cref="DataExplorerConfiguration"/>.</returns>
    public virtual DataExplorerEfCoreConfiguration AddDataServiceInterceptor(Type interceptor, DataInterceptorConfiguration configuration = DataInterceptorConfiguration.CrudAndReadOnly)
    {
        if (Builder is null)
            throw new NotSupportedException("Supported only when used with Autofac");
        
        DataInterceptors.TryAdd(interceptor ?? throw new ArgumentNullException(nameof(interceptor)), configuration);
        return this;
    }
    /// <summary>
    /// Marks an interceptor of a given type to be used for intercepting base data services.
    /// </summary>
    /// <param name="configuration">Interceptor configuration.</param>
    /// <returns>Current instance of the <see cref="DataExplorerConfiguration"/>.</returns>
    public virtual DataExplorerEfCoreConfiguration AddDataServiceInterceptor<T>(DataInterceptorConfiguration configuration = DataInterceptorConfiguration.CrudAndReadOnly) where T : notnull
    {
        if (Builder is null)
            throw new NotSupportedException("Supported only when used with Autofac");
        
        DataInterceptors.TryAdd(typeof(T), configuration);
        return this;
    }
}
