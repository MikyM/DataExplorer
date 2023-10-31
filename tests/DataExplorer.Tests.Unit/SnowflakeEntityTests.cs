using DataExplorer.Entities;
using DataExplorer.IdGenerator;

namespace DataExplorer.Tests.Unit;

[Collection("FactoryInvolved")]
public class SnowflakeEntityTests
{
    public class UponConstruction
    {
        [Fact]
        public void ShouldGetNewIdAssigned()
        {
            // Arrange
            var id = Random.Shared.NextInt64();
            SnowflakeIdFactory.AddFactoryMethod(() => id);
        
            // Act
            var entity = new TestEntity1();
        
            // Assert
            entity.HasValidId.Should().BeTrue();
            entity.Id.Should().BePositive();
            entity.Id.Should().Be(id);
        
            SnowflakeIdFactory.RemoveFactoryMethod();
        }
    }
    
    private class TestEntity1 : SnowflakeEntity
    {
    }
}
