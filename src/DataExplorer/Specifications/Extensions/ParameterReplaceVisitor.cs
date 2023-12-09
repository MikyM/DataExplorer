using System.Linq.Expressions;

namespace DataExplorer.Specifications.Extensions;

/// <summary>
/// Represents a visitor that replaces a parameter with an expression.
/// </summary>
[PublicAPI]
public class ParameterReplacerVisitor : ExpressionVisitor
{
    private readonly Expression _newExpression;
    private readonly ParameterExpression _oldParameter;

    private ParameterReplacerVisitor(ParameterExpression oldParameter, Expression newExpression)
    {
        _oldParameter = oldParameter;
        _newExpression = newExpression;
    }

    public static Expression Replace(Expression expression, ParameterExpression oldParameter,
        Expression newExpression)
        => new ParameterReplacerVisitor(oldParameter, newExpression).Visit(expression);

    protected override Expression VisitParameter(ParameterExpression p)
    {
        return p == _oldParameter ? _newExpression : p;
    }
}
