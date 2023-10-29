using DataExplorer.Services;

namespace DataExplorer.Tests.Unit;

public class SnowflakeGeneratorShould
{
    [Fact]
    public void Properly_create_a_snowflake()
    {
        // Arrange
        var generator = new SnowflakeGenerator(new IdGen.IdGenerator(1));
        
        // Act
        var result = generator.Generate();
        
        // Assert
        result.Should().BePositive();
    }
}
