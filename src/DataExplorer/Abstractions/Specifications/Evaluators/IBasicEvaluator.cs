namespace DataExplorer.Abstractions.Specifications.Evaluators;

[PublicAPI]
public interface IBasicEvaluator : IEvaluatorData
{
    IQueryable<T> GetQuery<T>(IQueryable<T> query, IBasicSpecification<T> specification) where T : class;
}
