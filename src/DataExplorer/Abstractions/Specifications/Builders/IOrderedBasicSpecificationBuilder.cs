namespace DataExplorer.Abstractions.Specifications.Builders;

public interface IOrderedBasicSpecificationBuilder<T> : IChainControlledSpecification, IBasicSpecificationBuilder<T> where T : class
{
}
