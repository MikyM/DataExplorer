using Autofac;
using DataExplorer.MongoDb.Abstractions;
using DataExplorer.MongoDb.Abstractions.DataContexts;
using Microsoft.Extensions.DependencyInjection;
using ServiceLifetime = AttributeBasedRegistration.ServiceLifetime;

namespace DataExplorer.MongoDb;

/// <summary>
/// Configuration for EFCore Data Explorer.
/// </summary>
[PublicAPI]
public class DataExplorerMongoDbConfiguration
{
    /// <summary>
    /// Creates an instance of the configuration class.
    /// </summary>
    /// <param name="builder"></param>
    public DataExplorerMongoDbConfiguration(ContainerBuilder builder)
    {
        Builder = builder;
    }

    /// <summary>
    /// Creates an instance of the configuration class.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="serviceCollection"></param>
    public DataExplorerMongoDbConfiguration(ContainerBuilder? builder, IServiceCollection? serviceCollection)
    {
        Builder = builder;
        ServiceCollection = serviceCollection;
    }
    
    /// <summary>
    /// Creates an instance of the configuration class.
    /// </summary>
    /// <param name="serviceCollection"></param>
    public DataExplorerMongoDbConfiguration(IServiceCollection serviceCollection)
    {
        ServiceCollection = serviceCollection;
    }

    internal readonly ContainerBuilder? Builder;
    internal readonly IServiceCollection? ServiceCollection;

    private Dictionary<Type,MongoDbContextOptions> _contextOptions = new();
    /// <summary>
    /// Connection settings where the key is the type of the <see cref="MongoDbContext"/> the settings were registered for.
    /// </summary>
    public IReadOnlyDictionary<Type,MongoDbContextOptions> ContextOptions => _contextOptions;

    /// <summary>
    /// Disables the insertion of audit log entries.
    /// </summary>
    /// 
    public bool DisableOnBeforeSaveChanges { get; set; }
    
    private Dictionary<string, Func<IMongoUnitOfWork, Task>>? _onBeforeSaveChangesActions;

    /// <summary>
    /// Whether to cache include expressions (queries are evaluated faster).
    /// </summary>
    public bool EnableIncludeCache { get; set; }
    
    /// <summary>
    /// Actions to execute before each commit.
    /// </summary>
    public Dictionary<string, Func<IMongoUnitOfWork, Task>>? OnBeforeSaveChangesActions
         => _onBeforeSaveChangesActions;
    
    /// <summary>
    /// Gets or sets the default lifetime for base generic data services.
    /// </summary>
    public ServiceLifetime BaseGenericDataServiceLifetime { get; set; } = ServiceLifetime.InstancePerLifetimeScope;
    
    /// <summary>
    /// Gets or sets the default lifetime for custom data services that implement or derive from base data services.
    /// </summary>
    public ServiceLifetime DataServiceLifetime { get; set; } = ServiceLifetime.InstancePerLifetimeScope;
    
    /// <summary>
    /// Gets data interceptor registration delegates.
    /// </summary>
    public Dictionary<Type, (DataRegistrationStrategy Strategy, int Order)> DataInterceptors { get; private set; } = new();
    
    /// <summary>
    /// Gets data decorator registration delegates.
    /// </summary>
    public Dictionary<Type, int> DataDecorators { get; private set; } = new();

    /// <summary>
    /// Adds an on before save changes action for a given context.
    /// </summary>
    /// <param name="action">Action to perform</param>
    /// <typeparam name="TContext">Type of the context for the action.</typeparam>
    /// <exception cref="NotSupportedException">Throw when trying to register second action for same context type.</exception>
    public void AddOnBeforeSaveChangesAction<TContext>(Func<IMongoUnitOfWork, Task> action)
        where TContext : IMongoDbContext
    {
        _onBeforeSaveChangesActions ??= new Dictionary<string, Func<IMongoUnitOfWork, Task>>();
        
        if (_onBeforeSaveChangesActions.TryGetValue(typeof(TContext).Name, out _))
            throw new NotSupportedException("Multiple actions for same context aren't supported");
        
        _onBeforeSaveChangesActions.Add(typeof(TContext).Name, action);
    }

    /// <summary>
    /// Adds the interface of a database context as a service.
    /// </summary>
    /// <returns>Current <see cref="DataExplorerMongoDbConfiguration"/> instance.</returns>
    public DataExplorerMongoDbConfiguration AddDbContext<TContextInterface, TContextImplementation>(Action<MongoDbContextOptions<TContextImplementation>> contextOptions, ServiceLifetime lifetime = ServiceLifetime.InstancePerLifetimeScope) where TContextInterface : class, IMongoDbContext
        where TContextImplementation : MongoDbContext, TContextInterface
    {
        var opt = new MongoDbContextOptions<TContextImplementation>();
        contextOptions(opt);

        opt.ContextType = typeof(TContextImplementation);

        if (!_contextOptions.TryAdd(typeof(TContextImplementation), opt))
            throw new InvalidOperationException("You tried to register same db context twice");
        
        Builder?.RegisterInstance(opt).As<MongoDbContextOptions<TContextImplementation>>();
        ServiceCollection?.AddSingleton(opt);
        
        switch (lifetime)
        {
            case ServiceLifetime.SingleInstance:
                Builder?.RegisterType<TContextImplementation>()
                    .As<TContextInterface>()
                    .AsSelf()
                    .WithParameter(
                        (x, y) => x.ParameterType == typeof(MongoDbContextOptions) ||
                                  x.ParameterType == typeof(MongoDbContextOptions<TContextImplementation>),
                        (x, y) => y.Resolve<MongoDbContextOptions<TContextImplementation>>())
                    .SingleInstance();
                ServiceCollection?.AddSingleton<TContextImplementation>(x =>
                    ActivatorUtilities.CreateInstance<TContextImplementation>(x,
                        x.GetRequiredService<MongoDbContextOptions<TContextImplementation>>()));
                ServiceCollection?.AddSingleton<TContextInterface>(x => x.GetRequiredService<TContextImplementation>());
                break;
            case ServiceLifetime.InstancePerRequest:
                Builder?.RegisterType<TContextImplementation>()
                    .As<TContextInterface>()
                    .AsSelf()
                    .WithParameter(
                        (x, y) => x.ParameterType == typeof(MongoDbContextOptions) ||
                                  x.ParameterType == typeof(MongoDbContextOptions<TContextImplementation>),
                        (x, y) => y.Resolve<MongoDbContextOptions<TContextImplementation>>())
                    .InstancePerRequest();
                ServiceCollection?.AddScoped<TContextImplementation>(x =>
                    ActivatorUtilities.CreateInstance<TContextImplementation>(x,
                        x.GetRequiredService<MongoDbContextOptions<TContextImplementation>>()));
                ServiceCollection?.AddScoped<TContextInterface>(x => x.GetRequiredService<TContextImplementation>());
                break;
            case ServiceLifetime.InstancePerLifetimeScope:
                Builder?.RegisterType<TContextImplementation>()
                    .As<TContextInterface>()
                    .AsSelf()
                    .WithParameter(
                        (x, y) => x.ParameterType == typeof(MongoDbContextOptions) ||
                                  x.ParameterType == typeof(MongoDbContextOptions<TContextImplementation>),
                        (x, y) => y.Resolve<MongoDbContextOptions<TContextImplementation>>())
                    .InstancePerLifetimeScope();
                ServiceCollection?.AddScoped<TContextImplementation>(x =>
                    ActivatorUtilities.CreateInstance<TContextImplementation>(x,
                        x.GetRequiredService<MongoDbContextOptions<TContextImplementation>>()));
                ServiceCollection?.AddScoped<TContextInterface>(x => x.GetRequiredService<TContextImplementation>());
                break;
            case ServiceLifetime.InstancePerMatchingLifetimeScope:
                throw new NotSupportedException();
            case ServiceLifetime.InstancePerDependency:
                Builder?.RegisterType<TContextImplementation>()
                    .As<TContextInterface>()
                    .AsSelf()
                    .WithParameter(
                        (x, y) => x.ParameterType == typeof(MongoDbContextOptions) ||
                                  x.ParameterType == typeof(MongoDbContextOptions<TContextImplementation>),
                        (x, y) => y.Resolve<MongoDbContextOptions<TContextImplementation>>())
                    .InstancePerDependency();
                ServiceCollection?.AddTransient<TContextImplementation>(x =>
                    ActivatorUtilities.CreateInstance<TContextImplementation>(x,
                        x.GetRequiredService<MongoDbContextOptions<TContextImplementation>>()));
                ServiceCollection?.AddTransient<TContextInterface>(x => x.GetRequiredService<TContextImplementation>());
                break;
            case ServiceLifetime.InstancePerOwned:
                throw new NotSupportedException();
            default:
                throw new ArgumentOutOfRangeException(nameof(lifetime), lifetime, null);
        }

        return this;
    }

    /// <summary>
    /// Marks an interceptor of a given type to be used for intercepting base data services.
    /// </summary>
    /// <param name="registrationOrder">Registration order.</param>
    /// <param name="interceptor">Type of the interceptor.</param>
    /// <param name="strategy">Interceptor configuration.</param>
    /// <returns>Current instance of the <see cref="DataExplorerConfiguration"/>.</returns>
    public virtual DataExplorerMongoDbConfiguration AddDataServiceInterceptor(int registrationOrder, Type interceptor, DataRegistrationStrategy strategy = DataRegistrationStrategy.CrudAndReadOnly)
    {
        if (Builder is null)
            throw new NotSupportedException("Supported only when used with Autofac");
        
        DataInterceptors.TryAdd(interceptor ?? throw new ArgumentNullException(nameof(interceptor)), new (strategy, registrationOrder));
        return this;
    }
    
    /// <summary>
    /// Marks an interceptor of a given type to be used for intercepting base data services.
    /// </summary>
    /// <param name="strategy">Interceptor configuration.</param>
    /// <param name="registrationOrder">Registration order.</param>
    /// <typeparam name="T">Interceptor type.</typeparam>
    /// <returns>Current instance of the <see cref="DataExplorerConfiguration"/>.</returns>
    public virtual DataExplorerMongoDbConfiguration AddDataServiceInterceptor<T>(int registrationOrder, DataRegistrationStrategy strategy = DataRegistrationStrategy.CrudAndReadOnly) where T : notnull
    {
        if (Builder is null)
            throw new NotSupportedException("Supported only when used with Autofac");
        
        DataInterceptors.TryAdd(typeof(T), new (strategy, registrationOrder));
        return this;
    }
    
    /// <summary>
    /// Marks a decorator of a given type to be used for decorating base data services.
    /// </summary>
    /// <param name="registrationOrder">Registration order.</param>
    /// <param name="decorator">Type of the decorator.</param>
    /// <returns>Current instance of the <see cref="DataExplorerConfiguration"/>.</returns>
    public virtual DataExplorerMongoDbConfiguration AddDataServiceDecorator(int registrationOrder, Type decorator)
    {
        if (Builder is null)
            throw new NotSupportedException("Supported only when used with Autofac");
        
        DataDecorators.TryAdd(decorator ?? throw new ArgumentNullException(nameof(decorator)), registrationOrder);
        return this;
    }
}
