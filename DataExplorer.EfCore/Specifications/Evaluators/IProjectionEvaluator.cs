namespace DataExplorer.EfCore.Specifications.Evaluators;

public interface IProjectionEvaluator
{
    IQueryable<TResult> GetQuery<T, TResult>(IQueryable<T> query, ISpecification<T, TResult> specification)
        where T : class where TResult : class;
}