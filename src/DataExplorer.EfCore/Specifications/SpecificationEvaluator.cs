using DataExplorer.EfCore.Specifications.Evaluators;
using DataExplorer.EfCore.Specifications.Exceptions;

namespace DataExplorer.EfCore.Specifications;

/// <inheritdoc cref="ISpecificationEvaluator" />
public class SpecificationEvaluator : ISpecificationEvaluator
{
    // Will use singleton for default configuration. Yet, it can be instantiated if necessary, with default or provided evaluators.
    /// <summary>
    /// <see cref="SpecificationEvaluator" /> instance with default evaluators and without any additional features enabled.
    /// </summary>
    public static SpecificationEvaluator Default { get; } = new();

    /// <summary>
    /// <see cref="SpecificationEvaluator" /> instance with default evaluators and enabled caching.
    /// </summary>
    public static SpecificationEvaluator Cached { get; } = new(true);

    private readonly List<IEvaluator> _evaluators = new();
    private readonly List<IBasicEvaluator> _basicEvaluators = new();
    private readonly List<IPreUpdateEvaluator> _preUpdateEvaluators = new();

    private readonly IProjectionEvaluator _projectionEvaluator;
    private readonly IUpdateEvaluator _updateEvaluator;

    internal SpecificationEvaluator(IEnumerable<IEvaluator> evaluators, IEnumerable<IBasicEvaluator> basicEvaluators, 
        IEnumerable<IPreUpdateEvaluator> preUpdateEvaluators, IProjectionEvaluator projectionEvaluator, IUpdateEvaluator updateEvaluator)
    {
        _projectionEvaluator = projectionEvaluator;
        _updateEvaluator = updateEvaluator;
        _preUpdateEvaluators.AddRange(preUpdateEvaluators);
        _evaluators.AddRange(evaluators);
        _basicEvaluators.AddRange(basicEvaluators);
    }

    internal SpecificationEvaluator(bool cacheEnabled = false)
    {
        _projectionEvaluator = ProjectionEvaluator.Instance;
        _updateEvaluator = UpdateEvaluator.Instance;
        _evaluators.AddRange(new List<IEvaluator>()
        {
            WhereEvaluator.Instance, SearchEvaluator.Instance, cacheEnabled ? IncludeEvaluator.Cached : IncludeEvaluator.Default,
            OrderEvaluator.Instance, PaginationEvaluator.Instance, AsNoTrackingEvaluator.Instance, AsTrackingEvaluator.Instance,
            AsSplitQueryEvaluator.Instance, AsNoTrackingWithIdentityResolutionEvaluator.Instance,
            GroupByEvaluator.Instance, CachingEvaluator.Instance
        });
        _basicEvaluators.AddRange(new List<IBasicEvaluator>()
        {
            WhereEvaluator.Instance, SearchEvaluator.Instance
        });
        _preUpdateEvaluators.AddRange(new List<IPreUpdateEvaluator>()
        {
            WhereEvaluator.Instance, SearchEvaluator.Instance
        });
    }

    public virtual IQueryable<TResult> GetQuery<T, TResult>(IQueryable<T> query,
        ISpecification<T, TResult> specification) where T : class
    {
        if (specification.Selector is not null && specification.SelectorMany is not null) 
            throw new ConcurrentSelectorsException();

        query = GetQuery(query, (ISpecification<T>)specification);

        if (specification.Selector is null && specification.SelectorMany is null)
            return _projectionEvaluator.GetQuery(query, specification);
        
        return specification.SelectorMany is not null 
            ? query.SelectMany(specification.SelectorMany) 
            : query.Select(specification.Selector!);
    }

    public virtual IQueryable<T> GetQuery<T>(IQueryable<T> query, ISpecification<T> specification,
        bool evaluateCriteriaOnly = false) where T : class
    {
        if (specification is null) 
            throw new ArgumentNullException(nameof(specification), "Specification is required");

        return (evaluateCriteriaOnly ? _evaluators.Where(x => x.IsCriteriaEvaluator) : _evaluators)
            .OrderBy(x => x.ApplicationOrder)
            .Aggregate(query, (current, evaluator) => evaluator.GetQuery(current, specification));
    }
    
    public virtual IQueryable<T> GetQuery<T>(IQueryable<T> query, IBasicSpecification<T> specification,
        bool evaluateCriteriaOnly = false) where T : class
    {
        if (specification is null) 
            throw new ArgumentNullException(nameof(specification), "Specification is required");

        return (evaluateCriteriaOnly ? _basicEvaluators.Where(x => x.IsCriteriaEvaluator) : _basicEvaluators)
            .OrderBy(x => x.ApplicationOrder)
            .Aggregate(query, (current, evaluator) => evaluator.GetQuery(current, specification));
    }

    public Task<int> EvaluateUpdateAsync<T>(IQueryable<T> query, IUpdateSpecification<T> specification,
        bool evaluateCriteriaOnly = false, CancellationToken cancellationToken = default) where T : class
    {
        if (specification is null) 
            throw new ArgumentNullException(nameof(specification), "Specification is required");

        return (evaluateCriteriaOnly ? _preUpdateEvaluators.Where(x => x.IsCriteriaEvaluator) : _preUpdateEvaluators)
            .OrderBy(x => x.ApplicationOrder)
            .Aggregate(query, (current, evaluator) => evaluator.GetQuery(current, specification))
            .ExecuteUpdateAsync(_updateEvaluator.Evaluate(specification), cancellationToken);
    }
}
