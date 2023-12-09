namespace DataExplorer.Abstractions.Specifications.Builders;

public interface IIncludableSpecificationBuilder<T, out TProperty> : IChainControlledSpecification, ISpecificationBuilder<T> where T : class
{
}
