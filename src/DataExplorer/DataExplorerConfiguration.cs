using DataExplorer.Abstractions;
using DataExplorer.IdGenerator;
using DataExplorer.Services;
using Microsoft.Extensions.Options;
using IdGeneratorOptions = IdGen.IdGeneratorOptions;
using ServiceLifetime = AttributeBasedRegistration.ServiceLifetime;

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
    public DataExplorerConfiguration(IRegistrator registrator) : base(registrator)
    {
    }
    
    /// <summary>
    /// Creates an instance of the configuration class.
    /// </summary>
    public DataExplorerConfiguration(DataExplorerConfigurationBase configurationBase) : base(configurationBase)
    {
    }
    
    /// <summary>
    /// Gets the registrator service.
    /// </summary>
    internal IRegistrator GetRegistrator()
        => Registrator;
    
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
        var opt = options();
        
        Registrator.DescribeInstance(new IdGen.IdGenerator(generatorId, opt)).As(typeof(IdGen.IIdGenerator<long>))
            .WithLifetime(ServiceLifetime.SingleInstance).Register();

        Registrator.Describe(typeof(SnowflakeGenerator)).As(typeof(ISnowflakeGenerator))
            .WithLifetime(ServiceLifetime.SingleInstance).Register();
        
        Registrator.DescribeOptions(opt);

        Registrator.DescribeHostedService<SnowflakeIdFactoryRegistrator>();

        return this;
    }

    /// <summary>
    /// Adds a custom snowflake Id generator.
    /// </summary>
    /// <returns>Current <see cref="DataExplorerConfiguration"/> instance.</returns>
    public DataExplorerConfiguration AddSnowflakeIdGenerator<TGenerator>() where TGenerator : class, ISnowflakeGenerator
    {
        Registrator.Describe(typeof(TGenerator)).As(typeof(ISnowflakeGenerator))
            .WithLifetime(ServiceLifetime.SingleInstance).Register();

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
