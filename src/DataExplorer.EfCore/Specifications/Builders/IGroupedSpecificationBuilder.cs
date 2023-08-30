namespace DataExplorer.EfCore.Specifications.Builders;

public interface IGroupedSpecificationBuilder<T> : IGroupedBasicSpecificationBuilder<T>, ISpecificationBuilder<T> where T : class
{
}
