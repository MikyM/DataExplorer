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
        Select(x => new TestEntityOffset());
    }
}
