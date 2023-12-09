namespace DataExplorer.Abstractions.Specifications.Evaluators;

[PublicAPI]
public interface IPreUpdateEvaluator : IBasicEvaluator
{
    IQueryable<T> GetQuery<T>(IQueryable<T> query, ISpecification<T> specification) where T : class;
}
