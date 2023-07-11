namespace DataExplorer.EfCore.Specifications.Builders;

public interface IIncludableSpecificationBuilder<T, out TProperty> : IChainControlledSpecification, IBasicSpecificationBuilder<T> where T : class
{
}
