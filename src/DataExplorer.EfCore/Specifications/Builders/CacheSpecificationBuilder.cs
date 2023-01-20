namespace DataExplorer.EfCore.Specifications.Builders;

[PublicAPI]
public class CacheSpecificationBuilder<T> : ICacheSpecificationBuilder<T> where T : class
{
    public Specification<T> Specification { get; }
    public bool IsChainDiscarded { get; set; }

    public CacheSpecificationBuilder(Specification<T> specification, bool isChainDiscarded = false)
    {
        Specification = specification;
        IsChainDiscarded = isChainDiscarded;
    }

    BasicSpecification<T> IBasicSpecificationBuilder<T>.Specification => Specification;
}
