namespace DataExplorer.EfCore.Specifications.Evaluators;

public class AsNoTrackingEvaluator : IEvaluator, IEvaluatorMarker
{
    private AsNoTrackingEvaluator()
    {
    }

    public static AsNoTrackingEvaluator Default { get; } = new();

    public bool IsCriteriaEvaluator { get; } = true;
    public int ApplicationOrder { get; } = 0;

    public IQueryable<T> GetQuery<T>(IQueryable<T> query, ISpecification<T> specification) where T : class
    {
        if (specification.IsAsNoTracking) query = query.AsNoTracking();

        return query;
    }
}
