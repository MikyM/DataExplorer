using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using Gridify;

namespace DataExplorer.Gridify;

/// <summary>
/// Provides <see cref="IGridifyMapper{T}"/>.
/// </summary>
[PublicAPI]
public class GridifyMapperProvider : IGridifyMapperProvider
{
    private readonly ConcurrentDictionary<Type, object> _mappers = new();
    
    /// <inheritdoc/>
    public IReadOnlyDictionary<Type, object> Mappers => _mappers;

    /// <inheritdoc/>
    public bool TryGetMapperFor<T>([NotNullWhen(true)] out IGridifyMapper<T>? mapper) where T : class
    {
        if (_mappers.TryGetValue(typeof(T), out var objectMapper))
        {
            mapper = (IGridifyMapper<T>)objectMapper;
            return true;
        }

        mapper = null;
        return false;
    }

    /// <inheritdoc/>
    public object? GetMapperFor(Type type)
        => _mappers.GetValueOrDefault(type);

    /// <inheritdoc/>
    public IGridifyMapper<T>? GetMapperFor<T>() where T : class
        => (IGridifyMapper<T>?)_mappers.GetValueOrDefault(typeof(T));

    /// <inheritdoc/>
    public bool TryGetMapperFor(Type type, [NotNullWhen(true)] out object? mapper)
        => _mappers.TryGetValue(type, out mapper);

    /// <inheritdoc/>
    public bool AddMapper<T>(IGridifyMapper<T> mapper) where T : class
        => _mappers.TryAdd(typeof(T), mapper);
}
