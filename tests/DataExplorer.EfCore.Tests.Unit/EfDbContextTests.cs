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
        
        private static DateTime GetDateTime(DateTimeStrategy strategy, DateTimeOffset offset)
        {
            return strategy switch
            {
                DateTimeStrategy.Now => offset.DateTime.ToLocalTime(),
                DateTimeStrategy.UtcNow => offset.DateTime.ToUniversalTime(),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
        
        private static DateTimeOffset GetDateTimeOffset(DateTimeStrategy strategy)
        {
            return strategy switch
            {
                DateTimeStrategy.Now => DateTimeOffset.Now,
                DateTimeStrategy.UtcNow => DateTimeOffset.UtcNow,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
        
        private static DateTime GetDateTime(DateTimeStrategy strategy)
        {
            return strategy switch
            {
                DateTimeStrategy.Now => DateTimeOffset.Now.DateTime.ToLocalTime(),
                DateTimeStrategy.UtcNow => DateTimeOffset.UtcNow.DateTime.ToUniversalTime(),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        [Theory]
        [InlineData(DateTimeStrategy.Now)]
        [InlineData(DateTimeStrategy.UtcNow)]
        public async Task ShouldFillUpdatedAtForUpdatedAtUpdatedEntriesWhenNonModified(DateTimeStrategy strategy)
        {
            // Arrange
            var dataExplorerTimeProvider = _fixture.GetTimeProviderMock();

            var futureNow = SetupTimeProvider(dataExplorerTimeProvider, strategy);

            await using var ctx = _fixture.GetTestContext(strategy, dataExplorerTimeProvider.Object);
            
            var entry = TestEntity.Create();

            var now = GetDateTime(strategy);

            entry.UpdatedAt = now;

            ctx.Add(entry);

            await ctx.SaveChangesAsync();

            // Act
            entry.Name = "b";
            await ctx.SaveChangesAsync();

            // Assert
            entry.UpdatedAt.Should().NotBeNull().And.Be(GetDateTime(strategy, futureNow));
        }

        [Theory]
        [InlineData(DateTimeStrategy.Now)]
        [InlineData(DateTimeStrategy.UtcNow)]
        public async Task ShouldFillUpdatedAtForUpdatedAtOffsetWhenUpdatedEntriesWhenNonModified(DateTimeStrategy strategy)
        {
            // Arrange
            var dataExplorerTimeProvider = _fixture.GetTimeProviderMock();
            
            var futureNow = SetupTimeProvider(dataExplorerTimeProvider, strategy);

            await using var ctx = _fixture.GetTestContext(strategy, dataExplorerTimeProvider.Object);
            var entry = TestEntityOffset.Create();

            var now = GetDateTimeOffset(strategy);

            entry.UpdatedAt = now;

            ctx.Add(entry);

            await ctx.SaveChangesAsync();

            // Act
            entry.Name = "b";
            await ctx.SaveChangesAsync();

            // Assert
            entry.UpdatedAt.Should().NotBeNull().And.Be(futureNow);
        }

        [Theory]
        [InlineData(DateTimeStrategy.Now)]
        [InlineData(DateTimeStrategy.UtcNow)]
        public async Task ShouldLeaveUpdatedAtForUpdatedAtEntriesUntouchedWhenManuallyModified(DateTimeStrategy strategy)
        {
            // Arrange
            var dataExplorerTimeProvider = _fixture.GetTimeProviderMock();
            
            var futureNow = SetupTimeProvider(dataExplorerTimeProvider, strategy);

            await using var ctx = _fixture.GetTestContext(strategy, dataExplorerTimeProvider.Object);
            var entry = TestEntity.Create();

            var now = GetDateTimeOffset(strategy);

            entry.UpdatedAt = GetDateTime(strategy, now);

            ctx.Add(entry);

            await ctx.SaveChangesAsync();

            // Act
            var anotherNow = GetDateTimeOffset(strategy);
            
            entry.UpdatedAt =GetDateTime(strategy, anotherNow);
            
            await ctx.SaveChangesAsync();

            // Assert
            entry.UpdatedAt.Should().NotBeNull().And.Be(GetDateTime(strategy, anotherNow));
        }

        [Theory]
        [InlineData(DateTimeStrategy.Now)]
        [InlineData(DateTimeStrategy.UtcNow)]
        public async Task ShouldLeaveUpdatedAtForUpdatedAtOffsetEntriesUntouchedWhenManuallyModified(DateTimeStrategy strategy)
        {
            // Arrange
            var dataExplorerTimeProvider = _fixture.GetTimeProviderMock();
            
            var futureNow = SetupTimeProvider(dataExplorerTimeProvider, strategy);

            await using var ctx = _fixture.GetTestContext(strategy, dataExplorerTimeProvider.Object);
            var entry = TestEntityOffset.Create();

            var now = GetDateTimeOffset(strategy);

            entry.UpdatedAt = now;

            ctx.Add(entry);

            await ctx.SaveChangesAsync();

            // Act
            var anotherNow = GetDateTimeOffset(strategy);
            
            entry.UpdatedAt = anotherNow;
            
            await ctx.SaveChangesAsync();

            // Assert
            entry.UpdatedAt.Should().NotBeNull().And.Be(anotherNow);
        }

        private static DateTimeOffset SetupTimeProvider(Mock<DataExplorerTimeProvider> mock, DateTimeStrategy strategy)
        {
            switch (strategy)
            {
                case DateTimeStrategy.Now:
                    
                    var now = DateTimeOffset.Now.Add(TimeSpan.FromMinutes(2)).ToLocalTime();
                    
                    mock.Setup(x => x.GetLocalNow()).Returns(now);

                    return now;
                
                case DateTimeStrategy.UtcNow:
                    
                    var utcNow = DateTimeOffset.UtcNow.Add(TimeSpan.FromMinutes(2)).ToUniversalTime();
                    
                    mock.Setup(x => x.GetUtcNow()).Returns(utcNow);

                    return utcNow;
                default:
                    throw new ArgumentOutOfRangeException();
                
            }
        }

        [Theory]
        [InlineData(DateTimeStrategy.Now)]
        [InlineData(DateTimeStrategy.UtcNow)]
        public async Task ShouldFillCreatedAtForCreatedAtEntriesWhenNonManuallyFilled(DateTimeStrategy strategy)
        {
            // Arrange
            var dataExplorerTimeProvider = _fixture.GetTimeProviderMock();
            
            var futureNow = SetupTimeProvider(dataExplorerTimeProvider, strategy);

            await using var ctx = _fixture.GetTestContext(strategy, dataExplorerTimeProvider.Object);
            
            var entry = TestEntity.Create();

            // Act
            ctx.Add(entry);
            await ctx.SaveChangesAsync();

            // Assert
            
            entry.CreatedAt.Should().NotBeNull().And.Be(GetDateTime(strategy,futureNow));
        }

        [Theory]
        [InlineData(DateTimeStrategy.Now)]
        [InlineData(DateTimeStrategy.UtcNow)]
        public async Task ShouldFillCreatedAtForCreatedAtOffsetEntriesWhenNonManuallyFilled(DateTimeStrategy strategy)
        {
            // Arrange
            var dataExplorerTimeProvider = _fixture.GetTimeProviderMock();
            
            var futureNow = SetupTimeProvider(dataExplorerTimeProvider, strategy);

            await using var ctx = _fixture.GetTestContext(strategy, dataExplorerTimeProvider.Object);
            var entry = TestEntityOffset.Create();

            // Act
            ctx.Add(entry);
            await ctx.SaveChangesAsync();

            // Assert
            entry.CreatedAt.Should().NotBeNull().And.Be(futureNow);
        }

        [Theory]
        [InlineData(DateTimeStrategy.Now)]
        [InlineData(DateTimeStrategy.UtcNow)]
        public async Task ShouldFillUpdatedAtForUpdatedAtEntriesWhenCreatedNonManuallyFilled(DateTimeStrategy strategy)
        {
            // Arrange
            var dataExplorerTimeProvider = _fixture.GetTimeProviderMock();
            
            var futureNow = SetupTimeProvider(dataExplorerTimeProvider, strategy);

            await using var ctx = _fixture.GetTestContext(strategy, dataExplorerTimeProvider.Object);
            
            var entry = TestEntity.Create();

            // Act
            ctx.Add(entry);
            await ctx.SaveChangesAsync();

            // Assert
            entry.UpdatedAt.Should().NotBeNull().And.Be(GetDateTime(strategy, futureNow));
        }

        [Theory]
        [InlineData(DateTimeStrategy.Now)]
        [InlineData(DateTimeStrategy.UtcNow)]
        public async Task ShouldFillUpdatedAtForUpdatedAtOffsetEntriesWhenCreatedNonManuallyFilled(DateTimeStrategy strategy)
        {
            // Arrange
            var dataExplorerTimeProvider = _fixture.GetTimeProviderMock();
            
            var futureNow = SetupTimeProvider(dataExplorerTimeProvider, strategy);

            await using var ctx = _fixture.GetTestContext(strategy, dataExplorerTimeProvider.Object);
            var entry = TestEntityOffset.Create();

            // Act
            ctx.Add(entry);
            await ctx.SaveChangesAsync();

            // Assert
            entry.UpdatedAt.Should().NotBeNull().And.Be(futureNow);
        }

        [Theory]
        [InlineData(DateTimeStrategy.Now)]
        [InlineData(DateTimeStrategy.UtcNow)]
        public async Task ShouldLeaveUpdatedAtForUpdatedAtEntriesUntouchedWhenCreatedManuallyFilled(DateTimeStrategy strategy)
        {
            // Arrange
            var dataExplorerTimeProvider = _fixture.GetTimeProviderMock();
            
            var futureNow = SetupTimeProvider(dataExplorerTimeProvider, strategy);

            await using var ctx = _fixture.GetTestContext(strategy, dataExplorerTimeProvider.Object);
            
            var anotherNow = GetDateTimeOffset(strategy);
            
            var entry = TestEntity.Create();
            
            entry.UpdatedAt = GetDateTime(strategy, anotherNow);

            // Act
            ctx.Add(entry);
            await ctx.SaveChangesAsync();

            // Assert
            entry.UpdatedAt.Should().NotBeNull().And.Be(GetDateTime(strategy, anotherNow));
        }

        [Theory]
        [InlineData(DateTimeStrategy.Now)]
        [InlineData(DateTimeStrategy.UtcNow)]
        public async Task ShouldLeaveUpdatedAtForUpdatedAtOffsetEntriesUntouchedWhenCreatedManuallyFilled(DateTimeStrategy strategy)
        {
            // Arrange
            var dataExplorerTimeProvider = _fixture.GetTimeProviderMock();
            
            var futureNow = SetupTimeProvider(dataExplorerTimeProvider, strategy);

            await using var ctx = _fixture.GetTestContext(strategy, dataExplorerTimeProvider.Object);

            var anotherNow = GetDateTimeOffset(strategy);
            
            var entry = TestEntityOffset.Create();
            
            entry.UpdatedAt = anotherNow;

            // Act
            ctx.Add(entry);
            await ctx.SaveChangesAsync();

            // Assert
            entry.UpdatedAt.Should().NotBeNull().And.Be(anotherNow);
        }

        [Theory]
        [InlineData(DateTimeStrategy.Now)]
        [InlineData(DateTimeStrategy.UtcNow)]
        public async Task ShouldLeaveCreatedAtForCreatedAtEntriesUntouchedWhenManuallyFilled(DateTimeStrategy strategy)
        {
            // Arrange
            var dataExplorerTimeProvider = _fixture.GetTimeProviderMock();
            
            var futureNow = SetupTimeProvider(dataExplorerTimeProvider, strategy);

            await using var ctx = _fixture.GetTestContext(strategy, dataExplorerTimeProvider.Object);
            
            var anotherNow = GetDateTimeOffset(strategy);
            
            var entry = TestEntity.Create();
            
            entry.CreatedAt = anotherNow.DateTime;

            // Act
            ctx.Add(entry);
            await ctx.SaveChangesAsync();

            // Assert
            entry.CreatedAt.Should().NotBeNull().And.Be(anotherNow.DateTime);
        }

        [Theory]
        [InlineData(DateTimeStrategy.Now)]
        [InlineData(DateTimeStrategy.UtcNow)]
        public async Task ShouldLeaveCreatedAtForCreatedAtOffsetEntriesUntouchedWhenManuallyFilled(DateTimeStrategy strategy)
        {
            // Arrange
            var dataExplorerTimeProvider = _fixture.GetTimeProviderMock();
            
            var futureNow = SetupTimeProvider(dataExplorerTimeProvider, strategy);

            await using var ctx = _fixture.GetTestContext(strategy, dataExplorerTimeProvider.Object);
            
            var anotherNow = GetDateTimeOffset(strategy);
            
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
            
            DateTimeOffset futureNow;

            if (dateTimeStrategy == DateTimeStrategy.UtcNow)
            {
                futureNow = DateTimeOffset.UtcNow.Add(TimeSpan.FromMinutes(Random.Shared.Next(1,10000)));
                timeProvider.Setup(x => x.GetUtcNow()).Returns(futureNow);
            }
            else
            {
                futureNow = DateTimeOffset.Now.Add(TimeSpan.FromMinutes(Random.Shared.Next(1,10000)));
                timeProvider.Setup(x => x.GetLocalNow()).Returns(futureNow);
            }

            var cfg = new DataExplorerEfCoreConfiguration(new MicrosoftRegistrator(new ServiceCollection()))
            {
                DateTimeStrategy = dateTimeStrategy
            };

            await using var ctx = _fixture.GetTestContext(cfg, timeProvider.Object);

            var entry = TestEntityOffset.Create();

            // Act
            ctx.Add(entry);
            await ctx.SaveChangesAsync();

            // Assert
            entry.CreatedAt.Should().NotBeNull().And.Be(futureNow);
        }
    }
}
