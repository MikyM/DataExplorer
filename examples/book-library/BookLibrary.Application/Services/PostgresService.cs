using BookLibrary.DataAccessLayer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Testcontainers.PostgreSql;

namespace BookLibrary.Application.Services;

public class PostgresService : IHostedService
{
    private readonly PostgreSqlContainer _container;
    private readonly IServiceProvider _rootProvider;
    
    public string GetConnectionString() => _container.GetConnectionString();

    public PostgresService(IServiceProvider rootProvider)
    {
        _rootProvider = rootProvider;
        
        _container = new PostgreSqlBuilder()
            .WithDatabase("book-library")
            .WithUsername("test")
            .WithPassword("test")
            .WithImage("postgres:16.1-bookworm")
            .WithPortBinding(5432, 5432)
            .Build();
    }
    
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _container.StartAsync(cancellationToken);
        
        await using var childScope = _rootProvider.CreateAsyncScope();
        var ctx = childScope.ServiceProvider.GetRequiredService<ILibraryDbContext>();

        await ctx.Database.EnsureDeletedAsync(cancellationToken);
        await ctx.Database.MigrateAsync(cancellationToken: cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await using var childScope = _rootProvider.CreateAsyncScope();
        var ctx = childScope.ServiceProvider.GetRequiredService<ILibraryDbContext>();
        
        await ctx.Database.EnsureDeletedAsync(cancellationToken);
        
        await _container.StopAsync(cancellationToken);
        await _container.DisposeAsync();
    }
}
