using DataExplorer.EfCore.Specifications;

namespace DataExplorer.Tests.Shared;

public class TestSpecification : Specification<TestEntity>
{
    public TestSpecification(long id)
    {
        Where(x => x.Id == id);
    }
}

public class TestSpecificationTransform : Specification<TestEntity, TestEntityOffset>
{
    public TestSpecificationTransform(long id)
    {
        Where(x => x.Id == id);
        Select(x => new TestEntityOffset
        {
            Name = x.Name,
            Description = x.Description,
            Version = 0,
            CreatedAt = x.CreatedAt,
            UpdatedAt = x.UpdatedAt
        });
    }
}

#if NET7_0_OR_GREATER
public sealed class TestUpdateSpecification : UpdateSpecification<TestEntity>
{
    public TestUpdateSpecification(long id)
    {
        Where(x => x.Id == id);
        Modify(x => x.SetProperty(y => y.Name, "Test"));
    }
}
#endif
