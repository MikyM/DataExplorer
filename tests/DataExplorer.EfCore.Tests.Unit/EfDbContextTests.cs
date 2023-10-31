using DataExplorer.Tests.Shared;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Moq.Protected;

namespace DataExplorer.EfCore.Tests.Unit;

public class EfDbContextTests : IClassFixture<ContextFixture>
{
    private readonly ContextFixture _fixture;

    public EfDbContextTests(ContextFixture fixture)
    {
        _fixture = fixture;
    }

    public class OnBeforeSaveChangesAsync : IClassFixture<ContextFixture>
    {
        private readonly ContextFixture _fixture;

        public OnBeforeSaveChangesAsync(ContextFixture fixture)
        {
            _fixture = fixture;
            
        }
        [Fact]
        public async Task ShouldBeCalledOnSaveChangesAsync()
        {
            // Arrange
            var ctx = _fixture.GetTextContextMock();

            // Act
            await ctx.Object.SaveChangesAsync(CancellationToken.None);
        
            // Assert
            ctx.Protected().Verify("OnBeforeSaveChangesAsync", Times.Once(), ItExpr.IsAny<IReadOnlyList<EntityEntry>?>());
        }

        [Fact]
        public async Task ShouldBeCalledOnSaveChangesAsync2Arg()
        {
            // Arrange
            var ctx = _fixture.GetTextContextMock();

            // Act
            await ctx.Object.SaveChangesAsync(true, CancellationToken.None);

            // Assert
            ctx.Protected().Verify("OnBeforeSaveChangesAsync", Times.Once(),
                ItExpr.IsAny<IReadOnlyList<EntityEntry>?>());
        }

        [Fact]
        public async Task ShouldCallGetTrackedEntries()
        {
            // Arrange
            var ctx = _fixture.GetTextContextMock();

            // Act
            await ctx.Object.SaveChangesAsync(CancellationToken.None);

            // Assert
            ctx.Protected().Verify("GetTrackedEntries", Times.Once());
        }

        [Fact]
        public async Task ShouldFillUpdatedAtForUpdatedAtUpdatedEntriesWhenNonModified()
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
        public async Task ShouldFillUpdatedAtForUpdatedAtOffsetWhenUpdatedEntriesWhenNonModified()
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
        public async Task ShouldLeaveUpdatedAtForUpdatedAtEntriesUntouchedWhenManuallyModified()
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
        public async Task ShouldLeaveUpdatedAtForUpdatedAtOffsetEntriesUntouchedWhenManuallyModified()
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
        public async Task ShouldFillCreatedAtForCreatedAtEntriesWhenNonManuallyFilled()
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
        public async Task ShouldFillCreatedAtForCreatedAtOffsetEntriesWhenNonManuallyFilled()
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
        public async Task ShouldFillUpdatedAtForUpdatedAtEntriesWhenCreatedNonManuallyFilled()
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
        public async Task ShouldFillUpdatedAtForUpdatedAtOffsetEntriesWhenCreatedNonManuallyFilled()
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
        public async Task ShouldLeaveUpdatedAtForUpdatedAtEntriesUntouchedWhenCreatedManuallyFilled()
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
        public async Task ShouldLeaveUpdatedAtForUpdatedAtOffsetEntriesUntouchedWhenCreatedManuallyFilled()
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
        public async Task ShouldLeaveCreatedAtForCreatedAtEntriesUntouchedWhenManuallyFilled()
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
        public async Task ShouldLeaveCreatedAtForCreatedAtOffsetEntriesUntouchedWhenManuallyFilled()
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
        public async Task ShouldUseCorrectDateTimeStrategy(DateTimeStrategy dateTimeStrategy)
        {
            // Arrange
            var timeProvider = _fixture.GetTimeProviderMock();
            var futureNow = TimeProvider.System.GetUtcNow().Add(TimeSpan.FromMinutes(2));
            timeProvider.Setup(x => x.GetUtcNow()).Returns(futureNow);
            timeProvider.SetupGet(x => x.LocalTimeZone)
                .Returns(TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time"));

            var cfg = new DataExplorerEfCoreConfiguration(new ServiceCollection())
            {
                DateTimeStrategy = dateTimeStrategy
            };

            await using var ctx = _fixture.GetTextContext(cfg, timeProvider.Object);

            var entry = TestEntityOffset.Create();

            // Act
            ctx.Add(entry);
            await ctx.SaveChangesAsync();

            // Assert
            entry.CreatedAt.Should().NotBeNull().And.Be(futureNow);
        }
    }
}
