using System.Reflection;
using DataExplorer.IdGenerator;

namespace DataExplorer.Tests.Unit;

[Collection("FactoryInvolved")]
public class SnowflakeIdFactoryTests
{
    public class CreateId
    {
        [Fact]
        public void ShouldThrowWhenNoFactoriesRegistered()
        {
            // Arrange
            SnowflakeIdFactory.RemoveFactoryMethod();
        
            // Act
            var func = () => SnowflakeIdFactory.CreateId();
        
            // Assert
            func.Should().ThrowExactly<InvalidOperationException>();
        }
        
        [Fact]
        public void ShouldThrowWhenNoFactoriesRegisteredFactoryIdPassed()
        {
            // Arrange
            SnowflakeIdFactory.RemoveFactoryMethod();

            // Act
            var func = () => SnowflakeIdFactory.CreateId(5);
        
            // Assert
            func.Should().ThrowExactly<InvalidOperationException>();
        }
        
        [Fact]
        public void ShouldCreateNewIdDefaultFactory()
        {
            // Arrange
            var id = Random.Shared.NextInt64();
            SnowflakeIdFactory.AddFactoryMethod(() => id);
        
            // Act
            var result = SnowflakeIdFactory.CreateId();
        
            // Assert
            result.Should().Be(id);

            SnowflakeIdFactory.RemoveFactoryMethod();
        }
    
        [Fact]
        public void ShouldCreateNewIdNonDefaultFactory()
        {
            // Arrange
            var id = Random.Shared.NextInt64();
            SnowflakeIdFactory.AddFactoryMethod(() => id, 5);
        
            // Act
            var result = SnowflakeIdFactory.CreateId(5);
        
            // Assert
            result.Should().Be(id);

            SnowflakeIdFactory.RemoveFactoryMethod();
        }
    }

    public class AddFactoryMethod
    {
        [Fact]
        public void ShouldProperlyAddNewFactory()
        {
            // Arrange
            // Act
            SnowflakeIdFactory.AddFactoryMethod(() => 1);
        
            // Assert
            var field = typeof(SnowflakeIdFactory).GetField("_factories", BindingFlags.Static | BindingFlags.NonPublic);
            var value = field!.GetValue(null) as Dictionary<int, Func<long>>;
            value.Should().NotBeNull();
            value.Should().ContainKey(1);

            SnowflakeIdFactory.RemoveFactoryMethod();
        }
    }
}
