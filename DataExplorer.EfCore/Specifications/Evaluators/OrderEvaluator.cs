using DataExplorer.EfCore.Specifications.Exceptions;
using DataExplorer.EfCore.Specifications.Helpers;
// ReSharper disable PossibleMultipleEnumeration

namespace DataExplorer.EfCore.Specifications.Evaluators;

public class OrderEvaluator : IEvaluator, IInMemoryEvaluator, IEvaluatorBase
{
    private OrderEvaluator() { }
    public static OrderEvaluator Instance { get; } = new();

    public bool IsCriteriaEvaluator { get; } = false;
    public int ApplicationOrder { get; } = 0;

    public IQueryable<T> GetQuery<T>(IQueryable<T> query, ISpecification<T> specification) where T : class
    {
        if (specification.OrderExpressions != null)
        {
            if (specification.OrderExpressions.Count(x => x.OrderType == OrderTypeEnum.OrderBy
                    || x.OrderType == OrderTypeEnum.OrderByDescending) > 1)
            {
                throw new DuplicateOrderChainException();
            }

            IOrderedQueryable<T>? orderedQuery = null;
            foreach (var orderExpression in specification.OrderExpressions)
            {
                if (orderExpression.OrderType == OrderTypeEnum.OrderBy)
                {
                    orderedQuery = query.OrderBy(orderExpression.KeySelector);
                }
                else if (orderExpression.OrderType == OrderTypeEnum.OrderByDescending)
                {
                    orderedQuery = query.OrderByDescending(orderExpression.KeySelector);
                }
                else if (orderExpression.OrderType == OrderTypeEnum.ThenBy)
                {
                    orderedQuery = orderedQuery!.ThenBy(orderExpression.KeySelector);
                }
                else if (orderExpression.OrderType == OrderTypeEnum.ThenByDescending)
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

    public IEnumerable<T> Evaluate<T>(IEnumerable<T> query, ISpecification<T> specification) where T : class
    {
        if (specification.OrderExpressions != null)
        {
            if (specification.OrderExpressions.Count(x => x.OrderType == OrderTypeEnum.OrderBy
                    || x.OrderType == OrderTypeEnum.OrderByDescending) > 1)
            {
                throw new DuplicateOrderChainException();
            }

            IOrderedEnumerable<T>? orderedQuery = null;
            foreach (var orderExpression in specification.OrderExpressions)
            {
                if (orderExpression.OrderType == OrderTypeEnum.OrderBy)
                {
                    orderedQuery = query.OrderBy(orderExpression.KeySelectorFunc);
                }
                else if (orderExpression.OrderType == OrderTypeEnum.OrderByDescending)
                {
                    orderedQuery = query.OrderByDescending(orderExpression.KeySelectorFunc);
                }
                else if (orderExpression.OrderType == OrderTypeEnum.ThenBy)
                {
                    orderedQuery = orderedQuery!.ThenBy(orderExpression.KeySelectorFunc);
                }
                else if (orderExpression.OrderType == OrderTypeEnum.ThenByDescending)
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
