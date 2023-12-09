using System.Linq.Expressions;
using DataExplorer.Abstractions.Specifications;
using DataExplorer.Abstractions.Specifications.Evaluators;
using DataExplorer.EfCore.Abstractions.Specifications;

namespace DataExplorer.EfCore.Specifications.Evaluators;

/// <summary>
///     Evaluates the logic encapsulated by an <see cref="ISpecification{T}" />.
/// </summary>
[PublicAPI]
public interface IEfSpecificationEvaluator : ISpecificationEvaluator
{
#if NET7_0_OR_GREATER 
    /// <summary>
    ///     Applies the logic encapsulated by <paramref name="specification" /> to given <paramref name="query" /> and executes the update.
    /// </summary>
    /// <param name="query">The sequence of <typeparamref name="T" /></param>
    /// <param name="specification">The encapsulated query logic.</param>
    /// <param name="evaluateCriteriaOnly">Whether to only evaluate criteria.</param>
    /// <returns>The number of updated rows.</returns>
    (IQueryable<T> Query, Expression<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>> EvaluatedCalls) GetQuery<T>(IQueryable<T> query, IUpdateSpecification<T> specification,
        bool evaluateCriteriaOnly = false) where T : class;   
#endif
}
