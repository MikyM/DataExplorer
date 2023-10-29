using DataExplorer.EfCore.Abstractions.DataContexts;
using DataExplorer.Tests.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;

namespace DataExplorer.EfCore.Tests.Unit;

public class ContextFixture
{
    public Mock<TestContext> GetTextContextMock(TimeProvider? timeProvider = null, bool callBase = true)
    {
        var opt = Options.Create(new DataExplorerEfCoreConfiguration(new ServiceCollection()));
        var inMemoryDatabase = new DbContextOptionsBuilder().UseInMemoryDatabase("test");
        var ctx = new Mock<TestContext>(inMemoryDatabase.Options, opt, timeProvider ?? TimeProvider.System)
        {
            CallBase = callBase
        };

        return ctx;
    }

    public Mock<IEfDbContext> GetEfDbContextMock() => new();

    public Mock<ITestContext> GetITextContextMock(bool callBase = true) => new() { CallBase = callBase };

    public ITestContext GetTextContext(TimeProvider? timeProvider = null, bool ensureCreated = true)
    {
        var opt = Options.Create(new DataExplorerEfCoreConfiguration(new ServiceCollection()));
        var inMemoryDatabase = new DbContextOptionsBuilder().UseInMemoryDatabase("test");
        var ctx = new TestContext(inMemoryDatabase.Options, opt, timeProvider ?? TimeProvider.System);
        if (ensureCreated)
            ctx.Database.EnsureCreated();
        return ctx;
    }
    
    public ITestContext GetTextContext(DataExplorerEfCoreConfiguration config, TimeProvider? timeProvider = null, bool ensureCreated = true)
    {
        var opt = Options.Create(config);
        var inMemoryDatabase = new DbContextOptionsBuilder().UseInMemoryDatabase("test");
        var ctx = new TestContext(inMemoryDatabase.Options, opt, timeProvider ?? TimeProvider.System);
        if (ensureCreated)
            ctx.Database.EnsureCreated();
        return ctx;
    }

    public Mock<TimeProvider> GetTimeProviderMock() => new();
}
