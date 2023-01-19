namespace DataExplorer.EfCore.Specifications;

public interface ISingleResultSpecification<T> : ISpecification<T>, ISingleResultSpecification where T : class
{
}