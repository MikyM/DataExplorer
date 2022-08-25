using Autofac;
using DataExplorer.IdGenerator;
using DataExplorer.Services;
using IdGen.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using IdGeneratorOptions = IdGen.IdGeneratorOptions;

namespace DataExplorer;

/// <summary>
/// Configuration of the data explorer.
/// </summary>
[PublicAPI]
public class DataExplorerConfiguration
{
    /// <summary>
    /// Creates an instance of the configuration class.
    /// </summary>
    /// <param name="builder"></param>
    public DataExplorerConfiguration(ContainerBuilder builder)
    {
        Builder = builder;
    }
    
    /// <summary>
    /// Creates an instance of the configuration class.
    /// </summary>
    /// <param name="serviceCollection"></param>
    public DataExplorerConfiguration(IServiceCollection serviceCollection)
    {
        ServiceCollection = serviceCollection;
    }

    internal readonly ContainerBuilder? Builder;
    internal readonly IServiceCollection? ServiceCollection;

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
        ServiceCollection?.TryAddSingleton<IdGen.IIdGenerator<long>>(new IdGen.IdGenerator(generatorId, options()));
        ServiceCollection?.TryAddSingleton(c => (IdGen.IdGenerator)c.GetRequiredService<IdGen.IIdGenerator<long>>());
        ServiceCollection?.TryAddSingleton<ISnowflakeIdGenerator, SnowflakeIdGenerator>();
        ServiceCollection?.TryAddSingleton<ISnowflakeIdFiller, SnowflakeIdFiller>();
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
public enum DataInterceptorConfiguration
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
