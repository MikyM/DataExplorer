using System.Diagnostics.CodeAnalysis;
using Gridify;

namespace DataExplorer.EfCore.Gridify;

/// <summary>
/// Provides <see cref="IGridifyMapper{T}"/>.
/// </summary>
[PublicAPI]
public interface IGridifyMapperProvider
{
    /// <summary>
    /// Gets a dictionary containing registered mappers.
    /// <para>The key is the entity type, the value is a <see cref="IGridifyMapper{T}"/> where T is the entity type.</para>
    /// </summary>
    IReadOnlyDictionary<Type, object> Mappers { get; }

    /// <summary>
    /// Tries to retrieve a mapper for a specified entity type from <see cref="Mappers"/>.
    /// </summary>
    /// <param name="type">The entity type to retrieve a mapper for.</param>
    /// <param name="mapper">The mapper, if found.</param>
    /// <returns>True if the mapper was found, otherwise false.</returns>
    bool TryGetMapperFor(Type type, [NotNullWhen(true)] out object? mapper);

    /// <summary>
    /// Tries to retrieve a mapper for a specified entity type from <see cref="Mappers"/>.
    /// </summary>
    /// <typeparam name="T">The entity type to retrieve a mapper for.</typeparam>
    /// <param name="mapper">The mapper, if found.</param>
    /// <returns>True if the mapper was found, otherwise false.</returns>
    bool TryGetMapperFor<T>([NotNullWhen(true)] out IGridifyMapper<T>? mapper) where T : class;

    /// <summary>
    /// Retrieves a mapper for a specified entity type from <see cref="Mappers"/>.
    /// </summary>
    /// <param name="type">The entity type to retrieve a mapper for.</param>
    /// <returns>The mapper if found, otherwise null.</returns>
    object? GetMapperFor(Type type);

    /// <summary>
    /// Retrieves a mapper for a specified entity type from <see cref="Mappers"/>.
    /// </summary>
    /// <typeparam name="T">The entity type to retrieve a mapper for.</typeparam>
    /// <returns>The mapper if found, otherwise null.</returns>
    IGridifyMapper<T>? GetMapperFor<T>() where T : class;

    /// <summary>
    /// Adds a mapper.
    /// </summary>
    /// <typeparam name="T">The type of the entity.</typeparam>
    /// <param name="mapper">The mapper to add.</param>
    /// <returns>True if the mapper was added, otherwise false.</returns>
    bool AddMapper<T>(IGridifyMapper<T> mapper) where T : class;
}
