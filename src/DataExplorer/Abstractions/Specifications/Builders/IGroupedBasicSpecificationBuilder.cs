namespace DataExplorer.Abstractions.Specifications.Builders;

public interface IGroupedBasicSpecificationBuilder<T> : IChainControlledSpecification, IBasicSpecificationBuilder<T> where T : class
{
}
