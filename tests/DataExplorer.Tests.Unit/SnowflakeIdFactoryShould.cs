using System.Reflection;
using DataExplorer.IdGenerator;

namespace DataExplorer.Tests.Unit;

[Collection("FactoryInvolved")]
public class SnowflakeIdFactoryShould
{
    [Fact]
    public void Throw_when_no_factories_registered()
    {
        // Arrange
        SnowflakeIdFactory.RemoveFactoryMethod(1);
        
        // Act
        var func = () => SnowflakeIdFactory.CreateId();
        
        // Assert
        func.Should().ThrowExactly<InvalidOperationException>();
    }
    
    [Fact]
    public void Throw_when_no_factories_registered_factory_id_passed()
    {
        // Arrange
        SnowflakeIdFactory.RemoveFactoryMethod(1);

        // Act
        var func = () => SnowflakeIdFactory.CreateId(1);
        
        // Assert
        func.Should().ThrowExactly<InvalidOperationException>();
    }
    
    [Fact]
    public void Properly_add_new_factory()
    {
        // Arrange
        // Act
        SnowflakeIdFactory.AddFactoryMethod(() => 1);
        
        // Assert
        var field = typeof(SnowflakeIdFactory).GetField("_factories", BindingFlags.Static | BindingFlags.NonPublic);
        var value = field!.GetValue(null) as Dictionary<int, Func<long>>;
        value.Should().NotBeNull();
        value.Should().ContainKey(1);

        SnowflakeIdFactory.RemoveFactoryMethod(1);
    }
    
    [Fact]
    public void Properly_create_new_id_default()
    {
        // Arrange
        var id = Random.Shared.NextInt64();
        SnowflakeIdFactory.AddFactoryMethod(() => id);
        
        // Act
        var result = SnowflakeIdFactory.CreateId();
        
        // Assert
        result.Should().Be(id);

        SnowflakeIdFactory.RemoveFactoryMethod(1);
    }
    
    [Fact]
    public void Properly_create_new_id_non_default()
    {
        // Arrange
        var id = Random.Shared.NextInt64();
        SnowflakeIdFactory.AddFactoryMethod(() => id, 5);
        
        // Act
        var result = SnowflakeIdFactory.CreateId(5);
        
        // Assert
        result.Should().Be(id);

        SnowflakeIdFactory.RemoveFactoryMethod(1);
    }
}
