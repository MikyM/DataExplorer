using DataExplorer.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DataExplorer.IdGenerator;

/// <summary>
/// Registers the default snowflake ID factory.
/// </summary>
[PublicAPI]
[UsedImplicitly]
public class SnowflakeIdFactoryRegistrator : IHostedService
{
    private readonly IServiceProvider _serviceProvider;

    public SnowflakeIdFactoryRegistrator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <inheritdoc/>
    public Task StartAsync(CancellationToken cancellationToken)
    {
        SnowflakeIdFactory.AddFactoryMethod(() => _serviceProvider.GetRequiredService<ISnowflakeGenerator>().Generate());
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}