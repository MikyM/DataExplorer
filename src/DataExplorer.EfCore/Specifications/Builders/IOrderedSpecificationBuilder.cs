namespace DataExplorer.EfCore.Specifications.Builders;

public interface IOrderedSpecificationBuilder<T> : IChainControlledSpecification, IBasicSpecificationBuilder<T> where T : class
{
}
