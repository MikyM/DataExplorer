﻿using Autofac;
using DataExplorer.IdGenerator;
using IdGen.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
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
    /// Registers a singleton <see cref="IdGenerator"/> with the given <paramref name="generatorId"/>.
    /// </summary>
    /// <param name="generatorId">The generator-id to use for the singleton <see cref="IdGenerator"/>.</param>
    /// <returns>The given <see cref="ContainerBuilder"/> with the registered singleton in it.</returns>
    public DataExplorerConfiguration AddIdGen(int generatorId)
        => AddIdGen(generatorId, () => IdGeneratorOptions.Default);

    /// <summary>
    /// Registers a singleton <see cref="IdGenerator"/> with the given <paramref name="generatorId"/> and <see cref="IdGeneratorOptions"/>.
    /// </summary>
    /// <param name="generatorId">The generator-id to use for the singleton <see cref="IdGenerator"/>.</param>
    /// <param name="options">The <see cref="IdGeneratorOptions"/> for the singleton <see cref="IdGenerator"/>.</param>
    /// <returns>The given <see cref="ContainerBuilder"/> with the registered singleton <see cref="IdGenerator"/> in it.</returns>
    public DataExplorerConfiguration AddIdGen(int generatorId, Func<IdGeneratorOptions> options)
    {
        Builder?.AddIdGen(generatorId, options);
        ServiceCollection?.AddIdGen(generatorId, options);
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
