namespace DataExplorer.Abstractions.Specifications.Evaluators;

[PublicAPI]
public interface IEvaluator : IEvaluatorData
{
    IQueryable<T> GetQuery<T>(IQueryable<T> query, ISpecification<T> specification) where T : class;
}
