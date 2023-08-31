using Autofac;
using DataExplorer.IdGenerator;
using DataExplorer.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using IdGeneratorOptions = IdGen.IdGeneratorOptions;

namespace DataExplorer;

/// <summary>
/// Configuration of the data explorer.
/// </summary>
[PublicAPI]
public sealed class DataExplorerConfiguration : DataExplorerConfigurationBase
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

        Builder?.RegisterType<SnowflakeIdGenerator>().As<ISnowflakeIdGenerator>().SingleInstance();
        ServiceCollection?.AddSingleton<ISnowflakeIdGenerator, SnowflakeIdGenerator>();

        Builder?.RegisterBuildCallback(x =>
            SnowflakeIdFactory.AddFactoryMethod(() => x.Resolve<ISnowflakeIdGenerator>().GenerateId(), SnowflakeIdFactory.DefaultFactoryId));

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
    public DataExplorerConfiguration AddSnowflakeIdGenerator<TGenerator>() where TGenerator : class, ISnowflakeIdGenerator
    {
        Builder?.RegisterType<TGenerator>().As<ISnowflakeIdGenerator>().SingleInstance();
        ServiceCollection?.AddSingleton<TGenerator>();
        return this;
    }
    
    /// <summary>
    /// Adds a custom snowflake Id filler.
    /// </summary>
    /// <returns>Current <see cref="DataExplorerConfiguration"/> instance.</returns>
    public DataExplorerConfiguration AddSnowflakeIdFiller<TFiller>() where TFiller : class, ISnowflakeIdFiller
    {
        Builder?.RegisterType<TFiller>().As<ISnowflakeIdFiller>().SingleInstance();
        ServiceCollection?.AddSingleton<TFiller>();
        return this;
    }

    /// <summary>
    /// Registers  <see cref="ContainerBuilder"/>.
    /// </summary>
    /// <returns>Current instance of the <see cref="DataExplorerConfiguration"/></returns>
    public DataExplorerConfiguration AddErrorHandlingSyncInterceptor(LogLevel diagnosticLogLevel = LogLevel.Trace,
        LogLevel errorLogLevel = LogLevel.Error, bool reThrowExceptions = false)
    {
        //todo

        return this;
    }
    
    /// <summary>
    /// Registers  <see cref="ContainerBuilder"/>.
    /// </summary>
    /// <returns>Current instance of the <see cref="DataExplorerConfiguration"/></returns>
    public DataExplorerConfiguration AddErrorHandlingAsyncInterceptor(LogLevel diagnosticLogLevel = LogLevel.Trace,
        LogLevel errorLogLevel = LogLevel.Error, bool reThrowExceptions = false)
    {
        //todo

        return this;
    }
    
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
