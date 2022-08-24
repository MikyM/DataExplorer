﻿using System.Reflection;
using AttributeBasedRegistration;
using AttributeBasedRegistration.Attributes;
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
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

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
    /// Automatically registers all base <see cref="IEvaluator"/> types, <see cref="ISpecificationValidator"/>, <see cref="IInMemorySpecificationEvaluator"/>, <see cref="ISpecificationEvaluator"/>, <see cref="IUnitOfWork"/> with <see cref="ContainerBuilder"/>.
    /// </remarks>
    /// <param name="configuration">Current instance of <see cref="DataExplorerConfiguration"/></param>
    /// <param name="options"><see cref="Action"/> that configures DAL.</param>
    public static DataExplorerConfiguration AddEfCore(this DataExplorerConfiguration configuration, Action<DataExplorerEfCoreConfiguration>? options = null)
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

        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            builder?.RegisterAssemblyTypes(assembly)
                .Where(x => x.GetInterface(nameof(IEvaluator)) is not null && x != typeof(IncludeEvaluator))
                .As<IEvaluator>()
                .FindConstructorsWith(ctorFinder)
                .SingleInstance();
            
            if (serviceCollection is not null)
                foreach (var type in assembly.GetTypes().Where(x => x.GetInterface(nameof(IEvaluator)) is not null && x != typeof(IncludeEvaluator)))
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
            case Lifetime.SingleInstance:
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
            case Lifetime.InstancePerRequest:
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
            case Lifetime.InstancePerLifetimeScope:
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
            case Lifetime.InstancePerMatchingLifetimeScope:
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
            case Lifetime.InstancePerDependency:
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
            case Lifetime.InstancePerOwned:
                throw new NotSupportedException();
            default:
                throw new ArgumentOutOfRangeException(nameof(config.BaseGenericDataServiceLifetime),
                    config.BaseGenericDataServiceLifetime, null);
        }

        // base data interceptors
        bool crudEnabled = false;
        bool readEnabled = false;
        if (builder is not null)
            foreach (var (interceptorType, dataConfig) in config.DataInterceptors)
            {
                switch (dataConfig)
                {
                    case DataInterceptorConfiguration.CrudAndReadOnly:
                        registCrudBuilder = interceptorType.GetInterfaces().Any(x => x == typeof(IAsyncInterceptor))
                            ? registCrudBuilder.InterceptedBy(
                                typeof(AsyncInterceptorAdapter<>).MakeGenericType(interceptorType))
                            : registCrudBuilder.InterceptedBy(interceptorType);
                        registReadOnlyBuilder = interceptorType.GetInterfaces().Any(x => x == typeof(IAsyncInterceptor))
                            ? registReadOnlyBuilder.InterceptedBy(
                                typeof(AsyncInterceptorAdapter<>).MakeGenericType(interceptorType))
                            : registReadOnlyBuilder.InterceptedBy(interceptorType);
                        registCrudGenericIdBuilder = interceptorType.GetInterfaces().Any(x => x == typeof(IAsyncInterceptor))
                            ? registCrudGenericIdBuilder.InterceptedBy(
                                typeof(AsyncInterceptorAdapter<>).MakeGenericType(interceptorType))
                            : registCrudGenericIdBuilder.InterceptedBy(interceptorType);
                        registReadOnlyGenericIdBuilder = interceptorType.GetInterfaces().Any(x => x == typeof(IAsyncInterceptor))
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
                    case DataInterceptorConfiguration.Crud:
                        registCrudBuilder = interceptorType.GetInterfaces().Any(x => x == typeof(IAsyncInterceptor))
                            ? registCrudBuilder.InterceptedBy(
                                typeof(AsyncInterceptorAdapter<>).MakeGenericType(interceptorType))
                            : registCrudBuilder.InterceptedBy(interceptorType);
                        registCrudGenericIdBuilder = interceptorType.GetInterfaces().Any(x => x == typeof(IAsyncInterceptor))
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
                    case DataInterceptorConfiguration.ReadOnly:
                        registReadOnlyBuilder = interceptorType.GetInterfaces().Any(x => x == typeof(IAsyncInterceptor))
                            ? registReadOnlyBuilder.InterceptedBy(
                                typeof(AsyncInterceptorAdapter<>).MakeGenericType(interceptorType))
                            : registReadOnlyBuilder.InterceptedBy(interceptorType);
                        registReadOnlyGenericIdBuilder = interceptorType.GetInterfaces().Any(x => x == typeof(IAsyncInterceptor))
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
                        throw new ArgumentOutOfRangeException(nameof(dataConfig));
                }
            }

        var excluded = new[] { typeof(IDataServiceBase<>), typeof(EfCoreDataServiceBase<>), typeof(CrudDataService<,>), typeof(ReadOnlyDataService<,>), typeof(CrudDataService<,,>), typeof(ReadOnlyDataService<,,>) };

        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
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
                if (dataType.GetCustomAttribute<SkipDataServiceRegistrationAttribute>(false) is not null)
                    continue;
                
                var scopeOverrideAttr = dataType.GetCustomAttribute<LifetimeAttribute>(false);
                var intrAttrs = dataType.GetCustomAttributes<InterceptedByAttribute>(false).ToList();
                var asAttr = dataType.GetCustomAttributes<RegisterAsAttribute>(false).ToList();
                var intrEnableAttr = dataType.GetCustomAttribute<EnableInterceptionAttribute>(false);

                var scope = scopeOverrideAttr?.Scope ?? config.DataServiceLifetime;

                var registerAsTypes = asAttr.Where(x => x.RegisterAsType is not null)
                    .Select(x => x.RegisterAsType)
                    .Distinct()
                    .ToList();
                var interfaceType = dataType.GetInterface($"I{dataType.Name}"); // by naming convention
                if (interfaceType is not null && !registerAsTypes.Contains(interfaceType))
                    registerAsTypes.Add(interfaceType);
                
                var shouldAsSelf = asAttr.Any(x => x.RegisterAsOption == RegisterAs.Self) &&
                                   asAttr.All(x => x.RegisterAsType != dataType);
                var shouldAsInterfaces =
                    !asAttr.Any() || asAttr.Any(x => x.RegisterAsOption == RegisterAs.ImplementedInterfaces);

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
                        registrationBuilder = intrEnableAttr.Intercept switch
                        {
                            Intercept.InterfaceAndClass => shouldAsInterfaces
                                ? builder?.RegisterType(dataType)
                                    .AsImplementedInterfaces()
                                    .EnableClassInterceptors()
                                    .EnableInterfaceInterceptors()
                                : builder?.RegisterType(dataType)
                                    .EnableClassInterceptors()
                                    .EnableInterfaceInterceptors(),
                            Intercept.Interface => shouldAsInterfaces
                                ? builder?.RegisterType(dataType).AsImplementedInterfaces().EnableInterfaceInterceptors()
                                : builder?.RegisterType(dataType).EnableInterfaceInterceptors(),
                            Intercept.Class => shouldAsInterfaces
                                ? builder?.RegisterType(dataType).AsImplementedInterfaces().EnableClassInterceptors()
                                : builder?.RegisterType(dataType).EnableClassInterceptors(),
                            _ => throw new ArgumentOutOfRangeException(nameof(intrEnableAttr.Intercept))
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

                foreach (var asType in registerAsTypes)
                {
                    if (asType is null) throw new InvalidOperationException("Type was null during registration");

                    registrationBuilder = registrationBuilder?.As(asType);
                    registrationGenericBuilder = registrationGenericBuilder?.As(asType);
                }

                var interfaces = dataType.GetInterfaces().ToList();
                
                switch (scope)
                {
                    case Lifetime.SingleInstance:
                        registrationBuilder = registrationBuilder?.SingleInstance();
                        registrationGenericBuilder = registrationGenericBuilder?.SingleInstance();

                        if (serviceCollection is not null)
                        {
                            if (shouldAsInterfaces)
                                interfaces.ForEach(x => serviceCollection.TryAddSingleton(x, dataType));
                            if (shouldAsSelf)
                                serviceCollection.TryAddSingleton(dataType);
                            if (registerAsTypes.Any())
                                registerAsTypes.ForEach(x => serviceCollection.TryAddSingleton(x, dataType));
                        }
                        break;
                    case Lifetime.InstancePerRequest:
                        registrationBuilder = registrationBuilder?.InstancePerRequest();
                        registrationGenericBuilder = registrationGenericBuilder?.InstancePerRequest();
                        
                        if (serviceCollection is not null)
                        {
                            if (shouldAsInterfaces)
                                interfaces.ForEach(x => serviceCollection.TryAddScoped(x, dataType));
                            if (shouldAsSelf)
                                serviceCollection.TryAddScoped(dataType);
                            if (registerAsTypes.Any())
                                registerAsTypes.ForEach(x => serviceCollection.TryAddScoped(x, dataType));
                        }
                        break;
                    case Lifetime.InstancePerLifetimeScope:
                        registrationBuilder = registrationBuilder?.InstancePerLifetimeScope();
                        registrationGenericBuilder = registrationGenericBuilder?.InstancePerLifetimeScope();
                        
                        if (serviceCollection is not null)
                        {
                            if (shouldAsInterfaces)
                                interfaces.ForEach(x => serviceCollection.TryAddScoped(x, dataType));
                            if (shouldAsSelf)
                                serviceCollection.TryAddScoped(dataType);
                            if (registerAsTypes.Any())
                                registerAsTypes.ForEach(x => serviceCollection.TryAddScoped(x, dataType));
                        }
                        break;
                    case Lifetime.InstancePerDependency:
                        registrationBuilder = registrationBuilder?.InstancePerDependency();
                        registrationGenericBuilder = registrationGenericBuilder?.InstancePerDependency();
                        
                        if (serviceCollection is not null)
                        {
                            if (shouldAsInterfaces)
                                interfaces.ForEach(x => serviceCollection.TryAddTransient(x, dataType));
                            if (shouldAsSelf)
                                serviceCollection.TryAddTransient(dataType);
                            if (registerAsTypes.Any())
                                registerAsTypes.ForEach(x => serviceCollection.TryAddTransient(x, dataType));
                        }
                        break;
                    case Lifetime.InstancePerMatchingLifetimeScope:
                        registrationBuilder =
                            registrationBuilder?.InstancePerMatchingLifetimeScope(scopeOverrideAttr?.Tags.ToArray() ??
                                Array.Empty<object>());
                        registrationGenericBuilder =
                            registrationGenericBuilder?.InstancePerMatchingLifetimeScope(
                                scopeOverrideAttr?.Tags.ToArray() ?? Array.Empty<object>());
                        break;
                    case Lifetime.InstancePerOwned:
                        if (scopeOverrideAttr?.Owned is null)
                            throw new InvalidOperationException("Owned type was null");

                        registrationBuilder = registrationBuilder?.InstancePerOwned(scopeOverrideAttr.Owned);
                        registrationGenericBuilder =
                            registrationGenericBuilder?.InstancePerOwned(scopeOverrideAttr.Owned);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(scope));
                }

                foreach (var attr in intrAttrs)
                {
                    registrationBuilder = attr.IsAsync
                        ? registrationBuilder?.InterceptedBy(
                            typeof(AsyncInterceptorAdapter<>).MakeGenericType(attr.Interceptor))
                        : registrationBuilder?.InterceptedBy(attr.Interceptor);
                    registrationGenericBuilder = attr.IsAsync
                        ? registrationGenericBuilder?.InterceptedBy(
                            typeof(AsyncInterceptorAdapter<>).MakeGenericType(attr.Interceptor))
                        : registrationGenericBuilder?.InterceptedBy(attr.Interceptor);
                }
            }
        }
        
        return configuration;
    }
}
