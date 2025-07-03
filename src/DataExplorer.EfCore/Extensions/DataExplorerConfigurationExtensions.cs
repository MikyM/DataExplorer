using System.Reflection;
using AttributeBasedRegistration;
using AttributeBasedRegistration.Attributes.Abstractions;
using AttributeBasedRegistration.Extensions;
using DataExplorer.Abstractions;
using DataExplorer.Abstractions.DataServices;
using DataExplorer.Abstractions.Specifications.Evaluators;
using DataExplorer.Attributes;
using DataExplorer.EfCore.Abstractions.DataServices;
using DataExplorer.EfCore.DataServices;
using DataExplorer.EfCore.Specifications.Evaluators;
using DataExplorer.Extensions;
using Microsoft.Extensions.DependencyInjection;
using GroupByEvaluator = DataExplorer.EfCore.Specifications.Evaluators.GroupByEvaluator;
using IBasicEvaluator = DataExplorer.Abstractions.Specifications.Evaluators.IBasicEvaluator;
using IBasicInMemoryEvaluator = DataExplorer.Abstractions.Specifications.Evaluators.IBasicInMemoryEvaluator;
using IBasicValidator = DataExplorer.Abstractions.Specifications.Validators.IBasicValidator;
using IEvaluator = DataExplorer.Abstractions.Specifications.Evaluators.IEvaluator;
using IInMemoryEvaluator = DataExplorer.Abstractions.Specifications.Evaluators.IInMemoryEvaluator;
using IInMemoryEvaluatorMarker = DataExplorer.Abstractions.Specifications.Evaluators.IInMemoryEvaluatorMarker;
using IInMemorySpecificationEvaluator = DataExplorer.Abstractions.Specifications.Evaluators.IInMemorySpecificationEvaluator;
using InMemorySpecificationEvaluator = DataExplorer.EfCore.Specifications.Evaluators.InMemorySpecificationEvaluator;
using IPreUpdateEvaluator = DataExplorer.Abstractions.Specifications.Evaluators.IPreUpdateEvaluator;
using IProjectionEvaluator = DataExplorer.Abstractions.Specifications.Evaluators.IProjectionEvaluator;
using ISpecialCaseEvaluator = DataExplorer.Abstractions.Specifications.Evaluators.ISpecialCaseEvaluator;
using ISpecificationValidator = DataExplorer.Abstractions.Specifications.Validators.ISpecificationValidator;
using IValidator = DataExplorer.Abstractions.Specifications.Validators.IValidator;
using IValidatorMarker = DataExplorer.Abstractions.Specifications.Validators.IValidatorMarker;
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
        
        options ??= _ => { };
        
        options.Invoke(config);

        var registrator = config.GetRegistrator();

        var toScan = assembliesToScanForServices.ToArray();
        
        var cache = new EfDataExplorerTypeCache(assembliesToScanForEntities);
        
        registrator.DescribeInstance(cache).As(typeof(IEfDataExplorerTypeCache)).WithLifetime(ServiceLifetime.SingleInstance).Register();

        registrator.DescribeOptions(options, config);

        registrator.DescribeOpenGeneric(typeof(UnitOfWork<>)).As(typeof(IUnitOfWork<>)).WithLifetime(ServiceLifetime.InstancePerLifetimeScope).Register();
        
        foreach (var assembly in toScan)
        {
            foreach (var type in assembly.GetTypes().Where(x => x.GetInterface(nameof(IValidatorMarker)) is not null || 
                                                                x.GetInterface(nameof(IInMemoryEvaluatorMarker)) is not null ||
                                                                x.GetInterface(nameof(IEvaluatorBase)) is not null 
                                                                && x.GetInterface(nameof(ISpecialCaseEvaluator)) is null))
            {
                var instance = Activator.CreateInstance(
                    type,
                    BindingFlags.Instance
                    | BindingFlags.Public
                    | BindingFlags.NonPublic,
                    null,
                    [],
                    null
                )!;
                    

            }
        }

        // handle non special
        foreach (var type in typeof(GroupByEvaluator).Assembly.GetTypes().Where(x => x.GetInterface(nameof(IValidatorMarker)) is not null || 
                     x.GetInterface(nameof(IInMemoryEvaluatorMarker)) is not null ||
                     x.GetInterface(nameof(IEvaluatorBase)) is not null 
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
                    
            registrator.DescribeInstance(instance).AsImplementedInterfaces().WithLifetime(ServiceLifetime.SingleInstance).Register();
        }
        
        registrator.DescribeInstance(new IncludeEvaluator(config.EnableIncludeCache)).As(typeof(IEvaluator)).WithLifetime(ServiceLifetime.SingleInstance).Register();
        
        registrator.DescribeInstance(new DefaultProjectionEvaluator()).As(typeof(IProjectionEvaluator)).WithLifetime(ServiceLifetime.SingleInstance).Register();

#if NET7_0_OR_GREATER
        registrator.DescribeInstance(new UpdateEvaluator()).As(typeof(IUpdateEvaluator)).WithLifetime(ServiceLifetime.SingleInstance).Register();
#endif


#if NET7_0_OR_GREATER
        registrator.DescribeFactory(x =>
            new SpecificationEvaluator(x.Resolve<IEnumerable<IEvaluator>>(),
                x.Resolve<IEnumerable<IBasicEvaluator>>(),
                x.Resolve<IEnumerable<IPreUpdateEvaluator>>(), x.Resolve<IProjectionEvaluator>(),
                x.Resolve<IUpdateEvaluator>()), typeof(SpecificationEvaluator)).As(typeof(ISpecificationEvaluator)).WithLifetime(ServiceLifetime.SingleInstance).Register();
        
        registrator.DescribeFactory(x => x.Resolve<ISpecificationEvaluator>(), typeof(SpecificationEvaluator)).As(typeof(IEfSpecificationEvaluator)).WithLifetime(ServiceLifetime.SingleInstance).Register();
#else
        registrator.DescribeFactory(x =>
            new SpecificationEvaluator(x.Resolve<IEnumerable<IEvaluator>>(),
                x.Resolve<IEnumerable<IBasicEvaluator>>(),
                x.Resolve<IEnumerable<IPreUpdateEvaluator>>(), x.Resolve<IProjectionEvaluator>(), typeof(SpecificationEvaluator)).As(typeof(ISpecialCaseEvaluator)).WithLifetime(ServiceLifetime.SingleInstance).Register();
#endif

        registrator.DescribeFactory(x =>
            new SpecificationValidator(x.Resolve<IEnumerable<IValidator>>(),
                x.Resolve<IEnumerable<IBasicValidator>>()), typeof(SpecificationValidator)).As(typeof(ISpecificationValidator)).WithLifetime(ServiceLifetime.SingleInstance).Register();

        registrator.DescribeFactory(x =>
            new InMemorySpecificationEvaluator(x.Resolve<IEnumerable<IInMemoryEvaluator>>(),
                x.Resolve<IEnumerable<IBasicInMemoryEvaluator>>()), typeof(InMemorySpecificationEvaluator)).As(typeof(IInMemorySpecificationEvaluator)).WithLifetime(ServiceLifetime.SingleInstance).Register();
        
        var registReadOnlyBuilder = registrator.DescribeOpenGeneric(typeof(ReadOnlyDataService<,>)).As(typeof(IReadOnlyDataService<,>)).WithLifetime(config.BaseGenericDataServiceLifetime);
        var registReadOnlyGenericIdBuilder = registrator.DescribeOpenGeneric(typeof(ReadOnlyDataService<,,>)).As(typeof(IReadOnlyDataService<,,>)).WithLifetime(config.BaseGenericDataServiceLifetime);
        var registCrudBuilder = registrator.DescribeOpenGeneric(typeof(CrudDataService<,>)).As(typeof(ICrudDataService<,>)).WithLifetime(config.BaseGenericDataServiceLifetime);
        var registCrudGenericIdBuilder = registrator.DescribeOpenGeneric(typeof(CrudDataService<,,>)).As(typeof(ICrudDataService<,,>)).WithLifetime(config.BaseGenericDataServiceLifetime);

        // base data interceptors
        var crudEnabled = false;
        var readEnabled = false;
        
        foreach (var (interceptorType, registrationData) in config.GenericDataInterceptors.OrderByDescending(x => x.Value.Order))
        {
            switch (registrationData.Strategy)
            {
                case DataRegistrationStrategy.CrudAndReadOnly:
                    registCrudBuilder = registCrudBuilder?.InterceptedBy(interceptorType);
                    registReadOnlyBuilder = registReadOnlyBuilder?.InterceptedBy(interceptorType);
                    registCrudGenericIdBuilder = registCrudGenericIdBuilder?.InterceptedBy(interceptorType);
                    registReadOnlyGenericIdBuilder = registReadOnlyGenericIdBuilder?.InterceptedBy(interceptorType);

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
                    registCrudBuilder = registCrudBuilder?.InterceptedBy(interceptorType);
                    registCrudGenericIdBuilder = registCrudGenericIdBuilder?.InterceptedBy(interceptorType);
                    if (!crudEnabled)
                    {
                        registCrudBuilder = registCrudBuilder?.EnableInterfaceInterceptors();
                        registCrudGenericIdBuilder = registCrudGenericIdBuilder?.EnableInterfaceInterceptors();
                        crudEnabled = true;
                    }

                    break;
                case DataRegistrationStrategy.ReadOnly:
                    registReadOnlyBuilder = registReadOnlyBuilder?.InterceptedBy(interceptorType);
                    registReadOnlyGenericIdBuilder = registReadOnlyGenericIdBuilder?.InterceptedBy(interceptorType);
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

        registReadOnlyBuilder?.Register();
        registReadOnlyGenericIdBuilder?.Register();
        registCrudBuilder?.Register();
        registCrudGenericIdBuilder?.Register();
           
        
        foreach (var (decoratorType, _) in config.GenericDataDecorators.OrderBy(x => x.Value))
        {
            if (decoratorType.IsAssignableToWithGenerics(typeof(ICrudDataService<,>)))
                registrator.DescribeOpenGenericDecorator(decoratorType, typeof(ICrudDataService<,>));
            if (decoratorType.IsAssignableToWithGenerics(typeof(IReadOnlyDataService<,>)))
                registrator.DescribeOpenGenericDecorator(decoratorType, typeof(IReadOnlyDataService<,>));
            if (decoratorType.IsAssignableToWithGenerics(typeof(ICrudDataService<,,>)))
                registrator.DescribeOpenGenericDecorator(decoratorType, typeof(ICrudDataService<,,>));
            if (decoratorType.IsAssignableToWithGenerics(typeof(IReadOnlyDataService<,,>)))
                registrator.DescribeOpenGenericDecorator(decoratorType, typeof(IReadOnlyDataService<,,>));
        }


        var excluded = new[] { typeof(IDataServiceBase<>), typeof(EfCoreDataServiceBase<>), typeof(CrudDataService<,>), typeof(ReadOnlyDataService<,>), typeof(CrudDataService<,,>), typeof(ReadOnlyDataService<,,>) };

        var hasCustomInterceptors = config.CustomDataInterceptors.Count != 0;
        var hasCustomDecorators = config.CustomDataDecorators.Count != 0;

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

                IRegistration? registrationGenericBuilder = null;
                IRegistration? registrationBuilder = null;

                if (dataType.IsGenericType && dataType.IsGenericTypeDefinition)
                {
                    if (intrEnableAttr is not null || hasCustomInterceptors)
                    {
                        registrationGenericBuilder = shouldAsInterfaces
                            ? registrator.DescribeOpenGeneric(dataType).AsImplementedInterfaces().EnableInterfaceInterceptors()
                            : registrator.DescribeOpenGeneric(dataType).EnableInterfaceInterceptors();
                    }
                    else
                    {
                        registrationGenericBuilder = shouldAsInterfaces
                            ? registrator.DescribeOpenGeneric(dataType).AsImplementedInterfaces()
                            : registrator.DescribeOpenGeneric(dataType);
                    }
                }
                else
                {
                    if (intrEnableAttr is not null || hasCustomInterceptors)
                    {
                        registrationBuilder = intrEnableAttr is not null
                            ? intrEnableAttr.InterceptionStrategy switch
                            {
                                InterceptionStrategy.Interface => shouldAsInterfaces
                                    ? registrator.Describe(dataType).AsImplementedInterfaces()
                                        .EnableInterfaceInterceptors()
                                    : registrator.Describe(dataType).EnableInterfaceInterceptors(),
                                InterceptionStrategy.Class => shouldAsInterfaces
                                    ? registrator.Describe(dataType).AsImplementedInterfaces().EnableClassInterceptors()
                                    : registrator.Describe(dataType).EnableClassInterceptors(),
                                _ => throw new ArgumentOutOfRangeException(nameof(intrEnableAttr.InterceptionStrategy))
                            }
                            : shouldAsInterfaces  
                                ? registrator.Describe(dataType).AsImplementedInterfaces().EnableInterfaceInterceptors()
                                : registrator.Describe(dataType).EnableInterfaceInterceptors(); 
                    }
                    else
                    {
                        registrationBuilder = shouldAsInterfaces
                            ? registrator.Describe(dataType).AsImplementedInterfaces()
                            : registrator.Describe(dataType);
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

                var tags = scopeOverrideAttr?.Tags?.ToArray();
                var owned = scopeOverrideAttr?.Owned;

                if (scope is ServiceLifetime.InstancePerMatchingLifetimeScope)
                {
                    if (!tags.AnyNullable())
                    {
                        throw new InvalidOperationException("Tags are not set");
                    }
                    
                    registrationBuilder = registrationBuilder?.WithMatchingLifetime(tags);
                    registrationGenericBuilder = registrationGenericBuilder?.WithMatchingLifetime(tags);
                }
                else if (scope is ServiceLifetime.InstancePerOwned)
                {
                    if (owned is null)
                    {
                        throw new InvalidOperationException("Owned type is not set");
                    }
                    
                    registrationBuilder = registrationBuilder?.WithOwnedLifetime(owned);
                    registrationGenericBuilder = registrationGenericBuilder?.WithOwnedLifetime(owned);
                }
                else
                {
                    registrationBuilder = registrationBuilder?.WithLifetime(scope);
                    registrationGenericBuilder = registrationGenericBuilder?.WithLifetime(scope);
                }

                var finalInterceptors = config.CustomDataInterceptors
                    .Select(x => x)
                    .ToList();
                finalInterceptors.AddRange(intrAttrs.Select(x => new KeyValuePair<Type, int>(x.Interceptor, x.RegistrationOrder)));
                
                if (finalInterceptors.GroupBy(x => x.Value).FirstOrDefault(x => x.Count() > 1) is not null)
                    throw new InvalidOperationException($"Duplicated interceptor registration order on type {dataType.Name}");

                if (finalInterceptors.GroupBy(x => x.Key)
                        .FirstOrDefault(x => x.Count() > 1) is not null)
                    throw new InvalidOperationException($"Duplicated interceptor type on type {dataType.Name}");

                foreach (var interceptor in finalInterceptors.OrderByDescending(x => x.Value).Select(x => x.Key).Distinct())
                {
                    registrationBuilder = registrationBuilder?.InterceptedBy(interceptor);
                    registrationGenericBuilder = registrationGenericBuilder?.InterceptedBy(interceptor);
                }

                registrationBuilder?.Register();
                registrationGenericBuilder?.Register();

                var decoratorAttributes = dataType.GetRegistrationAttributesOfType<IDecoratedByAttribute>().ToArray();
                if (decoratorAttributes.Length == 0 && !hasCustomDecorators)
                {
                    continue;
                }
                
                var finalDecorators = config.CustomDataDecorators
                    .Select(x => x)
                    .ToList();
                finalDecorators.AddRange(decoratorAttributes.Select(x => new KeyValuePair<Type, int>(x.Decorator, x.RegistrationOrder)));

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

                if (finalDecorators.GroupBy(x => x.Value).FirstOrDefault(x => x.Count() > 1) is not null)
                    throw new InvalidOperationException($"Duplicated decorator registration order on type {dataType.Name}");

                if (finalDecorators.GroupBy(x => x.Key)
                        .FirstOrDefault(x => x.Count() > 1) is not null)
                    throw new InvalidOperationException($"Duplicated decorator type on type {dataType.Name}");
                
                foreach (var attribute in finalDecorators.OrderBy(x => x.Value))
                {
                    if (attribute.Key.ShouldSkipRegistration<ISkipDecoratorRegistrationAttribute>())
                        continue;
            
                    if (attribute.Key.IsGenericType && attribute.Key.IsGenericTypeDefinition)
                    {
                        foreach (var serviceType in serviceTypes)
                            registrator.DescribeOpenGenericDecorator(attribute.Key, serviceType);
                    }
                    else
                    {
                        foreach (var serviceType in serviceTypes)
                            registrator.DescribeDecorator(attribute.Key, serviceType);
                    }
                }
            }
        }

        return configuration;
    }
}
