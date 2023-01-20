namespace DataExplorer.EfCore.Specifications.Builders;

public interface IIncludableSpecificationBuilder<T, out TProperty> : ISpecificationBuilder<T> where T : class
{
    bool IsChainDiscarded { get; set; }
}
