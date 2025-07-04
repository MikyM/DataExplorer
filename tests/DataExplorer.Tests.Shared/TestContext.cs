using DataExplorer.EfCore;
using DataExplorer.EfCore.Abstractions.DataContexts;
using DataExplorer.EfCore.DataContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace DataExplorer.Tests.Shared;

public class TestContext : EfDbContext, ITestContext
{
    public TestContext(DbContextOptions options) : base(options)
    {
    }
    
    public TestContext(DbContextOptions options, DataExplorerEfCoreConfiguration config, DataExplorerTimeProvider timeProvider) : base(options, config, timeProvider)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TestEntity>();
        modelBuilder.Entity<TestEntityOffset>();
        
        base.OnModelCreating(modelBuilder);
    }
}

public interface ITestContext : IEfDbContext
{
}

public class TestContextPooled : EfDbContext, ITestContextPooled
{
    public TestContextPooled(DbContextOptions<TestContextPooled> options) : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TestEntity>();
        modelBuilder.Entity<TestEntityOffset>();
        
        base.OnModelCreating(modelBuilder);
    }
}

public interface ITestContextPooled : IEfDbContext
{
}
