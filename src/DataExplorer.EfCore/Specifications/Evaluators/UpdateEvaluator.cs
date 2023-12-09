#if NET7_0_OR_GREATER 

using System.Linq.Expressions;
using DataExplorer.Abstractions.Specifications.Evaluators;
using DataExplorer.EfCore.Abstractions.Specifications;
using ExpressionExtensions = DataExplorer.EfCore.Specifications.Extensions.ExpressionExtensions;

namespace DataExplorer.EfCore.Specifications.Evaluators;

/// <inheritdoc cref="IUpdateEvaluator"/>
[PublicAPI]
public class UpdateEvaluator : IUpdateEvaluator, ISpecialCaseEvaluator
{
    public static UpdateEvaluator Instance { get; } = new();

    internal UpdateEvaluator()
    {
    }

    public Expression<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>> Evaluate<T>(IUpdateSpecification<T> specification) where T : class

    {
        if (specification.UpdateExpressions is null || !specification.UpdateExpressions.Any())
            throw new InvalidOperationException("At least one update expression is required");

        return ExpressionExtensions.Join(specification.UpdateExpressions);
    }

    public bool IsCriteriaEvaluator { get; } = false;
    public int ApplicationOrder { get; } = int.MaxValue;
}
#endif
