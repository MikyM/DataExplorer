using DataExplorer.Abstractions.Specifications;
using DataExplorer.Abstractions.Specifications.Evaluators;
using DataExplorer.Specifications;
using DataExplorer.Specifications.Exceptions;

// ReSharper disable PossibleMultipleEnumeration

namespace DataExplorer.EfCore.Specifications.Evaluators;

public class OrderEvaluator : IEvaluator, IBasicEvaluator, IInMemoryEvaluator, IEvaluatorBase
{
    private OrderEvaluator() { }
    public static OrderEvaluator Default { get; } = new();

    public bool IsCriteriaEvaluator { get; } = false;
    public int ApplicationOrder { get; } = 0;
    public IQueryable<T> GetQuery<T>(IQueryable<T> query, IBasicSpecification<T> specification) where T : class
    {
        if (specification.OrderExpressions != null)
        {
            if (specification.OrderExpressions.Count(x => x.OrderType == OrderType.OrderBy
                                                          || x.OrderType == OrderType.OrderByDescending) > 1)
            {
                throw new DuplicateOrderChainException();
            }

            IOrderedQueryable<T>? orderedQuery = null;
            foreach (var orderExpression in specification.OrderExpressions)
            {
                if (orderExpression.OrderType == OrderType.OrderBy)
                {
                    orderedQuery = query.OrderBy(orderExpression.KeySelector);
                }
                else if (orderExpression.OrderType == OrderType.OrderByDescending)
                {
                    orderedQuery = query.OrderByDescending(orderExpression.KeySelector);
                }
                else if (orderExpression.OrderType == OrderType.ThenBy)
                {
                    orderedQuery = orderedQuery!.ThenBy(orderExpression.KeySelector);
                }
                else if (orderExpression.OrderType == OrderType.ThenByDescending)
                {
                    orderedQuery = orderedQuery!.ThenByDescending(orderExpression.KeySelector);
                }
            }

            if (orderedQuery != null)
            {
                query = orderedQuery;
            }
        }

        return query;
    }

    public IQueryable<T> GetQuery<T>(IQueryable<T> query, ISpecification<T> specification) where T : class
        => GetQuery(query, (IBasicSpecification<T>)specification);

    public IEnumerable<T> Evaluate<T>(IEnumerable<T> query, ISpecification<T> specification) where T : class
    {
        if (specification.OrderExpressions != null)
        {
            if (specification.OrderExpressions.Count(x => x.OrderType == OrderType.OrderBy
                    || x.OrderType == OrderType.OrderByDescending) > 1)
            {
                throw new DuplicateOrderChainException();
            }

            IOrderedEnumerable<T>? orderedQuery = null;
            foreach (var orderExpression in specification.OrderExpressions)
            {
                if (orderExpression.OrderType == OrderType.OrderBy)
                {
                    orderedQuery = query.OrderBy(orderExpression.KeySelectorFunc);
                }
                else if (orderExpression.OrderType == OrderType.OrderByDescending)
                {
                    orderedQuery = query.OrderByDescending(orderExpression.KeySelectorFunc);
                }
                else if (orderExpression.OrderType == OrderType.ThenBy)
                {
                    orderedQuery = orderedQuery!.ThenBy(orderExpression.KeySelectorFunc);
                }
                else if (orderExpression.OrderType == OrderType.ThenByDescending)
                {
                    orderedQuery = orderedQuery!.ThenByDescending(orderExpression.KeySelectorFunc);
                }
            }

            if (orderedQuery != null)
            {
                query = orderedQuery;
            }
        }
        
        return query;
    }
}
