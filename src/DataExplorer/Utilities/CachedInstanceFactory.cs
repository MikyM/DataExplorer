using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

namespace DataExplorer.Utilities;

/// <inheritdoc/>
[UsedImplicitly]
public class CachedInstanceFactory : ICachedInstanceFactory
{
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
}

internal delegate object CreateDelegate(object? arg1, object? arg2, object? arg3, object? arg4);

internal static class CachedInstanceFactory<TArg1, TArg2, TArg3, TArg4>
{
  // ReSharper disable once StaticMemberInGenericType
  // this is fine - we will still properly cache the delegates because we use our own TypeToIgnore type to polyfill the generics
  private static readonly ConcurrentDictionary<Type, CreateDelegate> _cachedFuncs = new();

  internal static object? CreateInstance(Type type, TArg1? arg1, TArg2? arg2, TArg3? arg3, TArg4? arg4)
    => _cachedFuncs.TryGetValue(type, out var func) ? func(arg1, arg2, arg3, arg4) : CacheFunc(type)(arg1, arg2, arg3, arg4);

  private static CreateDelegate CacheFunc(Type type)
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
      Expression.Parameter(typeof(object)),
      Expression.Parameter(typeof(object)),
      Expression.Parameter(typeof(object)),
      Expression.Parameter(typeof(object))
    };

    var constructor = type.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, constructorTypes.ToArray());
    var constructorParameters = parameters.Take(constructorTypes.Count)
      .Select((x,i) => Expression.Convert(x, constructorTypes[i]));
    
    var newExpr = Expression.New(constructor ?? throw new InvalidOperationException("Constructor not found"), constructorParameters);
    var lambdaExpr = Expression.Lambda<CreateDelegate>(newExpr, parameters);
    
    var func = lambdaExpr.Compile();
    _cachedFuncs.TryAdd(type, func);
    
    return func;
  }
}

internal class TypeToIgnore
{
}
