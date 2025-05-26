using System.Linq.Expressions;
using DataExplorer.Abstractions.Specifications;
using DataExplorer.Abstractions.Specifications.Evaluators;
using DataExplorer.EfCore.Abstractions.Specifications;
using DataExplorer.EfCore.Specifications.Evaluators;
using DataExplorer.Specifications.Exceptions;

namespace DataExplorer.EfCore.Specifications;

/// <inheritdoc cref="ISpecificationEvaluator" />
[PublicAPI]
public class SpecificationEvaluator : IEfSpecificationEvaluator
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
    
#if NET7_0_OR_GREATER 
    private readonly IUpdateEvaluator _updateEvaluator;
#endif

#if NET7_0_OR_GREATER 
    internal SpecificationEvaluator(IEnumerable<IEvaluator> evaluators, IEnumerable<IBasicEvaluator> basicEvaluators, 
        IEnumerable<IPreUpdateEvaluator> preUpdateEvaluators, IProjectionEvaluator projectionEvaluator, IUpdateEvaluator updateEvaluator)
    {
        _projectionEvaluator = projectionEvaluator;
        _updateEvaluator = updateEvaluator;
        _preUpdateEvaluators.AddRange(preUpdateEvaluators);
        _evaluators.AddRange(evaluators);
        _basicEvaluators.AddRange(basicEvaluators);
    }
#else
    internal SpecificationEvaluator(IEnumerable<IEvaluator> evaluators, IEnumerable<IBasicEvaluator> basicEvaluators, 
        IEnumerable<IPreUpdateEvaluator> preUpdateEvaluators, IProjectionEvaluator projectionEvaluator)
    {
        _projectionEvaluator = projectionEvaluator;
        _preUpdateEvaluators.AddRange(preUpdateEvaluators);
        _evaluators.AddRange(evaluators);
        _basicEvaluators.AddRange(basicEvaluators);
    }
#endif

    internal SpecificationEvaluator(bool cacheEnabled = false, IProjectionEvaluator? projectionEvaluator = null)
    {
        _projectionEvaluator = projectionEvaluator ?? DefaultProjectionEvaluator.Instance;
#if NET7_0_OR_GREATER 
        _updateEvaluator = UpdateEvaluator.Instance;
#endif
        _evaluators.AddRange(new List<IEvaluator>()
        {
            WhereEvaluator.Default, SearchEvaluator.Default, cacheEnabled ? IncludeEvaluator.Cached : IncludeEvaluator.Default,
            OrderEvaluator.Default, PaginationEvaluator.Default, AsNoTrackingEvaluator.Default, AsTrackingEvaluator.Default,
            AsSplitQueryEvaluator.Default, AsNoTrackingWithIdentityResolutionEvaluator.Default,
            GroupByEvaluator.Default, CachingEvaluator.Default
        });
        _basicEvaluators.AddRange(new List<IBasicEvaluator>()
        {
            WhereEvaluator.Default, SearchEvaluator.Default
        });
        _preUpdateEvaluators.AddRange(new List<IPreUpdateEvaluator>()
        {
            WhereEvaluator.Default, SearchEvaluator.Default, PaginationEvaluator.Default
        });
    }

    public virtual IQueryable<TResult> GetQuery<T, TResult>(IQueryable<T> query,
        ISpecification<T, TResult> specification) where T : class
    {
        if (specification.Selector is not null && specification.SelectorMany is not null) 
            throw new ConcurrentSelectorsException();

        query = GetQuery<T>(query, specification);

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

#if NET7_0_OR_GREATER 
    public (IQueryable<T> Query, Expression<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>> EvaluatedCalls) GetQuery<T>(IQueryable<T> query, IUpdateSpecification<T> specification,
        bool evaluateCriteriaOnly = false) where T : class
    {
        if (specification is null) 
            throw new ArgumentNullException(nameof(specification), "Specification is required");

        var resultQuery = (evaluateCriteriaOnly
                ? _preUpdateEvaluators.Where(x => x.IsCriteriaEvaluator)
                : _preUpdateEvaluators)
            .OrderBy(x => x.ApplicationOrder)
            .Aggregate(query, (current, evaluator) => evaluator.GetQuery(current, specification));
        
        return new ValueTuple<IQueryable<T>, Expression<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>>>(resultQuery, _updateEvaluator.Evaluate(specification));
    }
#endif
}
