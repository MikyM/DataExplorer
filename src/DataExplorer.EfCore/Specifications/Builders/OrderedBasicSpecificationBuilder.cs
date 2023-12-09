using DataExplorer.Abstractions.Specifications;
using DataExplorer.Abstractions.Specifications.Builders;

namespace DataExplorer.EfCore.Specifications.Builders;

public class OrderedBasicSpecificationBuilder<T> : IOrderedBasicSpecificationBuilder<T> where T : class
{
    public IBasicSpecification<T> Specification { get; }
    public bool IsChainDiscarded { get; set; }

    public OrderedBasicSpecificationBuilder(IBasicSpecification<T> specification, bool isChainDiscarded = false)
    {
        Specification = specification;
        IsChainDiscarded = isChainDiscarded;
    }
    
    IBasicSpecification<T> IBasicSpecificationBuilder<T>.Specification => Specification;
}
