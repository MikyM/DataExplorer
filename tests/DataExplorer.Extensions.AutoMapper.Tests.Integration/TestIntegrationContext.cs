using DataExplorer.EfCore;
using DataExplorer.Tests.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace DataExplorer.Extensions.AutoMapper.Integration;

public class TestIntegrationContext : TestContext
{
    public TestIntegrationContext(DbContextOptions options) : base(options)
    {
    }

    public TestIntegrationContext(DbContextOptions options, DataExplorerEfCoreConfiguration config, DataExplorerTimeProvider timeProvider) : base(options, config, timeProvider)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new TestEntityConfiguration());
        base.OnModelCreating(modelBuilder);
    }
}
