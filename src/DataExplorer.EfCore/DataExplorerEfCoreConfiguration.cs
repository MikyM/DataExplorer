using DataExplorer.Abstractions;
using ServiceLifetime = AttributeBasedRegistration.ServiceLifetime;

namespace DataExplorer.EfCore;

/// <summary>
/// Configuration for EFCore Data Explorer.
/// </summary>
[PublicAPI]
public class DataExplorerEfCoreConfiguration : DataExplorerConfigurationBase, IOptions<DataExplorerEfCoreConfiguration>
{
    /// <summary>
    /// Creates an instance of the configuration class.
    /// </summary>
    public DataExplorerEfCoreConfiguration(DataExplorerConfigurationBase configurationBase) : base(configurationBase)
    {
    }
    
    /// <summary>
    /// Creates an instance of the configuration class.
    /// </summary>
    public DataExplorerEfCoreConfiguration(IRegistrator registrator) : base(registrator)
    {
    }
    
    
    /// <summary>
    /// Gets the registrator.
    /// </summary>
    internal IRegistrator GetRegistrator()
        => Registrator;
    
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
    /// Whether to use <see cref="DateTime.Now"/> or <see cref="DateTime.UtcNow"/> for <see cref="IUpdatedAt"/> and <see cref="ICreatedAt"/> entities. Defaults to UtcNow.
    /// </summary>
    public DateTimeStrategy DateTimeStrategy { get; set; } = DateTimeStrategy.UtcNow;

    /// <summary>
    /// Gets or sets the default lifetime for base generic data services.
    /// </summary>
    public ServiceLifetime BaseGenericDataServiceLifetime { get; set; } = ServiceLifetime.InstancePerLifetimeScope;
    /// <summary>
    /// Gets or sets the default lifetime for custom data services that implement or derive from base data services.
    /// </summary>
    public ServiceLifetime DataServiceLifetime { get; set; } = ServiceLifetime.InstancePerLifetimeScope;
    
    /// <summary>
    /// Gets registered generic (built-in) data service interceptors.
    /// </summary>
    public Dictionary<Type, (DataRegistrationStrategy Strategy, int Order)> GenericDataInterceptors { get; private set; } = new();
    
    /// <summary>
    /// Gets registered custom data service interceptors.
    /// </summary>
    public Dictionary<Type, int> CustomDataInterceptors { get; private set; } = new();
    
    /// <summary>
    /// Gets registered generic (built-in) data service decorators.
    /// </summary>
    public Dictionary<Type, int> GenericDataDecorators { get; private set; } = new();
    
    /// <summary>
    /// Gets registered custom data service decorators.
    /// </summary>
    public Dictionary<Type, int> CustomDataDecorators { get; private set; } = new();

    /// <summary>
    /// Marks an interceptor of a given type to be used for intercepting base data services.
    /// </summary>
    /// <param name="registrationOrder">Registration order.</param>
    /// <param name="interceptor">Type of the interceptor.</param>
    /// <param name="strategy">Interceptor configuration.</param>
    /// <returns>Current instance of the <see cref="DataExplorerConfiguration"/>.</returns>
    public virtual DataExplorerEfCoreConfiguration AddGenericDataServiceInterceptor(int registrationOrder, Type interceptor, DataRegistrationStrategy strategy = DataRegistrationStrategy.CrudAndReadOnly)
    {
        if (!Registrator.SupportsInterception)
            throw new NotSupportedException("Supported only when used with a registrator that supports interception");
        
        GenericDataInterceptors.TryAdd(interceptor ?? throw new ArgumentNullException(nameof(interceptor)), new (strategy, registrationOrder));
        return this;
    }
    
    /// <summary>
    /// Marks an interceptor of a given type to be used for intercepting base data services.
    /// </summary>
    /// <param name="strategy">Interceptor configuration.</param>
    /// <param name="registrationOrder">Registration order.</param>
    /// <typeparam name="TInterceptor">Interceptor type.</typeparam>
    /// <returns>Current instance of the <see cref="DataExplorerConfiguration"/>.</returns>
    public virtual DataExplorerEfCoreConfiguration AddGenericDataServiceInterceptor<TInterceptor>(int registrationOrder, DataRegistrationStrategy strategy = DataRegistrationStrategy.CrudAndReadOnly) where TInterceptor : notnull
    {
        if (!Registrator.SupportsInterception)
            throw new NotSupportedException("Supported only when used with a registrator that supports interception");
        
        GenericDataInterceptors.TryAdd(typeof(TInterceptor), new (strategy, registrationOrder));
        return this;
    }
    
    /// <summary>
    /// Marks an interceptor of a given type to be used for intercepting custom data services.
    /// </summary>
    /// <param name="registrationOrder">Registration order.</param>
    /// <param name="interceptor">Type of the interceptor.</param>
    /// <returns>Current instance of the <see cref="DataExplorerConfiguration"/>.</returns>
    public virtual DataExplorerEfCoreConfiguration AddCustomDataServiceInterceptor(int registrationOrder, Type interceptor)
    {
        if (!Registrator.SupportsInterception)
            throw new NotSupportedException("Supported only when used with a registrator that supports interception");
        
        CustomDataInterceptors.TryAdd(interceptor ?? throw new ArgumentNullException(nameof(interceptor)), registrationOrder);
        return this;
    }
    
    /// <summary>
    /// Marks an interceptor of a given type to be used for intercepting custom data services.
    /// </summary>
    /// <param name="registrationOrder">Registration order.</param>
    /// <typeparam name="TInterceptor">Interceptor type.</typeparam>
    /// <returns>Current instance of the <see cref="DataExplorerConfiguration"/>.</returns>
    public virtual DataExplorerEfCoreConfiguration AddCustomDataServiceInterceptor<TInterceptor>(int registrationOrder) where TInterceptor : notnull
    {
        if (!Registrator.SupportsInterception)
            throw new NotSupportedException("Supported only when used with a registrator that supports interception");
        
        CustomDataInterceptors.TryAdd(typeof(TInterceptor), registrationOrder);
        return this;
    }
    
    /// <summary>
    /// Marks a decorator of a given type to be used for decorating generic data services.
    /// </summary>
    /// <param name="registrationOrder">Registration order.</param>
    /// <param name="decorator">Type of the decorator.</param>
    /// <returns>Current instance of the <see cref="DataExplorerConfiguration"/>.</returns>
    public virtual DataExplorerEfCoreConfiguration AddGenericDataServiceDecorator(int registrationOrder, Type decorator)
    {
        if (!Registrator.SupportsDecoration)
            throw new NotSupportedException("Supported only when used with a registrator that supports decoration");
        
        GenericDataDecorators.TryAdd(decorator ?? throw new ArgumentNullException(nameof(decorator)), registrationOrder);
        return this;
    }
    
    /// <summary>
    /// Marks a decorator of a given type to be used for decorating custom data services.
    /// </summary>
    /// <param name="registrationOrder">Registration order.</param>
    /// <param name="decorator">Type of the decorator.</param>
    /// <returns>Current instance of the <see cref="DataExplorerConfiguration"/>.</returns>
    public virtual DataExplorerEfCoreConfiguration AddCustomDataServiceDecorator(int registrationOrder, Type decorator)
    {
        if (!Registrator.SupportsDecoration)
            throw new NotSupportedException("Supported only when used with a registrator that supports decoration");
        
        CustomDataDecorators.TryAdd(decorator ?? throw new ArgumentNullException(nameof(decorator)), registrationOrder);
        return this;
    }
    
    /// <inheritdoc/>
    DataExplorerEfCoreConfiguration IOptions<DataExplorerEfCoreConfiguration>.Value => this;
}
