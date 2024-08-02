using DataExplorer.Abstractions.Entities;
using DataExplorer.Entities;

namespace DataExplorer.Tests.Shared;

public class TestEntity : Entity, ICreatedAt, IUpdatedAt
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public int Version { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public static TestEntity Create()
    {
        var ent = new TestEntity()
        {
            Name = "a"
        };
        
        ent.SetId(Random.Shared.NextInt64());
        
        return ent;
    }
}

public class TestEntityOffset : Entity, ICreatedAtOffset, IUpdatedAtOffset
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public int Version { get; set; }
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    
    public static TestEntityOffset Create()
    {
        var ent = new TestEntityOffset()
        {
            Name = "a"
        };
        ent.SetId(Random.Shared.NextInt64());
        return ent;
    }
}

