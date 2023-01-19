namespace DataExplorer.EfCore.Specifications.Evaluators;

public interface IEvaluator
{
    bool IsCriteriaEvaluator { get; }
    
    int ApplicationOrder { get; }

    IQueryable<T> GetQuery<T>(IQueryable<T> query, ISpecification<T> specification) where T : class;
}
