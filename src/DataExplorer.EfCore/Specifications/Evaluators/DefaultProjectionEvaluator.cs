using DataExplorer.Abstractions.Specifications;
using DataExplorer.Abstractions.Specifications.Evaluators;

namespace DataExplorer.EfCore.Specifications.Evaluators;

public class DefaultProjectionEvaluator : IProjectionEvaluator, ISpecialCaseEvaluator
{
    public static DefaultProjectionEvaluator Instance { get; } = new();

    internal DefaultProjectionEvaluator()
    {
        
    }

    public IQueryable<TResult> GetQuery<T, TResult>(IQueryable<T> query, ISpecification<T, TResult> specification) where T : class
    {
        throw new InvalidOperationException(
            "You must either use a projection mapper library and/or implement your own projection evaluator");
    }
}
