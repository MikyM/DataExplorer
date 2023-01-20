using System.Linq.Expressions;

namespace DataExplorer.EfCore.Specifications;

/// <summary>
///     Encapsulates update logic and base query logic for <typeparamref name="T" />.
/// </summary>
[PublicAPI]
public interface IUpdateSpecification<T> : IBasicSpecification<T> where T : class
{
    /// <summary>
    /// The collection of update expressions.
    /// </summary>
    IEnumerable<Expression<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>>>? UpdateExpressions { get; }
}
