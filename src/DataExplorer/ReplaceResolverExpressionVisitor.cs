using System.Linq.Expressions;
using DataExplorer.Abstractions;

namespace DataExplorer;

/// <summary>
/// Factory expression visitor used to replace method calls done by <see cref="IResolver"/> to correct DI provider.
/// </summary>
public class ReplaceResolverExpressionVisitor<TProvider> : ExpressionVisitor
{
    private readonly ParameterExpression _pex;
    private readonly Func<ParameterExpression, MethodCallExpression, MethodCallExpression> _resolveRequiredFactory;
    private readonly Func<ParameterExpression, MethodCallExpression, MethodCallExpression>? _resolveKeyedFactory;

    public ReplaceResolverExpressionVisitor(
        Func<ParameterExpression, MethodCallExpression, MethodCallExpression> resolveRequiredFactory,
        Func<ParameterExpression, MethodCallExpression, MethodCallExpression>? resolveKeyedFactory = null)
    {
        _resolveRequiredFactory = resolveRequiredFactory;
        _resolveKeyedFactory = resolveKeyedFactory;

        _pex = Expression.Parameter(typeof(TProvider), "provider");
    }
    
    protected override Expression VisitMethodCall(MethodCallExpression node)
    {
        if (node.Method.DeclaringType != typeof(IResolver))
        {
            return base.VisitMethodCall(node);
        }

        if (node.Method.Name == "Resolve")
        {
            return _resolveRequiredFactory(_pex, node);
        }

        if (node.Method.Name == "ResolveKeyed")
        {
            if (_resolveKeyedFactory == null)
            {
                throw new InvalidOperationException("The factory expression visitor does not support resolve keyed method");
            }

            return _resolveKeyedFactory(_pex, node);
        }
        
        throw new InvalidOperationException("Could not determine method signature");
    }

    protected override Expression VisitParameter(ParameterExpression node)
    {
        if (node.Type != typeof(IResolver))
        {
            return base.VisitParameter(node);
        }

        return _pex;
    }
    
    protected override Expression VisitLambda<T>(Expression<T> node)
    {
        var delegateType = typeof(Func<TProvider,object>);

        var lambda = Expression.Lambda(delegateType, Visit(node.Body), _pex);
        
        return lambda;
    }

    protected override Expression VisitMember(MemberExpression node)
    {
        if (node.Member.DeclaringType == typeof(TProvider))
        {
            return Expression.Property(Visit(node.Expression)!, node.Member.Name);
        }
        
        return base.VisitMember(node);
    }
}