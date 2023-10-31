using DataExplorer.Entities;

namespace DataExplorer.Tests.Unit;

public class EntityTests
{
    public class SetId
    {
        [Fact]
        public void ShouldSetIdCorrectly()
        {
            // Arrange
            var entity = new TestEntity1();
        
            // Act
            entity.SetId(1213);
        
            // Assert
            entity.Id.Should().Be(1213);
        }
    }

    public class HasValidId
    {
        [Fact]
        public void ShouldBeFalseWhenDefault()
        {
            // Arrange
            var entity = new TestEntity1();
        
            // Act
            var res = entity.HasValidId;
        
            // Assert
            res.Should().BeFalse();
        }
        
        [Fact]
        public void ShouldBeTrueWhenNonDefault()
        {
            // Arrange
            var entity = new TestEntity1();
            entity.SetId(51);
        
            // Act
            var res = entity.HasValidId;
        
            // Assert
            res.Should().BeTrue();
        }
    }

    public class Equality
    {
        [Fact]
        public void ShouldReturnFalseWhenSameTypeAndDifferentId()
        {
            // Arrange
            var entity1 = new TestEntity1();
            entity1.SetId(51);

            var entity2 = new TestEntity1();
            entity2.SetId(15);

            // Act
            var equals1 = entity1.Equals(entity2);
            var equals2 = entity1 == entity2;

            // Assert
            equals1.Should().BeFalse();
            equals2.Should().BeFalse();
        }

        [Fact]
        public void ShouldReturnTrueWhenSameTypeAndSameId()
        {
            // Arrange
            var entity1 = new TestEntity1();
            entity1.SetId(51);

            var entity2 = new TestEntity1();
            entity2.SetId(51);

            // Act
            var equals1 = entity1.Equals(entity2);
            var equals2 = entity1 == entity2;

            // Assert
            equals1.Should().BeTrue();
            equals2.Should().BeTrue();
        }

        [Fact]
        public void ShouldReturnFalseWhenDifferentTypeAndSameId()
        {
            // Arrange
            var entity1 = new TestEntity1();
            entity1.SetId(51);

            var entity2 = new TestEntity2();
            entity2.SetId(51);

            // Act
            var equals1 = entity1.Equals(entity2);
            var equals2 = entity1 == entity2;

            // Assert
            equals1.Should().BeFalse();
            equals2.Should().BeFalse();
        }
    }

    private class TestEntity1 : Entity
    {
    }
    
    private class TestEntity2 : Entity
    {
    }
}
