using DataExplorer.Entities;
using DataExplorer.IdGenerator;

namespace DataExplorer.Tests.Unit;

[Collection("FactoryInvolved")]
public class SnowflakeEntityShould
{
    [Fact]
    public void Get_its_Id_auto_assigned_upon_creation()
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

    
    private class TestEntity1 : SnowflakeEntity
    {
    }
}
