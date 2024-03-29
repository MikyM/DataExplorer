﻿using System.Diagnostics.CodeAnalysis;

namespace DataExplorer.Extensions;

// ReSharper disable once InconsistentNaming
/// <summary>
/// <see cref="IEnumerable{T}"/> extensions.
/// </summary>
[PublicAPI]
public static class EnumerableExtensions
{
    /// <summary>
    /// LINQ extensions for Any method supporting nullable types.
    /// </summary>
    /// <typeparam name="T">Type wrapped by the <see cref="IEnumerable{T}"/></typeparam>
    /// <param name="source"><see cref="IEnumerable{T}"/> source </param>
    /// <param name="predicate">Given predicate</param>
    /// <returns>Whether a sequence contains elements that satisfy <paramref name="predicate"/>, if given sequence is null returns false.</returns>
    [System.Diagnostics.Contracts.Pure]
    public static bool AnyNullable<T>([NotNullWhen(true)] this IEnumerable<T>? source, Func<T, bool> predicate)
        => source is not null && source.Any(predicate);

    /// <summary>
    /// LINQ extensions for Any method supporting nullable types.
    /// </summary>
    /// <typeparam name="T">Type wrapped by the <see cref="IEnumerable{T}"/></typeparam>
    /// <param name="source"><see cref="IEnumerable{T}"/> source </param>
    /// <returns>Whether a sequence contains any elements, if given sequence is null returns false.</returns>
    [System.Diagnostics.Contracts.Pure]
    public static bool AnyNullable<T>([NotNullWhen(true)] this IEnumerable<T>? source)
        => source is not null && source.Any();
}
