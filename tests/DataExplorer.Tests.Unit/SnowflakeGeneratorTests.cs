using DataExplorer.Services;

namespace DataExplorer.Tests.Unit;

public class SnowflakeGeneratorTests
{
    public class Generate
    {
        [Fact]
        public void ShouldProperlyCreateASnowflake()
        {
            // Arrange
            var generator = new SnowflakeGenerator(new IdGen.IdGenerator(1));
        
            // Act
            var result = generator.Generate();
        
            // Assert
            result.Should().BePositive();
        }
    }
}
