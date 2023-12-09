using DataExplorer.Abstractions.Specifications;
using DataExplorer.Abstractions.Specifications.Builders;

namespace DataExplorer.EfCore.Specifications.Builders;

public class GroupedSpecificationBuilder<T> : IGroupedSpecificationBuilder<T> where T : class
{
    public ISpecification<T> Specification { get; }
    public bool IsChainDiscarded { get; set; }

    public GroupedSpecificationBuilder(ISpecification<T> specification, bool isChainDiscarded = false)
    {
        Specification = specification;
        IsChainDiscarded = isChainDiscarded;
    }
    
    IBasicSpecification<T> IBasicSpecificationBuilder<T>.Specification => Specification;
}
