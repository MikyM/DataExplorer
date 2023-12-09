using DataExplorer.Abstractions.Specifications;
using DataExplorer.Abstractions.Specifications.Builders;

namespace DataExplorer.EfCore.Specifications.Builders;

[PublicAPI]
public class CacheSpecificationBuilder<T> : ICacheSpecificationBuilder<T> where T : class
{
    public ISpecification<T> Specification { get; }
    public bool IsChainDiscarded { get; set; }

    public CacheSpecificationBuilder(ISpecification<T> specification, bool isChainDiscarded = false)
    {
        Specification = specification;
        IsChainDiscarded = isChainDiscarded;
    }

    IBasicSpecification<T> IBasicSpecificationBuilder<T>.Specification => Specification;
}
