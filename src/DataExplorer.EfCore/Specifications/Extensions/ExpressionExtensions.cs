using System.Linq.Expressions;

namespace DataExplorer.EfCore.Specifications.Extensions;

[PublicAPI]
public static class ExpressionExtensions
{
    public static Expression<Func<T, bool>> AndAlso<T>(this Expression<Func<T, bool>> leftExpression,
        Expression<Func<T, bool>> rightExpression) =>
        Combine(leftExpression, rightExpression, Expression.AndAlso);

    public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> leftExpression,
        Expression<Func<T, bool>> rightExpression) =>
        Combine(leftExpression, rightExpression, Expression.Or);

    public static Expression<Func<T, bool>> Combine<T>(Expression<Func<T, bool>> leftExpression,
        Expression<Func<T, bool>> rightExpression, Func<Expression, Expression, BinaryExpression> combineOperator)
    {
        var leftParameter = leftExpression.Parameters[0];
        var rightParameter = rightExpression.Parameters[0];

        var visitor = new ReplaceParameterVisitor(rightParameter, leftParameter);

        var leftBody = leftExpression.Body;
        var rightBody = visitor.Visit(rightExpression.Body);

        return Expression.Lambda<Func<T, bool>>(combineOperator(leftBody, rightBody), leftParameter);
    }

#if NET7_0_OR_GREATER 
    public static Expression<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>> Join<T>(this
            Expression<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>> leftExpression,
        Expression<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>> rightExpression)
    {
        var newSharedParam = Expression.Parameter(typeof(SetPropertyCalls<T>), "arbitraryParamDataExplorer");

        var left = new ReplaceExpressionVisitor(leftExpression.Parameters[0], newSharedParam)
            .Visit(leftExpression.Body);
        
        var right =
            new ReplaceExpressionVisitor(rightExpression.Parameters[0], left ?? throw new InvalidOperationException())
                .Visit(rightExpression.Body);

        return Expression.Lambda<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>>(
            right ?? throw new InvalidOperationException(), newSharedParam);
    }
    
    public static Expression<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>> Join<T>(
        params Expression<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>>[] expressions)
    {
        if (expressions.Length == 0)
            throw new InvalidOperationException();
        if (expressions.Length == 1)
            return expressions[0];
        if (expressions.Length == 2)
            return expressions[0].Join(expressions[1]);

        var newSharedParam = Expression.Parameter(typeof(SetPropertyCalls<T>), "joinParam");

        var newExp = expressions[0].Join(expressions[1], newSharedParam);
        
        for (var i = 2; i < expressions.Length; i++)
            newExp = newExp.Join(expressions[i], newSharedParam);

        return newExp;
    }
    
    private static Expression<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>> Join<T>(this
            Expression<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>> leftExpression,
        Expression<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>> rightExpression, ParameterExpression parameterExpression)
    {
        var left = new ReplaceExpressionVisitor(leftExpression.Parameters[0], parameterExpression)
            .Visit(leftExpression.Body) ?? throw new InvalidOperationException();

        var right = new ReplaceExpressionVisitor(rightExpression.Parameters[0], left).Visit(rightExpression.Body);

        return Expression.Lambda<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>>(
            right ?? throw new InvalidOperationException(), parameterExpression);
    }

    public static Expression<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>> Join<T>(
        IEnumerable<Expression<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>>> expressions)
        => Join(expressions.ToArray());
#endif

    private class ReplaceExpressionVisitor : ExpressionVisitor
    {
        private readonly Expression _exp1;
        private readonly Expression _exp2;

        public ReplaceExpressionVisitor(Expression exp1, Expression exp2)
        {
            _exp1 = exp1;
            _exp2 = exp2;
        }

        public override Expression? Visit(Expression? node)
        {
            return ReferenceEquals(node, _exp1) ? _exp2 : base.Visit(node);
        }
    }

    private class ReplaceParameterVisitor : ExpressionVisitor
    {
        private readonly ParameterExpression _oldParameter;
        private readonly ParameterExpression _newParameter;

        public ReplaceParameterVisitor(ParameterExpression oldParameter, ParameterExpression newParameter)
        {
            _oldParameter = oldParameter;
            _newParameter = newParameter;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return ReferenceEquals(node, _oldParameter) ? _newParameter : base.VisitParameter(node);
        }
    }
}
