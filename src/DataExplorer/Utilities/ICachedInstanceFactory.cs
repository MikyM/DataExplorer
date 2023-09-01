namespace DataExplorer.Utilities;

/// <summary>
/// Factory that utilizes delegate caching for creating instances.
/// </summary>
public interface ICachedInstanceFactory
{
    /// <summary>
    /// Creates an instance of a given type.
    /// </summary>
    /// <param name="type">Type to create.</param>
    /// <returns>Created type if successful, otherwise null.</returns>
    object? CreateInstance(Type type);

    /// <summary>
    /// Creates an instance of a given type.
    /// </summary>
    /// <param name="type">Type to create.</param>
    /// <param name="arg1">First argument of type's constructor.</param>
    /// <typeparam name="TArg1">Type of the first argument of type's constructor.</typeparam>
    /// <returns>Created type if successful, otherwise null.</returns>
    object? CreateInstance<TArg1>(Type type, TArg1 arg1);

    /// <summary>
    /// Creates an instance of a given type.
    /// </summary>
    /// <param name="type">Type to create.</param>
    /// <param name="arg1">First argument of type's constructor.</param>
    /// <param name="arg2">Second argument of type's constructor.</param>
    /// <typeparam name="TArg1">Type of the first argument of type's constructor.</typeparam>
    /// <typeparam name="TArg2">Type of the second argument of type's constructor.</typeparam>
    /// <returns>Created type if successful, otherwise null.</returns>
    object? CreateInstance<TArg1, TArg2>(Type type, TArg1 arg1, TArg2 arg2);

    /// <summary>
    /// Creates an instance of a given type.
    /// </summary>
    /// <param name="type">Type to create.</param>
    /// <param name="arg1">First argument of type's constructor.</param>
    /// <param name="arg2">Second argument of type's constructor.</param>
    /// <param name="arg3">Third argument of type's constructor.</param>
    /// <typeparam name="TArg1">Type of the first argument of type's constructor.</typeparam>
    /// <typeparam name="TArg2">Type of the second argument of type's constructor.</typeparam>
    /// <typeparam name="TArg3">Type of the third argument of type's constructor.</typeparam>
    /// <returns>Created type if successful, otherwise null.</returns>
    object? CreateInstance<TArg1, TArg2, TArg3>(Type type, TArg1 arg1, TArg2 arg2, TArg3 arg3);

    /// <summary>
    /// Creates an instance of a given type.
    /// </summary>
    /// <param name="type">Type to create.</param>
    /// <param name="arg1">First argument of type's constructor.</param>
    /// <param name="arg2">Second argument of type's constructor.</param>
    /// <param name="arg3">Third argument of type's constructor.</param>
    /// <param name="arg4">Fourth argument of type's constructor.</param>
    /// <typeparam name="TArg1">Type of the first argument of type's constructor.</typeparam>
    /// <typeparam name="TArg2">Type of the second argument of type's constructor.</typeparam>
    /// <typeparam name="TArg3">Type of the third argument of type's constructor.</typeparam>
    /// <typeparam name="TArg4">Type of the fourth argument of type's constructor.</typeparam>
    /// <returns>Created type if successful, otherwise null.</returns>
    object? CreateInstance<TArg1, TArg2, TArg3, TArg4>(Type type, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4);
    
    /// <summary>
    /// Creates an instance of a given type.
    /// </summary>
    /// <typeparam name="TType">Type to create.</typeparam>
    object? CreateInstance<TType>() where TType : class;
    
    /// <summary>
    /// Creates an instance of a given type.
    /// </summary>
    /// <typeparam name="TType">Type to create.</typeparam>
    /// <param name="arg1">First argument of type's constructor.</param>
    /// <typeparam name="TArg1">Type of the first argument of type's constructor.</typeparam>
    /// <returns>Created type if successful, otherwise null.</returns>
    object? CreateInstance<TType, TArg1>(TArg1 arg1) where TType : class;
    
    /// <summary>
    /// Creates an instance of a given type.
    /// </summary>
    /// <typeparam name="TType">Type to create.</typeparam>
    /// <param name="arg1">First argument of type's constructor.</param>
    /// <param name="arg2">Second argument of type's constructor.</param>
    /// <typeparam name="TArg1">Type of the first argument of type's constructor.</typeparam>
    /// <typeparam name="TArg2">Type of the second argument of type's constructor.</typeparam>
    /// <returns>Created type if successful, otherwise null.</returns>
    object? CreateInstance<TType, TArg1, TArg2>(TArg1 arg1, TArg2 arg2) where TType : class;
    
    /// <summary>
    /// Creates an instance of a given type.
    /// </summary>
    /// <typeparam name="TType">Type to create.</typeparam>
    /// <param name="arg1">First argument of type's constructor.</param>
    /// <param name="arg2">Second argument of type's constructor.</param>
    /// <param name="arg3">Third argument of type's constructor.</param>
    /// <typeparam name="TArg1">Type of the first argument of type's constructor.</typeparam>
    /// <typeparam name="TArg2">Type of the second argument of type's constructor.</typeparam>
    /// <typeparam name="TArg3">Type of the third argument of type's constructor.</typeparam>
    /// <returns>Created type if successful, otherwise null.</returns>
    object? CreateInstance<TType, TArg1, TArg2, TArg3>(TArg1 arg1, TArg2 arg2, TArg3 arg3) where TType : class;

    /// <summary>
    /// Creates an instance of a given type.
    /// </summary>
    /// <typeparam name="TType">Type to create.</typeparam>
    /// <typeparam name="TArg1">Type of the first argument of type's constructor.</typeparam>
    /// <typeparam name="TArg2">Type of the second argument of type's constructor.</typeparam>
    /// <typeparam name="TArg3">Type of the third argument of type's constructor.</typeparam>
    /// <typeparam name="TArg4">Type of the fourth argument of type's constructor.</typeparam>
    /// <param name="arg1">First argument of type's constructor.</param>
    /// <param name="arg2">Second argument of type's constructor.</param>
    /// <param name="arg3">Third argument of type's constructor.</param>
    /// <param name="arg4">Fourth argument of type's constructor.</param>
    /// <returns>Created type if successful, otherwise null.</returns>
    object? CreateInstance<TType, TArg1, TArg2, TArg3, TArg4>(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4) where TType : class;

    /// <summary>
    /// Creates an instance of a given type.
    /// </summary>
    /// <param name="type">Type to create.</param>
    /// <param name="args">Constructor arguments.</param>
    /// <returns>Created type if successful, otherwise null.</returns>
    object? CreateInstance(Type type, params object?[]? args);
    
    /// <summary>
    /// Creates an instance of a given type.
    /// </summary>
    /// <typeparam name="TType">Type to create.</typeparam>
    /// <param name="args">Constructor arguments.</param>
    /// <returns>Created type if successful, otherwise null.</returns>
    object? CreateInstance<TType>(params object?[]? args) where TType : class;
}
