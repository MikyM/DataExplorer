namespace DataExplorer.Abstractions.Specifications.Builders;

public interface IGroupedSpecificationBuilder<T> : IGroupedBasicSpecificationBuilder<T>, ISpecificationBuilder<T> where T : class
{
}
