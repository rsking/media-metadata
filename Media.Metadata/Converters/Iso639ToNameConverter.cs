// -----------------------------------------------------------------------
// <copyright file="Iso639ToNameConverter.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Media.Metadata.Converters;

/// <summary>
/// The ISO639 to Name converter.
/// </summary>
internal sealed partial class Iso639ToNameConverter : Microsoft.UI.Xaml.Data.IValueConverter
{
    private static readonly Lock LoadingLock = new();

    private static Dictionary<string, string>? bibliographicToName;

    private static Dictionary<string, string>? terminologicToName;

    /// <inheritdoc/>
    public object? Convert(object? value, Type targetType, object parameter, string language)
    {
        EnsureDictionaries();
        return value switch
        {
            string key when bibliographicToName.TryGetValue(key, out var name) => name,
            string key when terminologicToName.TryGetValue(key, out var name) => name,
            string key => key,
            _ => default,
        };
    }

    /// <inheritdoc/>
    public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotSupportedException();

    [System.Diagnostics.CodeAnalysis.MemberNotNull(nameof(bibliographicToName), nameof(terminologicToName))]
    private static void EnsureDictionaries()
    {
        if (!LoadRequired())
        {
            return;
        }

        lock (LoadingLock)
        {
            if (!LoadRequired())
            {
                return;
            }

            var values = GetValues().ToList();
            bibliographicToName = values.ToDictionary(value => value.Bibliographic, value => value.Name, StringComparer.Ordinal);
            terminologicToName = values.Where(value => value.Terminologic is not null).ToDictionary(value => value.Terminologic!, value => value.Name, StringComparer.Ordinal);
        }

        [System.Diagnostics.CodeAnalysis.MemberNotNullWhen(false, nameof(bibliographicToName), nameof(terminologicToName))]
        static bool LoadRequired()
        {
            return bibliographicToName is null || terminologicToName is null;
        }
    }

    private static IEnumerable<Iso639> GetValues()
    {
        var stream = typeof(App).Assembly.GetManifestResourceStream(typeof(App), "ISO-639-2_utf-8.txt") ?? throw new InvalidOperationException();
        using var reader = new StreamReader(stream, System.Text.Encoding.UTF8, leaveOpen: false);

        while (reader.ReadLine() is { } line)
        {
            var split = line.Split('|');
            yield return new Iso639
            {
                Bibliographic = split[0],
                Terminologic = SomethingOrNull(split[1]),
                Name = split[3],
            };

            static string? SomethingOrNull(string value)
            {
                return string.IsNullOrEmpty(value) ? null : value;
            }
        }
    }

    private readonly record struct Iso639
    {
        public string Bibliographic { get; init; }

        public string? Terminologic { get; init; }

        public string Name { get; init; }
    }
}