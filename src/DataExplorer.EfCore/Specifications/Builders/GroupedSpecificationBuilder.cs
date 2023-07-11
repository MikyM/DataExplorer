namespace DataExplorer.EfCore.Specifications.Builders;

public class GroupedSpecificationBuilder<T> : IGroupedSpecificationBuilder<T> where T : class
{
    public BasicSpecification<T> Specification { get; }
    public bool IsChainDiscarded { get; set; }

    public GroupedSpecificationBuilder(BasicSpecification<T> specification, bool isChainDiscarded = false)
    {
        Specification = specification;
        IsChainDiscarded = isChainDiscarded;
    }
    
    BasicSpecification<T> IBasicSpecificationBuilder<T>.Specification => Specification;
}
