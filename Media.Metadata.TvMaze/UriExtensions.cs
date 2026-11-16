// -----------------------------------------------------------------------
// <copyright file="UriExtensions.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Media.Metadata.TvMaze;

/// <summary>
/// Extensions for <see cref="Uri"/> instances.
/// </summary>
public static class UriExtensions
{
    /// <summary>
    /// Merges the base URL and the resource.
    /// </summary>
    /// <param name="baseUrl">The base URL.</param>
    /// <param name="resource">The resource.</param>
    /// <returns>The merged value.</returns>
    /// <exception cref="ArgumentException">Both <paramref name="baseUrl"/> and <paramref name="resource"/> are empty.</exception>
    public static Uri MergeBaseUrlAndResource(this Uri? baseUrl, string? resource)
    {
        var assembled = resource;
        if (IsNotEmpty(assembled) && assembled!.StartsWith("/", StringComparison.Ordinal))
        {
            assembled = assembled[1..];
        }

        if (baseUrl is null || IsEmpty(baseUrl.AbsoluteUri))
        {
            return IsNotEmpty(assembled)
                ? new Uri(assembled)
                : throw new ArgumentException("Both BaseUrl and Resource are empty", nameof(resource));
        }

        var usingBaseUri = baseUrl.AbsoluteUri.EndsWith("/", StringComparison.Ordinal) || IsEmpty(assembled)
            ? baseUrl
            : new Uri($"{baseUrl.AbsoluteUri}/");

        return assembled is not null
            ? new Uri(usingBaseUri, assembled)
            : baseUrl;

        static bool IsEmpty(string? value)
        {
            return string.IsNullOrWhiteSpace(value);
        }

        static bool IsNotEmpty(string? value)
        {
            return !IsEmpty(value);
        }
    }
}