using DataExplorer.Tests.Shared;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Moq.Protected;

namespace DataExplorer.EfCore.Tests.Unit;

public class EfDbContextShould : IClassFixture<ContextFixture>
{
    private readonly ContextFixture _fixture;

    public EfDbContextShould(ContextFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Call_OnBeforeSaveChangesAsync_1_arg()
    {
        // Arrange
        var ctx = _fixture.GetTextContextMock();

        // Act
        await ctx.Object.SaveChangesAsync(CancellationToken.None);
        
        // Assert
        ctx.Protected().Verify("OnBeforeSaveChangesAsync", Times.Once(), ItExpr.IsAny<IReadOnlyList<EntityEntry>?>());
    }
    
    [Fact]
    public async Task Call_OnBeforeSaveChangesAsync_2_arg()
    {
        // Arrange
        var ctx = _fixture.GetTextContextMock();

        // Act
        await ctx.Object.SaveChangesAsync(true, CancellationToken.None);
        
        // Assert
        ctx.Protected().Verify("OnBeforeSaveChangesAsync", Times.Once(), ItExpr.IsAny<IReadOnlyList<EntityEntry>?>());
    }
    
    [Fact]
    public async Task Call_GetTrackedEntries()
    {
        // Arrange
        var ctx = _fixture.GetTextContextMock();

        // Act
        await ctx.Object.SaveChangesAsync(CancellationToken.None);
        
        // Assert
        ctx.Protected().Verify("GetTrackedEntries", Times.Once());
    }
    
    [Fact]
    public async Task Fill_UpdatedAt_for_UpdatedAt_when_updated_entries_when_non_modified()
    {
        // Arrange
        var timeProvider = _fixture.GetTimeProviderMock();
        var futureNow = TimeProvider.System.GetUtcNow().Add(TimeSpan.FromMinutes(2));
        timeProvider.Setup(x => x.GetUtcNow()).Returns(futureNow);

        await using var ctx = _fixture.GetTextContext(timeProvider.Object);
        var entry = TestEntity.Create();
        
        var now = TimeProvider.System.GetUtcNow();
        
        entry.UpdatedAt = now.DateTime;
        
        ctx.Add(entry);
        
        await ctx.SaveChangesAsync();
        
        // Act
        entry.Name = "b";
        await ctx.SaveChangesAsync();
        
        // Assert
        entry.UpdatedAt.Should().NotBeNull().And.Be(futureNow.DateTime);
    }
    
    [Fact]
    public async Task Fill_UpdatedAt_for_UpdatedAtOffset_when_updated_entries_when_non_modified()
    {
        // Arrange
        var timeProvider = _fixture.GetTimeProviderMock();
        var futureNow = TimeProvider.System.GetUtcNow().Add(TimeSpan.FromMinutes(2));
        timeProvider.Setup(x => x.GetUtcNow()).Returns(futureNow);

        await using var ctx = _fixture.GetTextContext(timeProvider.Object);
        var entry = TestEntityOffset.Create();
        
        var now = TimeProvider.System.GetUtcNow();
        
        entry.UpdatedAt = now;
        
        ctx.Add(entry);
        
        await ctx.SaveChangesAsync();

        // Act
        entry.Name = "b";
        await ctx.SaveChangesAsync();
        
        // Assert
        entry.UpdatedAt.Should().NotBeNull().And.Be(futureNow);
    }
    
    [Fact]
    public async Task Leave_UpdatedAt_for_UpdatedAt_entries_untouched_when_manually_modified()
    {
        // Arrange
        var timeProvider = _fixture.GetTimeProviderMock();
        var futureNow = TimeProvider.System.GetUtcNow().Add(TimeSpan.FromMinutes(2));
        timeProvider.Setup(x => x.GetUtcNow()).Returns(futureNow);

        await using var ctx = _fixture.GetTextContext(timeProvider.Object);
        var entry = TestEntity.Create();
        
        var now = TimeProvider.System.GetUtcNow();
        
        entry.UpdatedAt = now.DateTime;
        
        ctx.Add(entry);
        
        await ctx.SaveChangesAsync();
        
        // Act
        var anotherNow = TimeProvider.System.GetUtcNow();
        entry.UpdatedAt = anotherNow.DateTime;
        await ctx.SaveChangesAsync();
        
        // Assert
        entry.UpdatedAt.Should().NotBeNull().And.Be(anotherNow.DateTime);
    }
    
    [Fact]
    public async Task Leave_UpdatedAt_for_UpdatedAtOffset_entries_untouched_when_manually_modified()
    {
        // Arrange
        var timeProvider = _fixture.GetTimeProviderMock();
        var futureNow = TimeProvider.System.GetUtcNow().Add(TimeSpan.FromMinutes(2));
        timeProvider.Setup(x => x.GetUtcNow()).Returns(futureNow);

        await using var ctx = _fixture.GetTextContext(timeProvider.Object);
        var entry = TestEntityOffset.Create();
        var now = TimeProvider.System.GetUtcNow();
        
        entry.UpdatedAt = now;
        
        ctx.Add(entry);
        
        await ctx.SaveChangesAsync();
        
        // Act
        var anotherNow = TimeProvider.System.GetUtcNow();
        entry.UpdatedAt = anotherNow;
        await ctx.SaveChangesAsync();
        
        // Assert
        entry.UpdatedAt.Should().NotBeNull().And.Be(anotherNow);
    }
    
    [Fact]
    public async Task Fill_CreatedAt_for_CreatedAt_entries_when_non_manually_filled()
    {
        // Arrange
        var timeProvider = _fixture.GetTimeProviderMock();
        var futureNow = TimeProvider.System.GetUtcNow().Add(TimeSpan.FromMinutes(2));
        timeProvider.Setup(x => x.GetUtcNow()).Returns(futureNow);

        await using var ctx = _fixture.GetTextContext(timeProvider.Object);
        var entry = TestEntity.Create();
        
        // Act
        ctx.Add(entry);
        await ctx.SaveChangesAsync();
        
        // Assert
        entry.CreatedAt.Should().NotBeNull().And.Be(futureNow.DateTime);
    }
    
    [Fact]
    public async Task Fill_CreatedAt_for_CreatedAtOffset_entries_when_non_manually_filled()
    {
        // Arrange
        var timeProvider = _fixture.GetTimeProviderMock();
        var futureNow = TimeProvider.System.GetUtcNow().Add(TimeSpan.FromMinutes(2));
        timeProvider.Setup(x => x.GetUtcNow()).Returns(futureNow);

        await using var ctx = _fixture.GetTextContext(timeProvider.Object);
        var entry = TestEntityOffset.Create();

        // Act
        ctx.Add(entry);
        await ctx.SaveChangesAsync();
        
        // Assert
        entry.CreatedAt.Should().NotBeNull().And.Be(futureNow);
    }
    
    [Fact]
    public async Task Fill_UpdatedAt_for_UpdatedAt_entries_when_created_non_manually_filled()
    {
        // Arrange
        var timeProvider = _fixture.GetTimeProviderMock();
        var futureNow = TimeProvider.System.GetUtcNow().Add(TimeSpan.FromMinutes(2));
        timeProvider.Setup(x => x.GetUtcNow()).Returns(futureNow);

        await using var ctx = _fixture.GetTextContext(timeProvider.Object);
        var entry = TestEntity.Create();
        
        // Act
        ctx.Add(entry);
        await ctx.SaveChangesAsync();
        
        // Assert
        entry.UpdatedAt.Should().NotBeNull().And.Be(futureNow.DateTime);
    }
    
    [Fact]
    public async Task Fill_UpdatedAt_for_UpdatedAtOffset_entries_when_created_non_manually_filled()
    {
        // Arrange
        var timeProvider = _fixture.GetTimeProviderMock();
        var futureNow = TimeProvider.System.GetUtcNow().Add(TimeSpan.FromMinutes(2));
        timeProvider.Setup(x => x.GetUtcNow()).Returns(futureNow);

        await using var ctx = _fixture.GetTextContext(timeProvider.Object);
        var entry = TestEntityOffset.Create();

        // Act
        ctx.Add(entry);
        await ctx.SaveChangesAsync();
        
        // Assert
        entry.UpdatedAt.Should().NotBeNull().And.Be(futureNow);
    }
    
    [Fact]
    public async Task Leave_UpdatedAt_for_UpdatedAt_entries_untouched_when_created_manually_filled()
    {
        // Arrange
        var timeProvider = _fixture.GetTimeProviderMock();
        var futureNow = TimeProvider.System.GetUtcNow().Add(TimeSpan.FromMinutes(2));
        timeProvider.Setup(x => x.GetUtcNow()).Returns(futureNow);

        await using var ctx = _fixture.GetTextContext(timeProvider.Object);
        var anotherNow = TimeProvider.System.GetUtcNow();
        var entry = TestEntity.Create();
        entry.UpdatedAt = anotherNow.DateTime;
        
        // Act
        ctx.Add(entry);
        await ctx.SaveChangesAsync();
        
        // Assert
        entry.UpdatedAt.Should().NotBeNull().And.Be(anotherNow.DateTime);
    }
    
    [Fact]
    public async Task Leave_UpdatedAt_for_UpdatedAtOffset_entries_untouched_when_created_manually_filled()
    {
        // Arrange
        var timeProvider = _fixture.GetTimeProviderMock();
        var futureNow = TimeProvider.System.GetUtcNow().Add(TimeSpan.FromMinutes(2));
        timeProvider.Setup(x => x.GetUtcNow()).Returns(futureNow);

        await using var ctx = _fixture.GetTextContext(timeProvider.Object);
        var anotherNow = TimeProvider.System.GetUtcNow();
        var entry = TestEntityOffset.Create();
        entry.UpdatedAt = anotherNow;
        
        // Act
        ctx.Add(entry);
        await ctx.SaveChangesAsync();
        
        // Assert
        entry.UpdatedAt.Should().NotBeNull().And.Be(anotherNow);
    }
    
    [Fact]
    public async Task Leave_CreatedAt_for_CreatedAt_entries_untouched_when_manually_filled()
    {
        // Arrange
        var timeProvider = _fixture.GetTimeProviderMock();
        var futureNow = TimeProvider.System.GetUtcNow().Add(TimeSpan.FromMinutes(2));
        timeProvider.Setup(x => x.GetUtcNow()).Returns(futureNow);

        await using var ctx = _fixture.GetTextContext(timeProvider.Object);
        var anotherNow = TimeProvider.System.GetUtcNow();
        var entry = TestEntity.Create();
        entry.CreatedAt = anotherNow.DateTime;
        
        // Act
        ctx.Add(entry);
        await ctx.SaveChangesAsync();
        
        // Assert
        entry.CreatedAt.Should().NotBeNull().And.Be(anotherNow.DateTime);
    }
    
    [Fact]
    public async Task Leave_CreatedAt_for_CreatedAtOffset_entries_untouched_when_manually_filled()
    {
        // Arrange
        var timeProvider = _fixture.GetTimeProviderMock();
        var futureNow = TimeProvider.System.GetUtcNow().Add(TimeSpan.FromMinutes(2));
        timeProvider.Setup(x => x.GetUtcNow()).Returns(futureNow);

        await using var ctx = _fixture.GetTextContext(timeProvider.Object);
        var anotherNow = TimeProvider.System.GetUtcNow();
        var entry = TestEntityOffset.Create();
        entry.CreatedAt = anotherNow;

        // Act
        ctx.Add(entry);
        await ctx.SaveChangesAsync();
        
        // Assert
        entry.CreatedAt.Should().NotBeNull().And.Be(anotherNow);
    }
    
    [Theory]
    [InlineData(DateTimeStrategy.Now)]
    [InlineData(DateTimeStrategy.UtcNow)]
    public async Task Use_correct_DateTimeStrategy(DateTimeStrategy dateTimeStrategy)
    {
        // Arrange
        var timeProvider = _fixture.GetTimeProviderMock();
        var futureNow = TimeProvider.System.GetUtcNow().Add(TimeSpan.FromMinutes(2));
        timeProvider.Setup(x => x.GetUtcNow()).Returns(futureNow);
        timeProvider.SetupGet(x => x.LocalTimeZone).Returns(TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time"));

        var cfg = new DataExplorerEfCoreConfiguration(new ServiceCollection())
        {
            DateTimeStrategy = dateTimeStrategy
        };

        await using var ctx = _fixture.GetTextContext(cfg,timeProvider.Object);
        
        var entry = TestEntityOffset.Create();

        // Act
        ctx.Add(entry);
        await ctx.SaveChangesAsync();
        
        // Assert
        entry.CreatedAt.Should().NotBeNull().And.Be(futureNow);
    }
}
