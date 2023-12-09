using System.Linq.Expressions;
using DataExplorer.Abstractions.Specifications;

namespace DataExplorer.EfCore.Abstractions.Specifications;

#if NET7_0_OR_GREATER

/// <summary>
///     Encapsulates update logic and base query logic for <typeparamref name="T" />.
/// </summary>
[PublicAPI]
public interface IUpdateSpecification<T> : IBasicSpecification<T> where T : class
{
    /// <summary>
    /// The collection of update expressions.
    /// </summary>
    IEnumerable<Expression<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>>>? UpdateExpressions { get; set; }
}
#endif
