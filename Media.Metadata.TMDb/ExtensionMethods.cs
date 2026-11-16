// -----------------------------------------------------------------------
// <copyright file="ExtensionMethods.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Media.Metadata.TMDb;

/// <summary>
/// Provides extension methods for filtering null elements from sequences.
/// </summary>
internal static class ExtensionMethods
{
    /// <summary>
    /// Gets all values that are not null.
    /// </summary>
    /// <typeparam name="T">The type.</typeparam>
    /// <param name="source">The source.</param>
    /// <returns>The result.</returns>
    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?>? source)
        where T : class
    {
        if (source is null)
        {
            yield break;
        }

#pragma warning disable S3267 // Loops should be simplified with "LINQ" expressions
        foreach (var item in source)
        {
            if (item is not null)
            {
                yield return item;
            }
        }
#pragma warning restore S3267 // Loops should be simplified with "LINQ" expressions
    }

    /// <summary>
    /// Materizlizes the enumerable.
    /// </summary>
    /// <typeparam name="T">The type.</typeparam>
    /// <param name="source">The source.</param>
    /// <returns>The result.</returns>
    public static ICollection<T> Materialize<T>(this IEnumerable<T> source) => [.. source];
}