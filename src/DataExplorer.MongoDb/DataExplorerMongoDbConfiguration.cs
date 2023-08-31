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
public class DataExplorerMongoDbConfiguration : DataExplorerConfigurationBase
{
    /// <summary>
    /// Creates an instance of the configuration class.
    /// </summary>
    internal DataExplorerMongoDbConfiguration(DataExplorerConfigurationBase configurationBase) : base(configurationBase)
    {
    }
    
    /// <summary>
    /// Gets the container builder.
    /// </summary>
    internal ContainerBuilder? GetContainerBuilder()
        => Builder;
    
    /// <summary>
    /// Gets the service collection.
    /// </summary>
    internal IServiceCollection? GetServiceCollection()
        => ServiceCollection;

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

    /// <summary>
    /// Whether to cache include expressions (queries are evaluated faster), defaults to true.
    /// </summary>
    public bool EnableIncludeCache { get; set; } = true;
    
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
    /// <typeparam name="TInterceptor">Interceptor type.</typeparam>
    /// <returns>Current instance of the <see cref="DataExplorerConfiguration"/>.</returns>
    public virtual DataExplorerMongoDbConfiguration AddDataServiceInterceptor<TInterceptor>(int registrationOrder, DataRegistrationStrategy strategy = DataRegistrationStrategy.CrudAndReadOnly) where TInterceptor : notnull
    {
        if (Builder is null)
            throw new NotSupportedException("Supported only when used with Autofac");
        
        DataInterceptors.TryAdd(typeof(TInterceptor), new (strategy, registrationOrder));
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
        
        if (decorator is { IsGenericType: false, IsGenericTypeDefinition: false })
            throw new ArgumentException("Decorator must be a generic type definition", nameof(decorator));
        
        DataDecorators.TryAdd(decorator ?? throw new ArgumentNullException(nameof(decorator)), registrationOrder);
        return this;
    }
    
    /// <summary>
    /// Marks a decorator of a given type to be used for decorating base data services.
    /// </summary>
    /// <param name="registrationOrder">Registration order.</param>
    /// <returns>Current instance of the <see cref="DataExplorerConfiguration"/>.</returns>
    public virtual DataExplorerMongoDbConfiguration AddDataServiceDecorator<TDecorator>(int registrationOrder)
    {
        if (Builder is null)
            throw new NotSupportedException("Supported only when used with Autofac");

        var decorator = typeof(TDecorator);
        
        if (decorator is { IsGenericType: false, IsGenericTypeDefinition: false })
            throw new ArgumentException("Decorator must be a generic type definition", nameof(TDecorator));
        
        DataDecorators.TryAdd(decorator ?? throw new ArgumentNullException(nameof(TDecorator)), registrationOrder);
        return this;
    }
}
