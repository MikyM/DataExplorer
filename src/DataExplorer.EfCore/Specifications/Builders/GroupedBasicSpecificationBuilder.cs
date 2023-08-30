namespace DataExplorer.EfCore.Specifications.Builders;

public class GroupedBasicSpecificationBuilder<T> : IGroupedBasicSpecificationBuilder<T> where T : class
{
    public BasicSpecification<T> Specification { get; }
    public bool IsChainDiscarded { get; set; }

    public GroupedBasicSpecificationBuilder(BasicSpecification<T> specification, bool isChainDiscarded = false)
    {
        Specification = specification;
        IsChainDiscarded = isChainDiscarded;
    }
    
    BasicSpecification<T> IBasicSpecificationBuilder<T>.Specification => Specification;
}
