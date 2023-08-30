using System.Reflection;
using AttributeBasedRegistration;
using AttributeBasedRegistration.Attributes.Abstractions;
using AttributeBasedRegistration.Autofac;
using AttributeBasedRegistration.Extensions;
using Autofac;
using Autofac.Builder;
using Autofac.Extras.DynamicProxy;
using Castle.DynamicProxy;
using DataExplorer.Abstractions.DataServices;
using DataExplorer.Attributes;
using DataExplorer.EfCore.Abstractions;
using DataExplorer.EfCore.Abstractions.DataServices;
using DataExplorer.EfCore.DataServices;
using DataExplorer.EfCore.Gridify;
using DataExplorer.EfCore.Specifications.Evaluators;
using Gridify;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MikyM.Utilities.Extensions;
using GroupByEvaluator = DataExplorer.EfCore.Specifications.Evaluators.GroupByEvaluator;
using IBasicEvaluator = DataExplorer.EfCore.Specifications.Evaluators.IBasicEvaluator;
using IBasicInMemoryEvaluator = DataExplorer.EfCore.Specifications.Evaluators.IBasicInMemoryEvaluator;
using IBasicValidator = DataExplorer.EfCore.Specifications.Validators.IBasicValidator;
using IEvaluator = DataExplorer.EfCore.Specifications.Evaluators.IEvaluator;
using IEvaluatorMarker = DataExplorer.EfCore.Specifications.Evaluators.IEvaluatorMarker;
using IInMemoryEvaluator = DataExplorer.EfCore.Specifications.Evaluators.IInMemoryEvaluator;
using IInMemoryEvaluatorMarker = DataExplorer.EfCore.Specifications.Evaluators.IInMemoryEvaluatorMarker;
using IInMemorySpecificationEvaluator = DataExplorer.EfCore.Specifications.Evaluators.IInMemorySpecificationEvaluator;
using InMemorySpecificationEvaluator = DataExplorer.EfCore.Specifications.Evaluators.InMemorySpecificationEvaluator;
using IPreUpdateEvaluator = DataExplorer.EfCore.Specifications.Evaluators.IPreUpdateEvaluator;
using IProjectionEvaluator = DataExplorer.EfCore.Specifications.Evaluators.IProjectionEvaluator;
using ISpecialCaseEvaluator = DataExplorer.EfCore.Specifications.Evaluators.ISpecialCaseEvaluator;
using ISpecificationEvaluator = DataExplorer.EfCore.Specifications.Evaluators.ISpecificationEvaluator;
using ISpecificationValidator = DataExplorer.EfCore.Specifications.Validators.ISpecificationValidator;
using IValidator = DataExplorer.EfCore.Specifications.Validators.IValidator;
using IValidatorMarker = DataExplorer.EfCore.Specifications.Validators.IValidatorMarker;
using ProjectionEvaluator = DataExplorer.EfCore.Specifications.Evaluators.ProjectionEvaluator;
using ServiceLifetime = AttributeBasedRegistration.ServiceLifetime;
using SpecificationEvaluator = DataExplorer.EfCore.Specifications.SpecificationEvaluator;
using SpecificationValidator = DataExplorer.EfCore.Specifications.Validators.SpecificationValidator;

namespace DataExplorer.EfCore.Extensions;

/// <summary>
/// Extensions for <see cref="DataExplorerConfiguration"/>.
/// </summary>
[PublicAPI]
public static class DataExplorerConfigurationExtensions
{
    /// <summary>
    /// Adds Data Access Layer to the application.
    /// </summary>
    /// <remarks>
    /// Automatically registers all base <see cref="IValidator"/> types, <see cref="IInMemoryEvaluator"/> types, <see cref="IEvaluator"/> types, <see cref="IProjectionEvaluator"/>, <see cref="ISpecificationValidator"/>, <see cref="IInMemorySpecificationEvaluator"/>, <see cref="ISpecificationEvaluator"/>, <see cref="IUnitOfWork"/>, <see cref="ICrudDataService{TEntity,TId,TContext}"/>, <see cref="ICrudDataService{TEntity,TContext}"/>, <see cref="IReadOnlyDataService{TEntity,TId,TContext}"/>, <see cref="IReadOnlyDataService{TEntity,TContext}"/>, <see cref="IDataServiceBase{TContext}"/> with the DI container.
    /// </remarks>
    /// <param name="configuration">Current instance of <see cref="DataExplorerConfiguration"/></param>
    /// <param name="assembliesToScanForServices">Assemblies containing types to scan DataExplorer services such as data services, validators, evaluators etc.</param>
    /// <param name="assembliesToScanForEntities">Assemblies containing entity types.</param>
    /// <param name="options"><see cref="Action"/> that configures DAL.</param>
    public static DataExplorerConfiguration AddEfCore(this DataExplorerConfiguration configuration,
        IEnumerable<Type> assembliesToScanForServices,
        IEnumerable<Type> assembliesToScanForEntities,
        Action<DataExplorerEfCoreConfiguration>? options = null)
        => AddEfCore(configuration, 
            assembliesToScanForServices.Select(x => x.Assembly).Distinct(), 
            assembliesToScanForEntities.Select(x => x.Assembly).Distinct(),
            options);

        /// <summary>
    /// Adds Data Access Layer to the application.
    /// </summary>
    /// <remarks>
    /// Automatically registers all base <see cref="IValidator"/> types, <see cref="IInMemoryEvaluator"/> types, <see cref="IEvaluator"/> types, <see cref="IProjectionEvaluator"/>, <see cref="ISpecificationValidator"/>, <see cref="IInMemorySpecificationEvaluator"/>, <see cref="ISpecificationEvaluator"/>, <see cref="IUnitOfWork"/>, <see cref="ICrudDataService{TEntity,TId,TContext}"/>, <see cref="ICrudDataService{TEntity,TContext}"/>, <see cref="IReadOnlyDataService{TEntity,TId,TContext}"/>, <see cref="IReadOnlyDataService{TEntity,TContext}"/>, <see cref="IDataServiceBase{TContext}"/> with the DI container.
    /// </remarks>
    /// <param name="configuration">Current instance of <see cref="DataExplorerConfiguration"/></param>
    /// <param name="assembliesToScan">Assemblies to scan for DataExplorer services such as data services, validators, evaluators etc. and entities</param>
    /// <param name="options"><see cref="Action"/> that configures DAL.</param>
    public static DataExplorerConfiguration AddEfCore(this DataExplorerConfiguration configuration,
        IEnumerable<Assembly> assembliesToScan,
        Action<DataExplorerEfCoreConfiguration>? options = null)
    {
        var arr = assembliesToScan.ToArray();
        return AddEfCore(configuration, arr.Distinct(), arr.Distinct(), options);
    }
    
    /// <summary>
    /// Adds Data Access Layer to the application.
    /// </summary>
    /// <remarks>
    /// Automatically registers all base <see cref="IValidator"/> types, <see cref="IInMemoryEvaluator"/> types, <see cref="IEvaluator"/> types, <see cref="IProjectionEvaluator"/>, <see cref="ISpecificationValidator"/>, <see cref="IInMemorySpecificationEvaluator"/>, <see cref="ISpecificationEvaluator"/>, <see cref="IUnitOfWork"/>, <see cref="ICrudDataService{TEntity,TId,TContext}"/>, <see cref="ICrudDataService{TEntity,TContext}"/>, <see cref="IReadOnlyDataService{TEntity,TId,TContext}"/>, <see cref="IReadOnlyDataService{TEntity,TContext}"/>, <see cref="IDataServiceBase{TContext}"/> with the DI container.
    /// </remarks>
    /// <param name="configuration">Current instance of <see cref="DataExplorerConfiguration"/></param>
    /// <param name="assembliesToScan">Assemblies to scan for DataExplorer services such as data services, validators, evaluators etc. and entities</param>
    /// <param name="options"><see cref="Action"/> that configures DAL.</param>
    public static DataExplorerConfiguration AddEfCore(this DataExplorerConfiguration configuration,
        Action<DataExplorerEfCoreConfiguration> options,
        params Type[] assembliesToScan)
    {
        var arr = assembliesToScan.Select(x => x.Assembly).ToArray();
        return AddEfCore(configuration, arr.Distinct(), arr.Distinct(), options);
    }
    
    /// <summary>
    /// Adds Data Access Layer to the application.
    /// </summary>
    /// <remarks>
    /// Automatically registers all base <see cref="IValidator"/> types, <see cref="IInMemoryEvaluator"/> types, <see cref="IEvaluator"/> types, <see cref="IProjectionEvaluator"/>, <see cref="ISpecificationValidator"/>, <see cref="IInMemorySpecificationEvaluator"/>, <see cref="ISpecificationEvaluator"/>, <see cref="IUnitOfWork"/>, <see cref="ICrudDataService{TEntity,TId,TContext}"/>, <see cref="ICrudDataService{TEntity,TContext}"/>, <see cref="IReadOnlyDataService{TEntity,TId,TContext}"/>, <see cref="IReadOnlyDataService{TEntity,TContext}"/>, <see cref="IDataServiceBase{TContext}"/> with the DI container.
    /// </remarks>
    /// <param name="configuration">Current instance of <see cref="DataExplorerConfiguration"/></param>
    /// <param name="assembliesToScan">Assemblies to scan for DataExplorer services such as data services, validators, evaluators etc. and entities</param>
    /// <param name="options"><see cref="Action"/> that configures DAL.</param>
    public static DataExplorerConfiguration AddEfCore(this DataExplorerConfiguration configuration,
        Action<DataExplorerEfCoreConfiguration> options,
        params Assembly[] assembliesToScan)
    {
        var arr = assembliesToScan.ToArray();
        return AddEfCore(configuration, arr.Distinct(), arr.Distinct(), options);
    }
    
    /// <summary>
    /// Adds Data Access Layer to the application.
    /// </summary>
    /// <remarks>
    /// Automatically registers all base <see cref="IValidator"/> types, <see cref="IInMemoryEvaluator"/> types, <see cref="IEvaluator"/> types, <see cref="IProjectionEvaluator"/>, <see cref="ISpecificationValidator"/>, <see cref="IInMemorySpecificationEvaluator"/>, <see cref="ISpecificationEvaluator"/>, <see cref="IUnitOfWork"/>, <see cref="ICrudDataService{TEntity,TId,TContext}"/>, <see cref="ICrudDataService{TEntity,TContext}"/>, <see cref="IReadOnlyDataService{TEntity,TId,TContext}"/>, <see cref="IReadOnlyDataService{TEntity,TContext}"/>, <see cref="IDataServiceBase{TContext}"/> with the DI container.
    /// </remarks>
    /// <param name="configuration">Current instance of <see cref="DataExplorerConfiguration"/></param>
    /// <param name="assembliesToScan">Assemblies to scan for DataExplorer services such as data services, validators, evaluators etc. and entities</param>
    /// <param name="options"><see cref="Action"/> that configures DAL.</param>
    public static DataExplorerConfiguration AddEfCore(this DataExplorerConfiguration configuration,
        IEnumerable<Type> assembliesToScan,
        Action<DataExplorerEfCoreConfiguration>? options = null)
    {
        var arr = assembliesToScan.Select(x => x.Assembly).ToArray();
        return AddEfCore(configuration, arr.Distinct(), arr.Distinct(), options);
    }

    /// <summary>
    /// Adds Data Access Layer to the application.
    /// </summary>
    /// <remarks>
    /// Automatically registers all base <see cref="IValidator"/> types, <see cref="IInMemoryEvaluator"/> types, <see cref="IEvaluator"/> types, <see cref="IProjectionEvaluator"/>, <see cref="ISpecificationValidator"/>, <see cref="IInMemorySpecificationEvaluator"/>, <see cref="ISpecificationEvaluator"/>, <see cref="IUnitOfWork"/>, <see cref="ICrudDataService{TEntity,TId,TContext}"/>, <see cref="ICrudDataService{TEntity,TContext}"/>, <see cref="IReadOnlyDataService{TEntity,TId,TContext}"/>, <see cref="IReadOnlyDataService{TEntity,TContext}"/>, <see cref="IDataServiceBase{TContext}"/> with the DI container.
    /// </remarks>
    /// <param name="configuration">Current instance of <see cref="DataExplorerConfiguration"/></param>
    /// <param name="assembliesToScanForServices">Assemblies to scan for DataExplorer services such as data services, validators, evaluators etc.</param>
    /// <param name="assembliesToScanForEntities">Assemblies to scan for entities.</param>
    /// <param name="options"><see cref="Action"/> that configures DAL.</param>
    public static DataExplorerConfiguration AddEfCore(this DataExplorerConfiguration configuration, 
        IEnumerable<Assembly> assembliesToScanForServices, 
        IEnumerable<Assembly> assembliesToScanForEntities,
        Action<DataExplorerEfCoreConfiguration>? options = null)
    {
        var config = new DataExplorerEfCoreConfiguration(configuration);
        options?.Invoke(config);
        
        var builder = config.GetContainerBuilder();
        var serviceCollection = config.GetServiceCollection();
        
        GridifyGlobalConfiguration.EnableEntityFrameworkCompatibilityLayer();

        var toScan = assembliesToScanForServices.ToArray();
        
        var cache = new EfDataExplorerTypeCache(assembliesToScanForEntities);
        builder?.RegisterInstance(cache).As<IEfDataExplorerTypeCache>().SingleInstance();
        serviceCollection?.AddSingleton<IEfDataExplorerTypeCache>(cache);

        var mapperProvider = config.MapperProvider ?? new GridifyMapperProvider();
        builder?.RegisterInstance(mapperProvider).As<IGridifyMapperProvider>().SingleInstance();
        serviceCollection?.AddSingleton(mapperProvider);

        var ctorFinder = new AllConstructorsFinder();

        var iopt = Options.Create(config);
        builder?.RegisterInstance(iopt).As<IOptions<DataExplorerEfCoreConfiguration>>().SingleInstance();
        builder?.Register(x => x.Resolve<IOptions<DataExplorerEfCoreConfiguration>>().Value).AsSelf().SingleInstance();
        builder?.RegisterGeneric(typeof(UnitOfWork<>)).As(typeof(IUnitOfWork<>)).InstancePerLifetimeScope();
        
        serviceCollection?.AddSingleton(iopt);
        serviceCollection?.AddSingleton(x => x.GetRequiredService<IOptions<DataExplorerEfCoreConfiguration>>().Value);
        serviceCollection?.AddScoped(typeof(IUnitOfWork<>), typeof(UnitOfWork<>));

        // handle non special
        if (builder is not null)
            foreach (var assembly in toScan)
            {
                builder.RegisterAssemblyTypes(assembly)
                    .Where(x => x.GetInterface(nameof(IValidatorMarker)) is not null || 
                                x.GetInterface(nameof(IInMemoryEvaluatorMarker)) is not null ||
                                x.GetInterface(nameof(IEvaluatorMarker)) is not null 
                                    && x.GetInterface(nameof(ISpecialCaseEvaluator)) is null)
                    .AsImplementedInterfaces()
                    .FindConstructorsWith(new AllConstructorsFinder())
                    .SingleInstance();
            }

        if (serviceCollection is not null)
            foreach (var assembly in toScan)
            {
                foreach (var type in assembly.GetTypes().Where(x => x.GetInterface(nameof(IValidatorMarker)) is not null || 
                                                                    x.GetInterface(nameof(IInMemoryEvaluatorMarker)) is not null ||
                                                                    x.GetInterface(nameof(IEvaluatorMarker)) is not null 
                                                                    && x.GetInterface(nameof(ISpecialCaseEvaluator)) is null))
                {
                    var instance = Activator.CreateInstance(
                        type,
                        BindingFlags.Instance
                        | BindingFlags.Public
                        | BindingFlags.NonPublic,
                        null,
                        Array.Empty<object>(),
                        null
                    )!;
                    
                    var interfaces = type.GetInterfaces();
                    foreach (var inter in interfaces)
                    {
                        
                        serviceCollection.AddSingleton(inter, instance);
                    }
                }
            }

        // handle non special
        builder?.RegisterAssemblyTypes(typeof(GroupByEvaluator).Assembly)
            .Where(x => x.GetInterface(nameof(IValidatorMarker)) is not null ||
                        x.GetInterface(nameof(IInMemoryEvaluatorMarker)) is not null ||
                        x.GetInterface(nameof(IEvaluatorMarker)) is not null
                        && x.GetInterface(nameof(ISpecialCaseEvaluator)) is null)
            .AsImplementedInterfaces()
            .FindConstructorsWith(new AllConstructorsFinder())
            .SingleInstance();
        
        if (serviceCollection is not null)
        {
            foreach (var type in typeof(GroupByEvaluator).Assembly.GetTypes().Where(x => x.GetInterface(nameof(IValidatorMarker)) is not null || 
                                                                x.GetInterface(nameof(IInMemoryEvaluatorMarker)) is not null ||
                                                                x.GetInterface(nameof(IEvaluatorMarker)) is not null 
                                                                && x.GetInterface(nameof(ISpecialCaseEvaluator)) is null))
            {
                var instance = Activator.CreateInstance(
                    type,
                    BindingFlags.Instance
                    | BindingFlags.Public
                    | BindingFlags.NonPublic,
                    null,
                    Array.Empty<object>(),
                    null
                )!;
                    
                var interfaces = type.GetInterfaces();
                foreach (var inter in interfaces)
                {
                        
                    serviceCollection.AddSingleton(inter, instance);
                }

            }
        }

        builder?.RegisterType<IncludeEvaluator>()
            .As<IEvaluator>()
            .UsingConstructor(typeof(bool))
            .FindConstructorsWith(ctorFinder)
            .WithParameter(new TypedParameter(typeof(bool), config.EnableIncludeCache))
            .SingleInstance();

        serviceCollection?.AddSingleton<IEvaluator>(new IncludeEvaluator(config.EnableIncludeCache));

        builder?.RegisterType<ProjectionEvaluator>()
            .As<IProjectionEvaluator>()
            .FindConstructorsWith(ctorFinder)
            .SingleInstance();
        
        serviceCollection?.AddSingleton<IProjectionEvaluator>(new ProjectionEvaluator());
        
        builder?.RegisterType<UpdateEvaluator>()
            .As<IUpdateEvaluator>()
            .FindConstructorsWith(ctorFinder)
            .SingleInstance();

        serviceCollection?.AddSingleton<IUpdateEvaluator>(new UpdateEvaluator());

        builder?.RegisterType<SpecificationEvaluator>()
            .As<ISpecificationEvaluator>()
            .UsingConstructor(typeof(IEnumerable<IEvaluator>), typeof(IEnumerable<IBasicEvaluator>),
                typeof(IEnumerable<IPreUpdateEvaluator>), typeof(IProjectionEvaluator), typeof(IUpdateEvaluator))
            .FindConstructorsWith(ctorFinder)
            .SingleInstance();

        serviceCollection?.AddSingleton<ISpecificationEvaluator>(x =>
            new SpecificationEvaluator(x.GetRequiredService<IEnumerable<IEvaluator>>(),
                x.GetRequiredService<IEnumerable<IBasicEvaluator>>(),
                x.GetRequiredService<IEnumerable<IPreUpdateEvaluator>>(), x.GetRequiredService<IProjectionEvaluator>(),
                x.GetRequiredService<IUpdateEvaluator>()));

        builder?.RegisterType<SpecificationValidator>()
            .As<ISpecificationValidator>()
            .UsingConstructor(typeof(IEnumerable<IValidator>), typeof(IEnumerable<IBasicValidator>))
            .FindConstructorsWith(ctorFinder)
            .SingleInstance();

        serviceCollection?.AddSingleton<ISpecificationValidator>(x =>
            new SpecificationValidator(x.GetRequiredService<IEnumerable<IValidator>>(),
                x.GetRequiredService<IEnumerable<IBasicValidator>>()));

        builder?.RegisterType<InMemorySpecificationEvaluator>()
            .As<IInMemorySpecificationEvaluator>()
            .UsingConstructor(typeof(IEnumerable<IInMemoryEvaluator>), typeof(IEnumerable<IBasicInMemoryEvaluator>))
            .FindConstructorsWith(ctorFinder)
            .SingleInstance();

        serviceCollection?.AddSingleton<IInMemorySpecificationEvaluator>(x =>
            new InMemorySpecificationEvaluator(x.GetRequiredService<IEnumerable<IInMemoryEvaluator>>(),
                x.GetRequiredService<IEnumerable<IBasicInMemoryEvaluator>>()));
        
        
        IRegistrationBuilder<object, ReflectionActivatorData, DynamicRegistrationStyle>? registReadOnlyBuilder;
        IRegistrationBuilder<object, ReflectionActivatorData, DynamicRegistrationStyle>? registCrudBuilder;
        IRegistrationBuilder<object, ReflectionActivatorData, DynamicRegistrationStyle>? registReadOnlyGenericIdBuilder;
        IRegistrationBuilder<object, ReflectionActivatorData, DynamicRegistrationStyle>? registCrudGenericIdBuilder;

        switch (config.BaseGenericDataServiceLifetime)
        {
            case ServiceLifetime.SingleInstance:
                registReadOnlyBuilder = builder?.RegisterGeneric(typeof(ReadOnlyDataService<,>))
                    .As(typeof(IReadOnlyDataService<,>))
                    .SingleInstance();
                registCrudBuilder = builder?.RegisterGeneric(typeof(CrudDataService<,>))
                    .As(typeof(ICrudDataService<,>))
                    .SingleInstance();
                registReadOnlyGenericIdBuilder = builder?.RegisterGeneric(typeof(ReadOnlyDataService<,,>))
                    .As(typeof(IReadOnlyDataService<,,>))
                    .SingleInstance();
                registCrudGenericIdBuilder = builder?.RegisterGeneric(typeof(CrudDataService<,,>))
                    .As(typeof(ICrudDataService<,,>))
                    .SingleInstance();

                serviceCollection?.AddSingleton(typeof(IReadOnlyDataService<,>), typeof(ReadOnlyDataService<,>));
                serviceCollection?.AddSingleton(typeof(IReadOnlyDataService<,,>), typeof(ReadOnlyDataService<,,>));
                serviceCollection?.AddSingleton(typeof(ICrudDataService<,>), typeof(CrudDataService<,>));
                serviceCollection?.AddSingleton(typeof(ICrudDataService<,,>), typeof(CrudDataService<,,>));
                break;
            case ServiceLifetime.InstancePerRequest:
                registReadOnlyBuilder = builder?.RegisterGeneric(typeof(ReadOnlyDataService<,>))
                    .As(typeof(IReadOnlyDataService<,>))
                    .InstancePerRequest();
                registCrudBuilder = builder?.RegisterGeneric(typeof(CrudDataService<,>))
                    .As(typeof(ICrudDataService<,>))
                    .InstancePerRequest();
                registReadOnlyGenericIdBuilder = builder?.RegisterGeneric(typeof(ReadOnlyDataService<,,>))
                    .As(typeof(IReadOnlyDataService<,,>))
                    .InstancePerRequest();
                registCrudGenericIdBuilder = builder?.RegisterGeneric(typeof(CrudDataService<,,>))
                    .As(typeof(ICrudDataService<,,>))
                    .InstancePerRequest();
                
                serviceCollection?.AddScoped(typeof(IReadOnlyDataService<,>), typeof(ReadOnlyDataService<,>));
                serviceCollection?.AddScoped(typeof(IReadOnlyDataService<,,>), typeof(ReadOnlyDataService<,,>));
                serviceCollection?.AddScoped(typeof(ICrudDataService<,>), typeof(CrudDataService<,>));
                serviceCollection?.AddScoped(typeof(ICrudDataService<,,>), typeof(CrudDataService<,,>));
                break;
            case ServiceLifetime.InstancePerLifetimeScope:
                registReadOnlyBuilder = builder?.RegisterGeneric(typeof(ReadOnlyDataService<,>))
                    .As(typeof(IReadOnlyDataService<,>))
                    .InstancePerLifetimeScope();
                registCrudBuilder = builder?.RegisterGeneric(typeof(CrudDataService<,>))
                    .As(typeof(ICrudDataService<,>))
                    .InstancePerLifetimeScope();
                registReadOnlyGenericIdBuilder = builder?.RegisterGeneric(typeof(ReadOnlyDataService<,,>))
                    .As(typeof(IReadOnlyDataService<,,>))
                    .InstancePerLifetimeScope();
                registCrudGenericIdBuilder = builder?.RegisterGeneric(typeof(CrudDataService<,,>))
                    .As(typeof(ICrudDataService<,,>))
                    .InstancePerLifetimeScope();
                
                serviceCollection?.AddScoped(typeof(IReadOnlyDataService<,>), typeof(ReadOnlyDataService<,>));
                serviceCollection?.AddScoped(typeof(IReadOnlyDataService<,,>), typeof(ReadOnlyDataService<,,>));
                serviceCollection?.AddScoped(typeof(ICrudDataService<,>), typeof(CrudDataService<,>));
                serviceCollection?.AddScoped(typeof(ICrudDataService<,,>), typeof(CrudDataService<,,>));
                break;
            case ServiceLifetime.InstancePerMatchingLifetimeScope:
                registReadOnlyBuilder = builder?.RegisterGeneric(typeof(ReadOnlyDataService<,>))
                    .As(typeof(IReadOnlyDataService<,>))
                    .InstancePerMatchingLifetimeScope();
                registCrudBuilder = builder?.RegisterGeneric(typeof(CrudDataService<,>))
                    .As(typeof(ICrudDataService<,>))
                    .InstancePerMatchingLifetimeScope();
                registReadOnlyGenericIdBuilder = builder?.RegisterGeneric(typeof(ReadOnlyDataService<,,>))
                    .As(typeof(IReadOnlyDataService<,,>))
                    .InstancePerMatchingLifetimeScope();
                registCrudGenericIdBuilder = builder?.RegisterGeneric(typeof(CrudDataService<,,>))
                    .As(typeof(ICrudDataService<,,>))
                    .InstancePerMatchingLifetimeScope();
                break;
            case ServiceLifetime.InstancePerDependency:
                registReadOnlyBuilder = builder?.RegisterGeneric(typeof(ReadOnlyDataService<,>))
                    .As(typeof(IReadOnlyDataService<,>))
                    .InstancePerDependency();
                registCrudBuilder = builder?.RegisterGeneric(typeof(CrudDataService<,>))
                    .As(typeof(ICrudDataService<,>))
                    .InstancePerDependency();
                registReadOnlyGenericIdBuilder = builder?.RegisterGeneric(typeof(ReadOnlyDataService<,,>))
                    .As(typeof(IReadOnlyDataService<,,>))
                    .InstancePerDependency();
                registCrudGenericIdBuilder = builder?.RegisterGeneric(typeof(CrudDataService<,,>))
                    .As(typeof(ICrudDataService<,,>))
                    .InstancePerDependency();
                
                serviceCollection?.AddTransient(typeof(IReadOnlyDataService<,>), typeof(ReadOnlyDataService<,>));
                serviceCollection?.AddTransient(typeof(IReadOnlyDataService<,,>), typeof(ReadOnlyDataService<,,>));
                serviceCollection?.AddTransient(typeof(ICrudDataService<,>), typeof(CrudDataService<,>));
                serviceCollection?.AddTransient(typeof(ICrudDataService<,,>), typeof(CrudDataService<,,>));
                break;
            case ServiceLifetime.InstancePerOwned:
                throw new NotSupportedException();
            default:
                throw new ArgumentOutOfRangeException(nameof(config.BaseGenericDataServiceLifetime),
                    config.BaseGenericDataServiceLifetime, null);
        }

        // base data interceptors
        var crudEnabled = false;
        var readEnabled = false;
        if (builder is not null)
            foreach (var (interceptorType, registrationData) in config.DataInterceptors.OrderByDescending(x => x.Value.Order))
            {
                switch (registrationData.Strategy)
                {
                    case DataRegistrationStrategy.CrudAndReadOnly:
                        registCrudBuilder = interceptorType.IsAsyncInterceptor()
                            ? registCrudBuilder?.InterceptedBy(
                                typeof(AsyncInterceptorAdapter<>).MakeGenericType(interceptorType))
                            : registCrudBuilder?.InterceptedBy(interceptorType);
                        registReadOnlyBuilder = interceptorType.IsAsyncInterceptor()
                            ? registReadOnlyBuilder?.InterceptedBy(
                                typeof(AsyncInterceptorAdapter<>).MakeGenericType(interceptorType))
                            : registReadOnlyBuilder?.InterceptedBy(interceptorType);
                        registCrudGenericIdBuilder = interceptorType.IsAsyncInterceptor()
                            ? registCrudGenericIdBuilder?.InterceptedBy(
                                typeof(AsyncInterceptorAdapter<>).MakeGenericType(interceptorType))
                            : registCrudGenericIdBuilder?.InterceptedBy(interceptorType);
                        registReadOnlyGenericIdBuilder = interceptorType.IsAsyncInterceptor()
                            ? registReadOnlyGenericIdBuilder?.InterceptedBy(
                                typeof(AsyncInterceptorAdapter<>).MakeGenericType(interceptorType))
                            : registReadOnlyGenericIdBuilder?.InterceptedBy(interceptorType);

                        if (!crudEnabled)
                        {
                            registCrudBuilder = registCrudBuilder?.EnableInterfaceInterceptors();
                            registCrudGenericIdBuilder = registCrudGenericIdBuilder?.EnableInterfaceInterceptors();
                            crudEnabled = true;
                        }

                        if (!readEnabled)
                        {
                            registReadOnlyBuilder = registReadOnlyBuilder?.EnableInterfaceInterceptors();
                            registReadOnlyGenericIdBuilder = registReadOnlyGenericIdBuilder?.EnableInterfaceInterceptors();
                            readEnabled = true;
                        }

                        break;
                    case DataRegistrationStrategy.Crud:
                        registCrudBuilder = interceptorType.IsAsyncInterceptor()
                            ? registCrudBuilder?.InterceptedBy(
                                typeof(AsyncInterceptorAdapter<>).MakeGenericType(interceptorType))
                            : registCrudBuilder?.InterceptedBy(interceptorType);
                        registCrudGenericIdBuilder = interceptorType.IsAsyncInterceptor()
                            ? registCrudGenericIdBuilder?.InterceptedBy(
                                typeof(AsyncInterceptorAdapter<>).MakeGenericType(interceptorType))
                            : registCrudGenericIdBuilder?.InterceptedBy(interceptorType);
                        if (!crudEnabled)
                        {
                            registCrudBuilder = registCrudBuilder?.EnableInterfaceInterceptors();
                            registCrudGenericIdBuilder = registCrudGenericIdBuilder?.EnableInterfaceInterceptors();
                            crudEnabled = true;
                        }

                        break;
                    case DataRegistrationStrategy.ReadOnly:
                        registReadOnlyBuilder = interceptorType.IsAsyncInterceptor()
                            ? registReadOnlyBuilder?.InterceptedBy(
                                typeof(AsyncInterceptorAdapter<>).MakeGenericType(interceptorType))
                            : registReadOnlyBuilder?.InterceptedBy(interceptorType);
                        registReadOnlyGenericIdBuilder = interceptorType.IsAsyncInterceptor()
                            ? registReadOnlyGenericIdBuilder?.InterceptedBy(
                                typeof(AsyncInterceptorAdapter<>).MakeGenericType(interceptorType))
                            : registReadOnlyGenericIdBuilder?.InterceptedBy(interceptorType);
                        if (!readEnabled)
                        {
                            registReadOnlyBuilder = registReadOnlyBuilder?.EnableInterfaceInterceptors();
                            registReadOnlyGenericIdBuilder = registReadOnlyGenericIdBuilder?.EnableInterfaceInterceptors();
                            readEnabled = true;
                        }

                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(registrationData.Strategy));
                }
            }
        
        if (builder is not null)
            foreach (var (decoratorType, _) in config.DataDecorators.OrderBy(x => x.Value))
            {
                if (decoratorType.IsAssignableToWithGenerics(typeof(ICrudDataService<,>)))
                    builder.RegisterGenericDecorator(decoratorType, typeof(ICrudDataService<,>));
                if (decoratorType.IsAssignableToWithGenerics(typeof(IReadOnlyDataService<,>)))
                    builder.RegisterGenericDecorator(decoratorType, typeof(IReadOnlyDataService<,>));
                if (decoratorType.IsAssignableToWithGenerics(typeof(ICrudDataService<,,>)))
                    builder.RegisterGenericDecorator(decoratorType, typeof(ICrudDataService<,,>));
                if (decoratorType.IsAssignableToWithGenerics(typeof(IReadOnlyDataService<,,>)))
                    builder.RegisterGenericDecorator(decoratorType, typeof(IReadOnlyDataService<,,>));
            }

        var excluded = new[] { typeof(IDataServiceBase<>), typeof(EfCoreDataServiceBase<>), typeof(CrudDataService<,>), typeof(ReadOnlyDataService<,>), typeof(CrudDataService<,,>), typeof(ReadOnlyDataService<,,>) };

        foreach (var assembly in toScan)
        {
            var dataSubSet = assembly.GetTypes()
                .Where(x => x.GetInterfaces()
                                .Any(y => y.IsGenericType &&
                                          y.GetGenericTypeDefinition() == typeof(IEfCoreDataServiceBase<>)) &&
                            x.IsClass && !x.IsAbstract)
                .ToList();

            dataSubSet.RemoveAll(x => excluded.Any(y => y == x));

            // handle data services
            foreach (var dataType in dataSubSet)
            {
                if (dataType.ShouldSkipRegistration<ISkipDataServiceRegistrationAttribute>())
                    continue;
                
                var scopeOverrideAttrs = dataType.GetRegistrationAttributesOfType<ILifetimeAttribute>().ToArray();
                if (scopeOverrideAttrs.Length > 1)
                    throw new InvalidOperationException($"Only a single lifetime attribute is allowed on a type, type: {dataType.Name}");
                var scopeOverrideAttr = scopeOverrideAttrs.FirstOrDefault();
                
                var intrAttrs = dataType.GetRegistrationAttributesOfType<IInterceptedByAttribute>().ToArray();
                
                var asAttrs = dataType.GetRegistrationAttributesOfType<IRegisterAsAttribute>().ToArray();
                
                var intrEnableAttrs = dataType.GetRegistrationAttributesOfType<IEnableInterceptionAttribute>().ToArray();
                if (intrEnableAttrs.Length > 1)
                    throw new InvalidOperationException($"Only a single enable interception attribute is allowed on a type, type: {dataType.Name}");
                var intrEnableAttr = intrEnableAttrs.FirstOrDefault();

                var scope = scopeOverrideAttr?.ServiceLifetime ?? config.DataServiceLifetime;

                var registerAsTypes = asAttrs.Where(x => x.ServiceTypes is not null)
                    .SelectMany(x => x.ServiceTypes ?? Type.EmptyTypes)
                    .Distinct()
                    .ToList();

                var shouldAsSelf = asAttrs.Any(x => x.RegistrationStrategy is RegistrationStrategy.AsSelf) &&
                                   asAttrs.All(x => (x.ServiceTypes ?? Type.EmptyTypes).All(y => y != dataType));
                var shouldAsInterfaces =
                    asAttrs.Any(x => x.RegistrationStrategy is RegistrationStrategy.AsImplementedInterfaces);
                var shouldAsDirectAncestors =
                    asAttrs.Any(x => x.RegistrationStrategy is RegistrationStrategy.AsDirectAncestorInterfaces);
                var shouldUsingNamingConvention =
                    asAttrs.Any(x => x.RegistrationStrategy is RegistrationStrategy.AsConventionNamedInterface);

                var convention = dataType.GetInterfaceByNamingConvention();
                if (convention is not null)
                    registerAsTypes.Add(convention);

                if (!shouldAsInterfaces && !registerAsTypes.Any() && !shouldUsingNamingConvention && !shouldAsSelf)
                    shouldAsDirectAncestors = true;

                IRegistrationBuilder<object, ReflectionActivatorData, DynamicRegistrationStyle>?
                    registrationGenericBuilder = null;
                IRegistrationBuilder<object, ReflectionActivatorData, SingleRegistrationStyle>? registrationBuilder =
                    null;

                if (dataType.IsGenericType && dataType.IsGenericTypeDefinition)
                {
                    if (intrEnableAttr is not null)
                    {
                        registrationGenericBuilder = shouldAsInterfaces
                            ? builder?.RegisterGeneric(dataType).AsImplementedInterfaces().EnableInterfaceInterceptors()
                            : builder?.RegisterGeneric(dataType).EnableInterfaceInterceptors();
                    }
                    else
                    {
                        registrationGenericBuilder = shouldAsInterfaces
                            ? builder?.RegisterGeneric(dataType).AsImplementedInterfaces()
                            : builder?.RegisterGeneric(dataType);
                    }
                }
                else
                {
                    if (intrEnableAttr is not null)
                    {
                        registrationBuilder = intrEnableAttr.InterceptionStrategy switch
                        {
                            InterceptionStrategy.Interface => shouldAsInterfaces
                                ? builder?.RegisterType(dataType).AsImplementedInterfaces().EnableInterfaceInterceptors()
                                : builder?.RegisterType(dataType).EnableInterfaceInterceptors(),
                            InterceptionStrategy.Class => shouldAsInterfaces
                                ? builder?.RegisterType(dataType).AsImplementedInterfaces().EnableClassInterceptors()
                                : builder?.RegisterType(dataType).EnableClassInterceptors(),
                            _ => throw new ArgumentOutOfRangeException(nameof(intrEnableAttr.InterceptionStrategy))
                        };
                    }
                    else
                    {
                        registrationBuilder = shouldAsInterfaces
                            ? builder?.RegisterType(dataType).AsImplementedInterfaces()
                            : builder?.RegisterType(dataType);
                    }
                }

                if (shouldAsSelf)
                {
                    registrationBuilder = registrationBuilder?.As(dataType);
                    registrationGenericBuilder = registrationGenericBuilder?.AsSelf();
                }
                
                if (shouldAsDirectAncestors)
                {
                    registrationBuilder = registrationBuilder?.AsDirectAncestorInterfaces();
                    registrationGenericBuilder = registrationGenericBuilder?.AsDirectAncestorInterfaces();
                }
                
                if (shouldUsingNamingConvention)
                {
                    registrationBuilder = registrationBuilder?.AsConventionNamedInterface();
                    registrationGenericBuilder = registrationGenericBuilder?.AsConventionNamedInterface();
                }

                foreach (var asType in registerAsTypes)
                {
                    if (asType is null) throw new InvalidOperationException("Type was null during registration");

                    registrationBuilder = registrationBuilder?.As(asType);
                    registrationGenericBuilder = registrationGenericBuilder?.As(asType);
                }

                var interfaces = dataType.GetInterfaces().Where(x => x != typeof(IDisposable) && x != typeof(IAsyncDisposable)).ToList();
                
                switch (scope)
                {
                    case ServiceLifetime.SingleInstance:
                        registrationBuilder = registrationBuilder?.SingleInstance();
                        registrationGenericBuilder = registrationGenericBuilder?.SingleInstance();

                        if (serviceCollection is not null)
                        {
                            if (shouldAsInterfaces)
                                interfaces.ForEach(x => serviceCollection.AddSingleton(x, dataType));
                            if (shouldAsSelf)
                                serviceCollection.AddSingleton(dataType);
                            if (registerAsTypes.Any())
                                registerAsTypes.ForEach(x => serviceCollection.AddSingleton(x, dataType));
                            if (shouldAsDirectAncestors)
                                dataType.GetDirectInterfaceAncestors()
                                    .Where(x => x != typeof(IDisposable) && x != typeof(IAsyncDisposable)).ToList()
                                    .ForEach(x => serviceCollection.AddSingleton(x, dataType));
                            if (shouldUsingNamingConvention)
                                serviceCollection.AddSingleton(dataType.GetInterfaceByNamingConvention() ?? throw new ArgumentException(
                                    "Couldn't find an implemented interface that follows the naming convention"), dataType);
                        }
                        break;
                    case ServiceLifetime.InstancePerRequest:
                        registrationBuilder = registrationBuilder?.InstancePerRequest();
                        registrationGenericBuilder = registrationGenericBuilder?.InstancePerRequest();
                        
                        if (serviceCollection is not null)
                        {
                            if (shouldAsInterfaces)
                                interfaces.ForEach(x => serviceCollection.AddScoped(x, dataType));
                            if (shouldAsSelf)
                                serviceCollection.AddScoped(dataType);
                            if (registerAsTypes.Any())
                                registerAsTypes.ForEach(x => serviceCollection.AddScoped(x, dataType));
                            if (shouldAsDirectAncestors)
                                dataType.GetDirectInterfaceAncestors()
                                    .Where(x => x != typeof(IDisposable) && x != typeof(IAsyncDisposable)).ToList()
                                    .ForEach(x => serviceCollection.AddScoped(x, dataType));
                            if (shouldUsingNamingConvention)
                                serviceCollection.AddScoped(dataType.GetInterfaceByNamingConvention() ?? throw new ArgumentException(
                                    "Couldn't find an implemented interface that follows the naming convention"), dataType);
                        }
                        break;
                    case ServiceLifetime.InstancePerLifetimeScope:
                        registrationBuilder = registrationBuilder?.InstancePerLifetimeScope();
                        registrationGenericBuilder = registrationGenericBuilder?.InstancePerLifetimeScope();
                        
                        if (serviceCollection is not null)
                        {
                            if (shouldAsInterfaces)
                                interfaces.ForEach(x => serviceCollection.AddScoped(x, dataType));
                            if (shouldAsSelf)
                                serviceCollection.AddScoped(dataType);
                            if (registerAsTypes.Any())
                                registerAsTypes.ForEach(x => serviceCollection.AddScoped(x, dataType));
                            if (shouldAsDirectAncestors)
                                dataType.GetDirectInterfaceAncestors()
                                    .Where(x => x != typeof(IDisposable) && x != typeof(IAsyncDisposable)).ToList()
                                    .ForEach(x => serviceCollection.AddScoped(x, dataType));
                            if (shouldUsingNamingConvention)
                                serviceCollection.AddScoped(dataType.GetInterfaceByNamingConvention() ?? throw new ArgumentException(
                                    "Couldn't find an implemented interface that follows the naming convention"), dataType);
                        }
                        break;
                    case ServiceLifetime.InstancePerDependency:
                        registrationBuilder = registrationBuilder?.InstancePerDependency();
                        registrationGenericBuilder = registrationGenericBuilder?.InstancePerDependency();
                        
                        if (serviceCollection is not null)
                        {
                            if (shouldAsInterfaces)
                                interfaces.ForEach(x => serviceCollection.AddTransient(x, dataType));
                            if (shouldAsSelf)
                                serviceCollection.AddTransient(dataType);
                            if (registerAsTypes.Any())
                                registerAsTypes.ForEach(x => serviceCollection.AddTransient(x, dataType));
                            if (shouldAsDirectAncestors)
                                dataType.GetDirectInterfaceAncestors()
                                    .Where(x => x != typeof(IDisposable) && x != typeof(IAsyncDisposable)).ToList()
                                    .ForEach(x => serviceCollection.AddTransient(x, dataType));
                            if (shouldUsingNamingConvention)
                                serviceCollection.AddTransient(dataType.GetInterfaceByNamingConvention() ?? throw new ArgumentException(
                                    "Couldn't find an implemented interface that follows the naming convention"), dataType);
                        }
                        break;
                    case ServiceLifetime.InstancePerMatchingLifetimeScope:
                        registrationBuilder =
                            registrationBuilder?.InstancePerMatchingLifetimeScope(scopeOverrideAttr?.Tags?.ToArray() ??
                                Array.Empty<object>());
                        registrationGenericBuilder =
                            registrationGenericBuilder?.InstancePerMatchingLifetimeScope(
                                scopeOverrideAttr?.Tags?.ToArray() ?? Array.Empty<object>());
                        break;
                    case ServiceLifetime.InstancePerOwned:
                        if (scopeOverrideAttr?.Owned is null)
                            throw new InvalidOperationException("Owned type was null");

                        registrationBuilder = registrationBuilder?.InstancePerOwned(scopeOverrideAttr.Owned);
                        registrationGenericBuilder =
                            registrationGenericBuilder?.InstancePerOwned(scopeOverrideAttr.Owned);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(scope));
                }
                
                if (builder is null)
                    continue;
                
                if (intrAttrs.GroupBy(x => x.RegistrationOrder).FirstOrDefault(x => x.Count() > 1) is not null)
                    throw new InvalidOperationException($"Duplicated interceptor registration order on type {dataType.Name}");

                if (intrAttrs.GroupBy(x => x.Interceptor)
                        .FirstOrDefault(x => x.Count() > 1) is not null)
                    throw new InvalidOperationException($"Duplicated interceptor type on type {dataType.Name}");

                foreach (var interceptor in intrAttrs.OrderByDescending(x => x.RegistrationOrder).Select(x => x.Interceptor).Distinct())
                {
                    registrationBuilder = interceptor.IsAsyncInterceptor()
                        ? registrationBuilder?.InterceptedBy(
                            typeof(AsyncInterceptorAdapter<>).MakeGenericType(interceptor))
                        : registrationBuilder?.InterceptedBy(interceptor);
                    registrationGenericBuilder = interceptor.IsAsyncInterceptor()
                        ? registrationGenericBuilder?.InterceptedBy(
                            typeof(AsyncInterceptorAdapter<>).MakeGenericType(interceptor))
                        : registrationGenericBuilder?.InterceptedBy(interceptor);
                }

                var decoratorAttributes = dataType.GetRegistrationAttributesOfType<IDecoratedByAttribute>().ToArray();
                if (!decoratorAttributes.Any())
                    continue;

                HashSet<Type> serviceTypes = new();
                if (shouldAsSelf)
                    serviceTypes.Add(dataType);
                if (registerAsTypes.Any())
                    registerAsTypes.ForEach(x => serviceTypes.Add(x));
                if (shouldAsInterfaces)
                    dataType.GetInterfaces().Where(x => x != typeof(IDisposable) && x != typeof(IAsyncDisposable)).ToList()
                        .ForEach(x => serviceTypes.Add(x));
                if (shouldAsDirectAncestors)
                    dataType.GetDirectInterfaceAncestors()
                        .Where(x => x != typeof(IDisposable) && x != typeof(IAsyncDisposable)).ToList()
                        .ForEach(x => serviceTypes.Add(x));
                if (shouldUsingNamingConvention)
                    serviceTypes.Add(dataType.GetInterfaceByNamingConvention() ??
                                     throw new InvalidOperationException("Couldn't find an interface by naming convention"));

                if (decoratorAttributes.GroupBy(x => x.RegistrationOrder).FirstOrDefault(x => x.Count() > 1) is not null)
                    throw new InvalidOperationException($"Duplicated decorator registration order on type {dataType.Name}");

                if (decoratorAttributes.GroupBy(x => x.Decorator)
                        .FirstOrDefault(x => x.Count() > 1) is not null)
                    throw new InvalidOperationException($"Duplicated decorator type on type {dataType.Name}");
                
                foreach (var attribute in decoratorAttributes.OrderBy(x => x.RegistrationOrder))
                {
                    if (attribute.Decorator.ShouldSkipRegistration<ISkipDecoratorRegistrationAttribute>())
                        continue;
            
                    if (attribute.Decorator.IsGenericType && attribute.Decorator.IsGenericTypeDefinition)
                    {
                        foreach (var serviceType in serviceTypes)
                            builder?.RegisterGenericDecorator(attribute.Decorator, serviceType);
                    }
                    else
                    {
                        foreach (var serviceType in serviceTypes)
                            builder?.RegisterDecorator(attribute.Decorator, serviceType);
                    }
                }
            }
        }

        return configuration;
    }
    
    /// <summary>
    /// Whether given interceptor is an async interceptor.
    /// </summary>
    private static bool IsAsyncInterceptor(this Type interceptorCandidate) => interceptorCandidate.GetInterfaces().Any(x => x == typeof(IAsyncInterceptor));
}
