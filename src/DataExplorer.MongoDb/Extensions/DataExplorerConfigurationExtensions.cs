/*
using System.Reflection;
using AttributeBasedRegistration;
using AttributeBasedRegistration.Attributes;
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
using DataExplorer.EfCore.Specifications;
using DataExplorer.EfCore.Specifications.Evaluators;
using DataExplorer.EfCore.Specifications.Validators;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MikyM.Utilities.Extensions;
using ServiceLifetime = AttributeBasedRegistration.ServiceLifetime;

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
    /// <param name="assembliesContainingTypesToScan">Assemblies containing types to scan DataExplorer services such as data services, validators, evaluators etc.</param>
    /// <param name="options"><see cref="Action"/> that configures DAL.</param>
    public static DataExplorerConfiguration AddEfCore(this DataExplorerConfiguration configuration,
        IEnumerable<Type> assembliesContainingTypesToScan, Action<DataExplorerEfCoreConfiguration>? options = null)
        => AddEfCore(configuration, assembliesContainingTypesToScan.Select(x => x.Assembly).Distinct(), options);
        
    /// <summary>
    /// Adds Data Access Layer to the application.
    /// </summary>
    /// <remarks>
    /// Automatically registers all base <see cref="IValidator"/> types, <see cref="IInMemoryEvaluator"/> types, <see cref="IEvaluator"/> types, <see cref="IProjectionEvaluator"/>, <see cref="ISpecificationValidator"/>, <see cref="IInMemorySpecificationEvaluator"/>, <see cref="ISpecificationEvaluator"/>, <see cref="IUnitOfWork"/>, <see cref="ICrudDataService{TEntity,TId,TContext}"/>, <see cref="ICrudDataService{TEntity,TContext}"/>, <see cref="IReadOnlyDataService{TEntity,TId,TContext}"/>, <see cref="IReadOnlyDataService{TEntity,TContext}"/>, <see cref="IDataServiceBase{TContext}"/> with the DI container.
    /// </remarks>
    /// <param name="configuration">Current instance of <see cref="DataExplorerConfiguration"/></param>
    /// <param name="assembliesToScan">Assemblies to scan for DataExplorer services such as data services, validators, evaluators etc.</param>
    /// <param name="options"><see cref="Action"/> that configures DAL.</param>
    public static DataExplorerConfiguration AddEfCore(this DataExplorerConfiguration configuration, IEnumerable<Assembly> assembliesToScan, Action<DataExplorerEfCoreConfiguration>? options = null)
    {
        var builder = configuration.Builder;
        var serviceCollection = configuration.ServiceCollection;
        var config = new DataExplorerEfCoreConfiguration(builder, serviceCollection);
        options?.Invoke(config);

        var ctorFinder = new AllConstructorsFinder();

        var iopt = Options.Create(config);
        builder?.RegisterInstance(iopt).As<IOptions<DataExplorerEfCoreConfiguration>>().SingleInstance();
        builder?.Register(x => x.Resolve<IOptions<DataExplorerEfCoreConfiguration>>().Value).AsSelf().SingleInstance();
        builder?.RegisterGeneric(typeof(UnitOfWork<>)).As(typeof(IUnitOfWork<>)).InstancePerLifetimeScope();
        
        serviceCollection?.AddSingleton(iopt);
        serviceCollection?.AddSingleton(x => x.GetRequiredService<IOptions<DataExplorerEfCoreConfiguration>>().Value);
        serviceCollection?.AddScoped(typeof(IUnitOfWork<>), typeof(UnitOfWork<>));

        var toScan = assembliesToScan.ToList();
        
        if (builder is not null)
            foreach (var assembly in toScan)
            {
                builder?.RegisterAssemblyTypes(assembly)
                    .Where(x => x.GetInterface(nameof(IInMemoryEvaluator)) is not null)
                    .As<IInMemoryEvaluator>()
                    .FindConstructorsWith(new AllConstructorsFinder())
                    .SingleInstance();
            }
        
        if (serviceCollection is not null)
            foreach (var assembly in toScan)
            {
                foreach (var type in assembly.GetTypes().Where(x => x.GetInterface(nameof(IInMemoryEvaluator)) is not null))
                {
                    serviceCollection.AddSingleton(typeof(IInMemoryEvaluator), _ => Activator.CreateInstance(
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
        
        if (builder is not null)
            foreach (var assembly in toScan)
            {
                builder?.RegisterAssemblyTypes(assembly)
                    .Where(x => x.GetInterface(nameof(IValidator)) is not null)
                    .As<IValidator>()
                    .FindConstructorsWith(new AllConstructorsFinder())
                    .SingleInstance();
            }
        
        if (serviceCollection is not null)
            foreach (var assembly in toScan)
            {
                foreach (var type in assembly.GetTypes().Where(x => x.GetInterface(nameof(IValidator)) is not null))
                {
                    serviceCollection.AddSingleton(typeof(IValidator), _ => Activator.CreateInstance(
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

        builder?.RegisterAssemblyTypes(typeof(GroupByEvaluator).Assembly)
            .Where(x => x.GetInterface(nameof(IEvaluator)) is not null && x != typeof(IncludeEvaluator))
            .As<IEvaluator>()
            .FindConstructorsWith(ctorFinder)
            .SingleInstance();
            
        if (serviceCollection is not null)
            foreach (var type in typeof(GroupByEvaluator).Assembly.GetTypes().Where(x => x.GetInterface(nameof(IEvaluator)) is not null && x != typeof(IncludeEvaluator)))
            {
                serviceCollection.AddSingleton(typeof(IEvaluator), _ => Activator.CreateInstance(
                    type,
                    BindingFlags.Instance
                    | BindingFlags.Public
                    | BindingFlags.NonPublic,
                    null,
                    Array.Empty<object>(),
                    null
                )!);
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

        builder?.RegisterType<SpecificationEvaluator>()
            .As<ISpecificationEvaluator>()
            .UsingConstructor(typeof(IEnumerable<IEvaluator>), typeof(IProjectionEvaluator))
            .FindConstructorsWith(ctorFinder)
            .SingleInstance();
        
        serviceCollection?.AddSingleton<ISpecificationEvaluator>(x => new SpecificationEvaluator(x.GetRequiredService<IEnumerable<IEvaluator>>(), x.GetRequiredService<IProjectionEvaluator>()));

        builder?.RegisterType<SpecificationValidator>()
            .As<ISpecificationValidator>()
            .UsingConstructor(typeof(IEnumerable<IValidator>))
            .FindConstructorsWith(ctorFinder)
            .SingleInstance();
        
        serviceCollection?.AddSingleton<ISpecificationValidator>(x => new SpecificationValidator(x.GetRequiredService<IEnumerable<IValidator>>()));

        builder?.RegisterType<InMemorySpecificationEvaluator>()
            .As<IInMemorySpecificationEvaluator>()
            .UsingConstructor(typeof(IEnumerable<IInMemoryEvaluator>))
            .FindConstructorsWith(ctorFinder)
            .SingleInstance();

        serviceCollection?.AddSingleton<IInMemorySpecificationEvaluator>(x =>
            new InMemorySpecificationEvaluator(x.GetRequiredService<IEnumerable<IInMemoryEvaluator>>()));
        
        
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
        bool crudEnabled = false;
        bool readEnabled = false;
        if (builder is not null)
            foreach (var (interceptorType, registrationData) in config.DataInterceptors.OrderByDescending(x => x.Value.Order))
            {
                switch (registrationData.Strategy)
                {
                    case DataRegistrationStrategy.CrudAndReadOnly:
                        registCrudBuilder = interceptorType.IsAsyncInterceptor()
                            ? registCrudBuilder.InterceptedBy(
                                typeof(AsyncInterceptorAdapter<>).MakeGenericType(interceptorType))
                            : registCrudBuilder.InterceptedBy(interceptorType);
                        registReadOnlyBuilder = interceptorType.IsAsyncInterceptor()
                            ? registReadOnlyBuilder.InterceptedBy(
                                typeof(AsyncInterceptorAdapter<>).MakeGenericType(interceptorType))
                            : registReadOnlyBuilder.InterceptedBy(interceptorType);
                        registCrudGenericIdBuilder = interceptorType.IsAsyncInterceptor()
                            ? registCrudGenericIdBuilder.InterceptedBy(
                                typeof(AsyncInterceptorAdapter<>).MakeGenericType(interceptorType))
                            : registCrudGenericIdBuilder.InterceptedBy(interceptorType);
                        registReadOnlyGenericIdBuilder = interceptorType.IsAsyncInterceptor()
                            ? registReadOnlyGenericIdBuilder.InterceptedBy(
                                typeof(AsyncInterceptorAdapter<>).MakeGenericType(interceptorType))
                            : registReadOnlyGenericIdBuilder.InterceptedBy(interceptorType);

                        if (!crudEnabled)
                        {
                            registCrudBuilder = registCrudBuilder.EnableInterfaceInterceptors();
                            registCrudGenericIdBuilder = registCrudGenericIdBuilder.EnableInterfaceInterceptors();
                            crudEnabled = true;
                        }

                        if (!readEnabled)
                        {
                            registReadOnlyBuilder = registReadOnlyBuilder.EnableInterfaceInterceptors();
                            registReadOnlyGenericIdBuilder = registReadOnlyGenericIdBuilder.EnableInterfaceInterceptors();
                            readEnabled = true;
                        }

                        break;
                    case DataRegistrationStrategy.Crud:
                        registCrudBuilder = interceptorType.IsAsyncInterceptor()
                            ? registCrudBuilder.InterceptedBy(
                                typeof(AsyncInterceptorAdapter<>).MakeGenericType(interceptorType))
                            : registCrudBuilder.InterceptedBy(interceptorType);
                        registCrudGenericIdBuilder = interceptorType.IsAsyncInterceptor()
                            ? registCrudGenericIdBuilder.InterceptedBy(
                                typeof(AsyncInterceptorAdapter<>).MakeGenericType(interceptorType))
                            : registCrudGenericIdBuilder.InterceptedBy(interceptorType);
                        if (!crudEnabled)
                        {
                            registCrudBuilder = registCrudBuilder.EnableInterfaceInterceptors();
                            registCrudGenericIdBuilder = registCrudGenericIdBuilder.EnableInterfaceInterceptors();
                            crudEnabled = true;
                        }

                        break;
                    case DataRegistrationStrategy.ReadOnly:
                        registReadOnlyBuilder = interceptorType.IsAsyncInterceptor()
                            ? registReadOnlyBuilder.InterceptedBy(
                                typeof(AsyncInterceptorAdapter<>).MakeGenericType(interceptorType))
                            : registReadOnlyBuilder.InterceptedBy(interceptorType);
                        registReadOnlyGenericIdBuilder = interceptorType.IsAsyncInterceptor()
                            ? registReadOnlyGenericIdBuilder.InterceptedBy(
                                typeof(AsyncInterceptorAdapter<>).MakeGenericType(interceptorType))
                            : registReadOnlyGenericIdBuilder.InterceptedBy(interceptorType);
                        if (!readEnabled)
                        {
                            registReadOnlyBuilder = registReadOnlyBuilder.EnableInterfaceInterceptors();
                            registReadOnlyGenericIdBuilder = registReadOnlyGenericIdBuilder.EnableInterfaceInterceptors();
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
*/
