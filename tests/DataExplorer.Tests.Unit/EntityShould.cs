using DataExplorer.Entities;

namespace DataExplorer.Tests.Unit;

public class EntityShould
{
    [Fact]
    public void Have_SetId_work_correctly()
    {
        // Arrange
        var entity = new TestEntity1();
        
        // Act
        entity.SetId(1213);
        
        // Assert
        entity.Id.Should().Be(1213);
    }
    
    [Fact]
    public void Return_HasValidId_false_when_default()
    {
        // Arrange
        var entity = new TestEntity1();
        
        // Act
        var res = entity.HasValidId;
        
        // Assert
        res.Should().BeFalse();
    }
    
    [Fact]
    public void Return_HasValidId_true_when_non_default()
    {
        // Arrange
        var entity = new TestEntity1();
        entity.SetId(51);
        
        // Act
        var res = entity.HasValidId;
        
        // Assert
        res.Should().BeTrue();
    }
    
    [Fact]
    public void Correctly_compare_to_another_false_same_type()
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
    public void Correctly_compare_to_another_true_same_type()
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
    public void Correctly_compare_to_another_false_diff_type()
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

    private class TestEntity1 : Entity
    {
    }
    
    private class TestEntity2 : Entity
    {
    }
}
