namespace DataExplorer.EfCore.Specifications.Evaluators;

public class WhereEvaluator : IEvaluator, IBasicInMemoryEvaluator, IInMemoryEvaluator, IEvaluatorMarker, IPreUpdateEvaluator, IInMemoryEvaluatorMarker
{
    private WhereEvaluator() { }
    public static WhereEvaluator Instance { get; } = new();

    public bool IsCriteriaEvaluator { get; } = true;
    public int ApplicationOrder { get; } = 0;
    
    public IEnumerable<T> Evaluate<T>(IEnumerable<T> query, IBasicSpecification<T> specification) where T : class
    {
        if (specification.WhereExpressions is null) return query;

        foreach (var info in specification.WhereExpressions)
        {
            query = query.Where(info.FilterFunc);
        }

        return query;
    }

    public IQueryable<T> GetQuery<T>(IQueryable<T> query, IBasicSpecification<T> specification) where T : class
    {
        if (specification.WhereExpressions is null) return query;

        foreach (var info in specification.WhereExpressions)
        {
            query = query.Where(info.Filter);
        }

        return query;
    }

    public IQueryable<T> GetQuery<T>(IQueryable<T> query, ISpecification<T> specification) where T : class
        => GetQuery(query, (IBasicSpecification<T>)specification);

    public IEnumerable<T> Evaluate<T>(IEnumerable<T> query, ISpecification<T> specification) where T : class
        => Evaluate(query, (IBasicSpecification<T>)specification);
}
