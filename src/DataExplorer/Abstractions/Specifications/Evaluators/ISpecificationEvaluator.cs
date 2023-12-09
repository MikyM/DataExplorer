namespace DataExplorer.Abstractions.Specifications.Evaluators;

/// <summary>
///     Evaluates the logic encapsulated by an <see cref="ISpecification{T}" />.
/// </summary>
[PublicAPI]
public interface ISpecificationEvaluator
{
    /// <summary>
    ///     Applies the logic encapsulated by <paramref name="specification" /> to given <paramref name="query" />,
    ///     and projects the result into <typeparamref name="TResult" />.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <typeparam name="T"></typeparam>
    /// <param name="query">The sequence of <typeparamref name="T" /></param>
    /// <param name="specification">The encapsulated query logic.</param>
    /// <returns>A filtered sequence of <typeparamref name="TResult" /></returns>
    IQueryable<TResult> GetQuery<T, TResult>(IQueryable<T> query, ISpecification<T, TResult> specification) where T : class;

    /// <summary>
    ///     Applies the logic encapsulated by <paramref name="specification" /> to given <paramref name="query" />.
    /// </summary>
    /// <param name="query">The sequence of <typeparamref name="T" /></param>
    /// <param name="specification">The encapsulated query logic.</param>
    /// <param name="evaluateCriteriaOnly">Whether to only evaluate criteria.</param>
    /// <returns>A filtered sequence of <typeparamref name="T" /></returns>
    IQueryable<T> GetQuery<T>(IQueryable<T> query, ISpecification<T> specification,
        bool evaluateCriteriaOnly = false) where T : class;
    
    /// <summary>
    ///     Applies the logic encapsulated by <paramref name="specification" /> to given <paramref name="query" />.
    /// </summary>
    /// <param name="query">The sequence of <typeparamref name="T" /></param>
    /// <param name="specification">The encapsulated query logic.</param>
    /// <param name="evaluateCriteriaOnly">Whether to only evaluate criteria.</param>
    /// <returns>A filtered sequence of <typeparamref name="T" /></returns>
    IQueryable<T> GetQuery<T>(IQueryable<T> query, IBasicSpecification<T> specification,
        bool evaluateCriteriaOnly = false) where T : class;
}
