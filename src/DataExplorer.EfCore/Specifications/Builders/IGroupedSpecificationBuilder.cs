namespace DataExplorer.EfCore.Specifications.Builders;

public interface IGroupedSpecificationBuilder<T> : IChainControlledSpecification, IBasicSpecificationBuilder<T> where T : class
{
}
