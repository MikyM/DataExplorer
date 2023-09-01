using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

namespace DataExplorer.Utilities;

/// <inheritdoc/>
[UsedImplicitly]
public class CachedInstanceFactory : ICachedInstanceFactory
{
  private delegate object? CreateDelegate(Type type, object? arg1, object? arg2, object? arg3, object? arg4);

  private readonly ConcurrentDictionary<Tuple<Type, Type, Type, Type, Type>, CreateDelegate> _cachedFuncs = new();

  /// <inheritdoc/>
  public object? CreateInstance(Type type)
    => CachedInstanceFactory<TypeToIgnore, TypeToIgnore, TypeToIgnore, TypeToIgnore>.CreateInstance(type, null, null, null, null);

  /// <inheritdoc/>
  public object? CreateInstance<TArg1>(Type type, TArg1 arg1)
    => CachedInstanceFactory<TArg1, TypeToIgnore, TypeToIgnore, TypeToIgnore>.CreateInstance(type, arg1, null, null, null);

  /// <inheritdoc/>
  public object? CreateInstance<TArg1, TArg2>(Type type, TArg1 arg1, TArg2 arg2)
    => CachedInstanceFactory<TArg1, TArg2, TypeToIgnore, TypeToIgnore>.CreateInstance(type, arg1, arg2, null, null);

  /// <inheritdoc/>
  public object? CreateInstance<TArg1, TArg2, TArg3>(Type type, TArg1 arg1, TArg2 arg2, TArg3 arg3)
    => CachedInstanceFactory<TArg1, TArg2, TArg3, TypeToIgnore>.CreateInstance(type, arg1, arg2, arg3, null);
  
  /// <inheritdoc/>
  public object? CreateInstance<TArg1, TArg2, TArg3, TArg4>(Type type, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4)
    => CachedInstanceFactory<TArg1, TArg2, TArg3, TArg4>.CreateInstance(type, arg1, arg2, arg3, arg4);
  
  /// <inheritdoc/>
  public object? CreateInstance<TType>() where TType : class
    => CachedInstanceFactory<TypeToIgnore, TypeToIgnore, TypeToIgnore, TypeToIgnore>.CreateInstance(typeof(TType), null, null, null, null) as TType;

  /// <inheritdoc/>
  public object? CreateInstance<TType, TArg1>(TArg1 arg1) where TType : class
    => CachedInstanceFactory<TArg1, TypeToIgnore, TypeToIgnore, TypeToIgnore>.CreateInstance(typeof(TType), arg1, null, null, null) as TType;

  /// <inheritdoc/>
  public object? CreateInstance<TType, TArg1, TArg2>(TArg1 arg1, TArg2 arg2) where TType : class
    => CachedInstanceFactory<TArg1, TArg2, TypeToIgnore, TypeToIgnore>.CreateInstance(typeof(TType), arg1, arg2, null, null) as TType;

  /// <inheritdoc/>
  public object? CreateInstance<TType, TArg1, TArg2, TArg3>(TArg1 arg1, TArg2 arg2, TArg3 arg3) where TType : class
    => CachedInstanceFactory<TArg1, TArg2, TArg3, TypeToIgnore>.CreateInstance(typeof(TType), arg1, arg2, arg3, null) as TType;
  
  /// <inheritdoc/>
  public object? CreateInstance<TType, TArg1, TArg2, TArg3, TArg4>(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4) where TType : class
    => CachedInstanceFactory<TArg1, TArg2, TArg3, TArg4>.CreateInstance(typeof(TType), arg1, arg2, arg3, arg4) as TType;

  /// <inheritdoc/>
  public object? CreateInstance<TType>(params object?[]? args) where TType : class
    => CreateInstance(typeof(TType), args?.ToArray());
  
  /// <inheritdoc/>
  public object? CreateInstance(Type type, params object?[]? args)
  {
    if (args is null)
      return CreateInstance(type);

    if (args.Length > 4 || 
      (args.Length > 0 && args[0] == null) ||
      (args.Length > 1 && args[1] == null) ||
      (args.Length > 2 && args[2] == null) ||
      (args.Length > 3 && args[3] == null))
    {
        return Activator.CreateInstance(type, args);   
    }

    var arg0 = args.Length > 0 ? args[0] : null;
    var arg1 = args.Length > 1 ? args[1] : null;
    var arg2 = args.Length > 2 ? args[2] : null;
    var arg3 = args.Length > 3 ? args[3] : null;

    var key = Tuple.Create(
      type,
      arg0?.GetType() ?? typeof(TypeToIgnore),
      arg1?.GetType() ?? typeof(TypeToIgnore),
      arg2?.GetType() ?? typeof(TypeToIgnore),
      arg3?.GetType() ?? typeof(TypeToIgnore));
    
    return _cachedFuncs.TryGetValue(key, out var func) 
      ? func(type, arg0, arg1, arg2, arg3) 
      : CacheFunc(key)(type, arg0, arg1, arg2, arg3);
  }

  private CreateDelegate CacheFunc(Tuple<Type, Type, Type, Type, Type> key)
  {
    var types = new[] { key.Item1, key.Item2, key.Item3, key.Item4 };
    var method = typeof(CachedInstanceFactory)
      .GetMethods()
      .Where(m => m.Name == "CreateInstance").Single(m => m.GetParameters().Length == 4);
    var generic = method.MakeGenericMethod(key.Item2, key.Item3, key.Item4);

    var paramExpr = new List<ParameterExpression>();
    paramExpr.Add(Expression.Parameter(typeof(Type)));
    
    for (var i = 0; i < 3; i++)
      paramExpr.Add(Expression.Parameter(typeof(object)));

    var callParamExpr = new List<Expression>();
    callParamExpr.Add(paramExpr[0]);
    
    for (var i = 1; i < 4; i++)
      callParamExpr.Add(Expression.Convert(paramExpr[i], types[i]));
    
    var callExpr = Expression.Call(generic, callParamExpr);
    var lambdaExpr = Expression.Lambda<CreateDelegate>(callExpr, paramExpr);
    var func = lambdaExpr.Compile();
    _cachedFuncs.TryAdd(key, func);
    return func;
  }
}

internal static class CachedInstanceFactory<TArg1, TArg2, TArg3, TArg4>
{
  private static readonly ConcurrentDictionary<Type, Func<TArg1?, TArg2?, TArg3?, TArg4?, object>> CachedFuncs = new();

  internal static object? CreateInstance(Type type, TArg1? arg1, TArg2? arg2, TArg3? arg3, TArg4? arg4)
    => CachedFuncs.TryGetValue(type, out var func) ? func(arg1, arg2, arg3, arg4) : CacheFunc(type, arg1, arg2, arg3, arg4)(arg1, arg2, arg3, arg4);

  private static Func<TArg1?, TArg2?, TArg3?, TArg4?, object> CacheFunc(Type type, TArg1? arg1, TArg2? arg2, TArg3? arg3, TArg4? arg4)
  {
    var constructorTypes = new List<Type>();
    if (typeof(TArg1) != typeof(TypeToIgnore))
      constructorTypes.Add(typeof(TArg1));
    if (typeof(TArg2) != typeof(TypeToIgnore))
      constructorTypes.Add(typeof(TArg2));
    if (typeof(TArg3) != typeof(TypeToIgnore))
      constructorTypes.Add(typeof(TArg3));
    if (typeof(TArg4) != typeof(TypeToIgnore))
      constructorTypes.Add(typeof(TArg4));

    var parameters = new List<ParameterExpression>()
    {
      Expression.Parameter(typeof(TArg1)),
      Expression.Parameter(typeof(TArg2)),
      Expression.Parameter(typeof(TArg3)),
      Expression.Parameter(typeof(TArg4))
    };

    var constructor = type.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, constructorTypes.ToArray());
    var constructorParameters = parameters.Take(constructorTypes.Count).ToList();
    
    var newExpr = Expression.New(constructor ?? throw new InvalidOperationException(), constructorParameters);
    var lambdaExpr = Expression.Lambda<Func<TArg1?, TArg2?, TArg3?, TArg4?, object>>(newExpr, parameters);
    
    var func = lambdaExpr.Compile();
    
    CachedFuncs.TryAdd(type, func);
    
    return func;
  }
}

internal class TypeToIgnore
{
}
