using DataExplorer.Abstractions.Specifications;
using DataExplorer.Abstractions.Specifications.Builders;

namespace DataExplorer.EfCore.Specifications.Builders;

[PublicAPI]
public class IncludableSpecificationBuilder<T, TProperty> : IIncludableSpecificationBuilder<T, TProperty> where T : class
{
    public ISpecification<T> Specification { get; }
    public bool IsChainDiscarded { get; set; }

    public IncludableSpecificationBuilder(ISpecification<T> specification, bool isChainDiscarded = false)
    {
        Specification = specification;
        IsChainDiscarded = isChainDiscarded;
    }
    
    IBasicSpecification<T> IBasicSpecificationBuilder<T>.Specification => Specification;
}
