namespace DataExplorer.EfCore.Specifications.Evaluators;

public interface IInMemoryEvaluator
{
    IEnumerable<T> Evaluate<T>(IEnumerable<T> query, ISpecification<T> specification) where T : class;
}