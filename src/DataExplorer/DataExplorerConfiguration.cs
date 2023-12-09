using System.Reflection;
using Autofac;
using AutoMapper;
using AutoMapper.Extensions.ExpressionMapping;
using DataExplorer.Gridify;
using DataExplorer.IdGenerator;
using DataExplorer.Services;
using Gridify;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using IdGeneratorOptions = IdGen.IdGeneratorOptions;

namespace DataExplorer;

/// <summary>
/// Configuration of the data explorer.
/// </summary>
[PublicAPI]
public sealed class DataExplorerConfiguration : DataExplorerConfigurationBase, IOptions<DataExplorerConfiguration>
{
    /// <summary>
    /// Creates an instance of the configuration class.
    /// </summary>
    public DataExplorerConfiguration(IServiceCollection serviceCollection) : base(serviceCollection)
    {
    }
    
    /// <summary>
    /// Creates an instance of the configuration class.
    /// </summary>
    public DataExplorerConfiguration(ContainerBuilder builder) : base(builder)
    {
    }
    
    /// <summary>
    /// Creates an instance of the configuration class.
    /// </summary>
    public DataExplorerConfiguration(DataExplorerConfigurationBase configurationBase) : base(configurationBase)
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
    
    internal IGridifyMapperProvider? MapperProvider { get; set; }
    
    /// <summary>
    /// Gets or sets AutoMapper's configuration, defaults to opt.AddExpressionMapping().
    /// </summary>
    public Action<IMapperConfigurationExpression> AutoMapperConfiguration { get; set; } =
        opt => opt.AddExpressionMapping();
    
    /// <summary>
    /// Gets or sets the accessor used to get assemblies to scan for AutoMapper's profiles, defaults to all assemblies.
    /// </summary>
    public Func<IEnumerable<Assembly>> AutoMapperProfileAssembliesAccessor { get; set; } = () => AppDomain.CurrentDomain.GetAssemblies();
    
    /// <summary>
    /// Adds a <see cref="IGridifyMapper{T}"/> to the <see cref="IGridifyMapperProvider"/>.
    /// </summary>
    /// <returns>Current <see cref="DataExplorerConfiguration"/> instance.</returns>
    public DataExplorerConfiguration AddGridifyMapper<T>() where T : class, IGridifyMapper<T>, new()
    {
        MapperProvider ??= new GridifyMapperProvider();
        
        ((GridifyMapperProvider)MapperProvider).AddMapper(new T());
        return this;
    }
    
    /// <summary>
    /// Adds a <see cref="IGridifyMapper{T}"/> to the <see cref="IGridifyMapperProvider"/>.
    /// </summary>
    /// <returns>Current <see cref="DataExplorerConfiguration"/> instance.</returns>
    public DataExplorerConfiguration AddGridifyMapper<T>(IGridifyMapper<T> mapper) where T : class
    {
        MapperProvider ??= new GridifyMapperProvider();
        
        var addRes = ((GridifyMapperProvider)MapperProvider).AddMapper(mapper);
        if (!addRes)
            throw new InvalidOperationException($"Two gridify mappers were registered for the type: {typeof(T).Name}");
        
        return this;
    }
    
    /// <summary>
    /// Registers a customized implementation of <see cref="IGridifyMapperProvider"/>.
    /// </summary>
    /// <returns>Current <see cref="DataExplorerConfiguration"/> instance.</returns>
    public DataExplorerConfiguration UseGridifyMapperProvider<TProvider>(TProvider provider) where TProvider : class, IGridifyMapperProvider
    {
        MapperProvider = provider;
        return this;
    }
    
    /// <summary>
    /// Registers a customized implementation of <see cref="IGridifyMapperProvider"/>.
    /// </summary>
    /// <returns>Current <see cref="DataExplorerConfiguration"/> instance.</returns>
    public DataExplorerConfiguration UseGridifyMapperProvider<TProvider>() where TProvider : class, IGridifyMapperProvider, new()
    {
        MapperProvider = new TProvider();
        return this;
    }
    
    /// <summary>
    /// Registers required Id generator services with the given <paramref name="generatorId"/>.
    /// </summary>
    /// <param name="generatorId">The generator-id to use for the singleton <see cref="IdGenerator"/> if using provided implementation.</param>
    /// <returns>Current <see cref="DataExplorerConfiguration"/> instance.</returns>
    public DataExplorerConfiguration AddSnowflakeIdGeneration(int generatorId = 1)
        => AddSnowflakeIdGeneration(generatorId, () => IdGeneratorOptions.Default);

    /// <summary>
    /// Registers required Id generator services with the given <paramref name="generatorId"/>.
    /// </summary>
    /// <param name="generatorId">The generator-id to use for the singleton <see cref="IdGenerator"/>.</param>
    /// <param name="options">The <see cref="IdGeneratorOptions"/> for the singleton <see cref="IdGenerator"/>.</param>
    /// <returns>Current <see cref="DataExplorerConfiguration"/> instance.</returns>
    public DataExplorerConfiguration AddSnowflakeIdGeneration(int generatorId, Func<IdGeneratorOptions> options)
    {
        Builder?.AddIdGen(generatorId, options);

        var opt = options();
        ServiceCollection?.AddSingleton<IdGen.IIdGenerator<long>>(new IdGen.IdGenerator(generatorId, opt));

        Builder?.RegisterType<SnowflakeGenerator>().As<ISnowflakeGenerator>().SingleInstance();
        ServiceCollection?.AddSingleton<ISnowflakeGenerator, SnowflakeGenerator>();

        Builder?.RegisterBuildCallback(x =>
            SnowflakeIdFactory.AddFactoryMethod(() => x.Resolve<ISnowflakeGenerator>().Generate()));

        if (Builder is null)
        {
            var iopt = Options.Create(opt);
            ServiceCollection?.AddSingleton(iopt);
            ServiceCollection?.AddSingleton(x => x.GetRequiredService<IOptions<IdGeneratorOptions>>().Value);
        }

        return this;
    }

    /// <summary>
    /// Adds a custom snowflake Id generator.
    /// </summary>
    /// <returns>Current <see cref="DataExplorerConfiguration"/> instance.</returns>
    public DataExplorerConfiguration AddSnowflakeIdGenerator<TGenerator>() where TGenerator : class, ISnowflakeGenerator
    {
        Builder?.RegisterType<TGenerator>().As<ISnowflakeGenerator>().SingleInstance();
        ServiceCollection?.AddSingleton<TGenerator>();
        return this;
    }

    /// <inheritdoc/>
    DataExplorerConfiguration IOptions<DataExplorerConfiguration>.Value => this;
}

/// <summary>
/// Configuration for base data service interceptors.
/// </summary>
public enum DataRegistrationStrategy
{
    /// <summary>
    /// Crud and read-only.
    /// </summary>
    CrudAndReadOnly,
    /// <summary>
    /// Crud.
    /// </summary>
    Crud,
    /// <summary>
    /// Read-only.
    /// </summary>
    ReadOnly
}
