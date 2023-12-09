using DataExplorer.Abstractions.Specifications;
using DataExplorer.Abstractions.Specifications.Evaluators;

namespace DataExplorer.EfCore.Specifications.Evaluators;

/// <summary>
/// This evaluator applies EF Core's IgnoreQueryFilters feature to a given query
/// See: https://docs.microsoft.com/en-us/ef/core/querying/filters
/// </summary>
public class IgnoreQueryFiltersEvaluator : IEvaluator, IBasicEvaluator, IEvaluatorBase
{
    private IgnoreQueryFiltersEvaluator() { }
    public static IgnoreQueryFiltersEvaluator Instance { get; } = new IgnoreQueryFiltersEvaluator();

    public bool IsCriteriaEvaluator { get; } = true;
    public int ApplicationOrder { get; } = 0;
    public IQueryable<T> GetQuery<T>(IQueryable<T> query, IBasicSpecification<T> specification) where T : class
        => specification.IgnoreQueryFilters 
            ? query.IgnoreQueryFilters() 
            : query;

    public IQueryable<T> GetQuery<T>(IQueryable<T> query, ISpecification<T> specification) where T : class
        => GetQuery(query, (IBasicSpecification<T>)specification);
}
