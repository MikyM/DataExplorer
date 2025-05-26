using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DataExplorer.EfCore.Tests.Integration;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<TestIntegrationContext>
{
    public TestIntegrationContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<TestIntegrationContext>();
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=test;Username=test;Password=test");

        return new TestIntegrationContext(optionsBuilder.Options,
            Options.Create(new DataExplorerEfCoreConfiguration(new MicrosoftRegistrator(new ServiceCollection()))),
            new DataExplorerTimeProvider.StaticDataExplorerTimeProvider());
    }
}