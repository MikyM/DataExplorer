using DataExplorer.EfCore.Specifications.Exceptions;

namespace DataExplorer.EfCore.Specifications.Evaluators;

public class InMemorySpecificationEvaluator : IInMemorySpecificationEvaluator
{
    // Will use singleton for default configuration. Yet, it can be instantiated if necessary, with default or provided evaluators.
    public static InMemorySpecificationEvaluator Default { get; } = new();

    private readonly List<IInMemoryEvaluator> _evaluators = new();

    internal InMemorySpecificationEvaluator()
    {
        _evaluators.AddRange(new IInMemoryEvaluator[]
        {
            WhereEvaluator.Instance,
            SearchEvaluator.Instance,
            OrderEvaluator.Instance,
            PaginationEvaluator.Instance
        });
    }

    internal InMemorySpecificationEvaluator(IEnumerable<IInMemoryEvaluator> evaluators)
    {
        _evaluators.AddRange(evaluators);
    }

    public virtual IEnumerable<TResult> Evaluate<T, TResult>(IEnumerable<T> source,
        ISpecification<T, TResult> specification) where T : class
    {
        _ = specification.Selector ?? throw new SelectorNotFoundException();

        var baseQuery = Evaluate(source, (ISpecification<T>)specification);

        var resultQuery = baseQuery.Select(specification.Selector.Compile());

        return specification.PostProcessingAction == null
            ? resultQuery
            : specification.PostProcessingAction(resultQuery);
    }

    public virtual IEnumerable<T> Evaluate<T>(IEnumerable<T> source, ISpecification<T> specification) where T : class
    {
        foreach (var evaluator in _evaluators.OrderBy(x => x.ApplicationOrder))
        {
            source = evaluator.Evaluate(source, specification);
        }

        return specification.PostProcessingAction == null
            ? source
            : specification.PostProcessingAction(source);
    }
}
