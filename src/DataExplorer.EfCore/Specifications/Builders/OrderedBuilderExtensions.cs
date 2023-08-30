using System.Linq.Expressions;
using DataExplorer.EfCore.Specifications.Expressions;
using DataExplorer.EfCore.Specifications.Helpers;

namespace DataExplorer.EfCore.Specifications.Builders;

[PublicAPI]
public static class OrderedBuilderExtensions
{
    /// <summary>
    /// Specify complex ordering
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="orderedBuilder"></param>
    /// <param name="orderExpression">Member to oder by</param>
    /// <returns>Current <see cref="IOrderedSpecificationBuilder{T}"/> instance</returns>
    public static IOrderedSpecificationBuilder<T> ThenBy<T>(
        this IOrderedSpecificationBuilder<T> orderedBuilder,
        Expression<Func<T, object?>> orderExpression) where T : class => ThenBy(orderedBuilder, orderExpression, true);

    /// <summary>
    /// Specify complex ordering
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="orderedBuilder"></param>
    /// <param name="orderExpression">Member to oder by</param>
    /// <param name="condition">Condition as to when should given member be ordered</param>
    /// <returns>Current <see cref="IOrderedSpecificationBuilder{T}"/> instance</returns>
    public static IOrderedSpecificationBuilder<T> ThenBy<T>(
        this IOrderedSpecificationBuilder<T> orderedBuilder,
        Expression<Func<T, object?>> orderExpression,
        bool condition) where T : class
    {
        if (condition && !orderedBuilder.IsChainDiscarded)
        {
            orderedBuilder.Specification.OrderExpressions ??= new List<OrderExpressionInfo<T>>();
            ((List<OrderExpressionInfo<T>>)orderedBuilder.Specification.OrderExpressions).Add(new OrderExpressionInfo<T>(orderExpression, OrderTypeEnum.ThenBy));
        }
        else
        {
            orderedBuilder.IsChainDiscarded = true;
        }

        return orderedBuilder;
    }

    /// <summary>
    /// Specify complex ordering
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="orderedBuilder"></param>
    /// <param name="orderExpression">Member to oder by</param>
    /// <returns>Current <see cref="IOrderedSpecificationBuilder{T}"/> instance</returns>
    public static IOrderedSpecificationBuilder<T> ThenByDescending<T>(
        this IOrderedSpecificationBuilder<T> orderedBuilder,
        Expression<Func<T, object?>> orderExpression) where T : class => ThenByDescending(orderedBuilder, orderExpression, true);

    /// <summary>
    /// Specify complex ordering
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="orderedBuilder"></param>
    /// <param name="orderExpression">Member to oder by</param>
    /// <param name="condition">Condition as to when should given member be ordered</param>
    /// <returns>Current <see cref="IOrderedSpecificationBuilder{T}"/> instance</returns>
    public static IOrderedSpecificationBuilder<T> ThenByDescending<T>(
        this IOrderedSpecificationBuilder<T> orderedBuilder,
        Expression<Func<T, object?>> orderExpression,
        bool condition) where T : class
    {
        if (condition && !orderedBuilder.IsChainDiscarded)
        {
            orderedBuilder.Specification.OrderExpressions ??= new List<OrderExpressionInfo<T>>();
            ((List<OrderExpressionInfo<T>>)orderedBuilder.Specification.OrderExpressions).Add(new OrderExpressionInfo<T>(orderExpression, OrderTypeEnum.ThenByDescending));
        }
        else
        {
            orderedBuilder.IsChainDiscarded = true;
        }

        return orderedBuilder;
    }
    
    /// <summary>
    /// Specify complex ordering
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="orderedBuilder"></param>
    /// <param name="orderExpression">Member to oder by</param>
    /// <returns>Current <see cref="IOrderedSpecificationBuilder{T}"/> instance</returns>
    public static IOrderedBasicSpecificationBuilder<T> ThenBy<T>(
        this IOrderedBasicSpecificationBuilder<T> orderedBuilder,
        Expression<Func<T, object?>> orderExpression) where T : class => ThenBy(orderedBuilder, orderExpression, true);

    /// <summary>
    /// Specify complex ordering
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="orderedBuilder"></param>
    /// <param name="orderExpression">Member to oder by</param>
    /// <param name="condition">Condition as to when should given member be ordered</param>
    /// <returns>Current <see cref="IOrderedSpecificationBuilder{T}"/> instance</returns>
    public static IOrderedBasicSpecificationBuilder<T> ThenBy<T>(
        this IOrderedBasicSpecificationBuilder<T> orderedBuilder,
        Expression<Func<T, object?>> orderExpression,
        bool condition) where T : class
    {
        if (condition && !orderedBuilder.IsChainDiscarded)
        {
            orderedBuilder.Specification.OrderExpressions ??= new List<OrderExpressionInfo<T>>();
            ((List<OrderExpressionInfo<T>>)orderedBuilder.Specification.OrderExpressions).Add(new OrderExpressionInfo<T>(orderExpression, OrderTypeEnum.ThenBy));
        }
        else
        {
            orderedBuilder.IsChainDiscarded = true;
        }

        return orderedBuilder;
    }

    /// <summary>
    /// Specify complex ordering
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="orderedBuilder"></param>
    /// <param name="orderExpression">Member to oder by</param>
    /// <returns>Current <see cref="IOrderedSpecificationBuilder{T}"/> instance</returns>
    public static IOrderedBasicSpecificationBuilder<T> ThenByDescending<T>(
        this IOrderedBasicSpecificationBuilder<T> orderedBuilder,
        Expression<Func<T, object?>> orderExpression) where T : class => ThenByDescending(orderedBuilder, orderExpression, true);

    /// <summary>
    /// Specify complex ordering
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="orderedBuilder"></param>
    /// <param name="orderExpression">Member to oder by</param>
    /// <param name="condition">Condition as to when should given member be ordered</param>
    /// <returns>Current <see cref="IOrderedSpecificationBuilder{T}"/> instance</returns>
    public static IOrderedBasicSpecificationBuilder<T> ThenByDescending<T>(
        this IOrderedBasicSpecificationBuilder<T> orderedBuilder,
        Expression<Func<T, object?>> orderExpression,
        bool condition) where T : class
    {
        if (condition && !orderedBuilder.IsChainDiscarded)
        {
            orderedBuilder.Specification.OrderExpressions ??= new List<OrderExpressionInfo<T>>();
            ((List<OrderExpressionInfo<T>>)orderedBuilder.Specification.OrderExpressions).Add(new OrderExpressionInfo<T>(orderExpression, OrderTypeEnum.ThenByDescending));
        }
        else
        {
            orderedBuilder.IsChainDiscarded = true;
        }

        return orderedBuilder;
    }
}
