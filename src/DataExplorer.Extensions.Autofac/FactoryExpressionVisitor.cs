using System.Linq.Expressions;
using Autofac;
using DataExplorer.Abstractions;

namespace DataExplorer.Extensions.Autofac;

internal class FactoryExpressionVisitor : ExpressionVisitor
{
    private ParameterExpression _pex;

    public FactoryExpressionVisitor()
    {
        _pex = Expression.Parameter(typeof(IComponentContext), "componentContext");
    }
    
    protected override Expression VisitMethodCall(MethodCallExpression node)
    {
        if (node.Method.DeclaringType != typeof(IResolver))
        {
            return base.VisitMethodCall(node);
        }
        
        if (node.Method.Name == "Resolve")
        {
            var resolveMethod = typeof(ResolutionExtensions).GetMethods().Where(x => x is { Name: nameof(ResolutionExtensions.Resolve), IsGenericMethodDefinition: true })
                .FirstOrDefault(x => x.GetParameters().Length == 1) ?? throw new InvalidOperationException("Could not find get required service method.");
        
            var actualMethod = resolveMethod.MakeGenericMethod(node.Method.ReturnType);

            var methodCall = Expression.Call(null, actualMethod, _pex);
        
            return methodCall;
        }

        if (node.Method.Name == "ResolveKeyed")
        {
            var getRequiredServiceMethod = typeof(ResolutionExtensions).GetMethods().Where(x =>
                                               x is
                                               {
                                                   Name: nameof(ResolutionExtensions.ResolveKeyed),
                                                   IsGenericMethodDefinition: true
                                               }).FirstOrDefault(x => x.GetParameters().Length == 2) ?? throw new InvalidOperationException("Could not find get required service method.")
                                           ?? throw new InvalidOperationException(
                                               "Could not find get required service method.");

            var actualMethod = getRequiredServiceMethod.MakeGenericMethod(node.Method.ReturnType);

            var methodCall = Expression.Call(null, actualMethod, _pex, node.Arguments.First(x => x.Type != typeof(IResolver)));

            return methodCall;
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
        var delegateType = typeof(Func<IComponentContext,object>);

        var lambda = Expression.Lambda(delegateType, Visit(node.Body), _pex);
        
        return lambda;
    }

    protected override Expression VisitMember(MemberExpression node)
    {
        if (node.Member.DeclaringType == typeof(IComponentContext))
        {
            return Expression.Property(Visit(node.Expression)!, node.Member.Name);
        }
        
        return base.VisitMember(node);
    }
}