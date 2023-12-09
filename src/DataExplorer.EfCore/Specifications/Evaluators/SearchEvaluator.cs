using DataExplorer.Abstractions.Specifications;
using DataExplorer.Abstractions.Specifications.Evaluators;
using DataExplorer.EfCore.Specifications.Extensions;

namespace DataExplorer.EfCore.Specifications.Evaluators;

public class SearchEvaluator : IEvaluator, IInMemoryEvaluator, IEvaluatorBase, IPreUpdateEvaluator
{
    private SearchEvaluator() { }
    public static SearchEvaluator Default { get; } = new SearchEvaluator();

    public bool IsCriteriaEvaluator { get; } = true;
    public int ApplicationOrder { get; } = 0;
    public IQueryable<T> GetQuery<T>(IQueryable<T> query, IBasicSpecification<T> specification) where T : class
    {
        if (specification.SearchCriterias is null) return query;

        foreach (var searchCriteria in specification.SearchCriterias.GroupBy(x => x.SearchGroup))
        {
            query = query.Search(searchCriteria);
        }

        return query;
    }

    public IQueryable<T> GetQuery<T>(IQueryable<T> query, ISpecification<T> specification) where T : class
        => GetQuery(query, (IBasicSpecification<T>)specification);

    public IEnumerable<T> Evaluate<T>(IEnumerable<T> query, ISpecification<T> specification) where T : class
    {
        if (specification.SearchCriterias is null) return query;

        foreach (var searchGroup in specification.SearchCriterias.GroupBy(x => x.SearchGroup))
        {
            query = query.Where(x => searchGroup.Any(c => c.SelectorFunc(x).Like(c.SearchTerm)));
        }

        return query;
    }
}
