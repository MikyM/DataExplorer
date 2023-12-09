using DataExplorer.Abstractions.Specifications;
using DataExplorer.Abstractions.Specifications.Builders;

namespace DataExplorer.EfCore.Specifications.Builders;

public class OrderedSpecificationBuilder<T> : IOrderedSpecificationBuilder<T> where T : class
{
    public ISpecification<T> Specification { get; }
    public bool IsChainDiscarded { get; set; }

    public OrderedSpecificationBuilder(ISpecification<T> specification, bool isChainDiscarded = false)
    {
        Specification = specification;
        IsChainDiscarded = isChainDiscarded;
    }
    
    IBasicSpecification<T> IBasicSpecificationBuilder<T>.Specification => Specification;
}
