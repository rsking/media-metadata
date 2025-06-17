// -----------------------------------------------------------------------
// <copyright file="PListExtensions.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Media.Metadata;

/// <summary>
/// Extension methods for <see cref="Formatters.PList.PList"/>.
/// </summary>
public static class PListExtensions
{
    /// <summary>
    /// Adds the value if it is not null or empty.
    /// </summary>
    /// <param name="propertyList">The list.</param>
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    public static void AddIfNotNullOrEmpty(this Formatters.PList.PList propertyList, string key, string? value)
    {
        if (value is null || string.IsNullOrEmpty(value))
        {
            return;
        }

        propertyList.Add(key, value);
    }

    /// <summary>
    /// Adds the values as a join string, if it is not null or empty.
    /// </summary>
    /// <param name="propertyList">The list.</param>
    /// <param name="key">The key.</param>
    /// <param name="separator">The separator.</param>
    /// <param name="values">The values.</param>
    public static void AddIfNotNullOrEmpty(this Formatters.PList.PList propertyList, string key, string separator, IEnumerable<string>? values)
    {
        if (values is null)
        {
            return;
        }

        propertyList.AddIfNotNullOrEmpty(key, string.Join(separator, values));
    }

    /// <summary>
    /// Adds the string values as dictionaries wrapped in an array.
    /// </summary>
    /// <param name="propertyList">The PList.</param>
    /// <param name="key">The array key.</param>
    /// <param name="values">The values.</param>
    public static void AddIfNotNull(this Formatters.PList.PList propertyList, string key, IEnumerable<string>? values)
    {
        if (values is null)
        {
            return;
        }

        Add(propertyList, key, values);
    }

    /// <summary>
    /// Adds the string values as dictionaries wrapped in an array.
    /// </summary>
    /// <param name="propertyList">The PList.</param>
    /// <param name="key">The array key.</param>
    /// <param name="values">The values.</param>
    public static void Add(this Formatters.PList.PList propertyList, string key, IEnumerable<string> values)
    {
        var @array = values.ToPListArray("name");
        if (array.Length is 0)
        {
            return;
        }

        propertyList.Add(key, array);
    }

    /// <summary>
    /// Serializes the <see cref="Formatters.PList.PList"/>.
    /// </summary>
    /// <param name="propertyList">The RList to serialize.</param>
    /// <returns>The string representation of <paramref name="propertyList"/>.</returns>
    public static string Serialize(this Formatters.PList.PList propertyList)
    {
        using var memoryStream = new MemoryStream();
        var formatter = new Formatters.PList.PListAsciiFormatter();
        formatter.Serialize(memoryStream, propertyList);
        return System.Text.Encoding.UTF8.GetString(memoryStream.ToArray());
    }

    private static object[] ToPListArray(this IEnumerable<string> values, string key) => [.. values.Select<string, object>(value => new Dictionary<string, object>(StringComparer.Ordinal) { { key, value } })];
}