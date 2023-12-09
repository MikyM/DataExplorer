using DataExplorer.Abstractions.Specifications;
using DataExplorer.Abstractions.Specifications.Evaluators;

namespace DataExplorer.EfCore.Specifications.Evaluators;

public class GroupByEvaluator : IEvaluator, IBasicEvaluator, IInMemoryEvaluator, IEvaluatorBase, IInMemoryEvaluatorMarker
{
    private GroupByEvaluator()
    {
    }

    public static GroupByEvaluator Default { get; } = new();

    public bool IsCriteriaEvaluator { get; } = false;
    public int ApplicationOrder { get; } = 0;
    public IQueryable<T> GetQuery<T>(IQueryable<T> query, IBasicSpecification<T> specification) where T : class 
        => specification.GroupByExpression is null
            ? query
            : query.GroupBy(specification.GroupByExpression).SelectMany(x => x);

    public IQueryable<T> GetQuery<T>(IQueryable<T> query, ISpecification<T> specification) where T : class
        => GetQuery(query, (IBasicSpecification<T>)specification);

    public IEnumerable<T> Evaluate<T>(IEnumerable<T> query, ISpecification<T> specification) where T : class 
        => specification.GroupByExpression is null
            ? query
            : query.GroupBy(specification.GroupByExpression.Compile()).SelectMany(x => x);
}
