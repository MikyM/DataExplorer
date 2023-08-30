namespace DataExplorer.EfCore.Specifications.Builders;

public interface IOrderedSpecificationBuilder<T> : IOrderedBasicSpecificationBuilder<T>, ISpecificationBuilder<T> where T : class
{
}
