// -----------------------------------------------------------------------
// <copyright file="MediaTrackViewModel.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Media.Metadata.ViewModels;

/// <summary>
/// The <see cref="MediaTrack"/> view model.
/// </summary>
/// <param name="mediaTrack">The media track.</param>
internal sealed partial class MediaTrackViewModel(MediaTrack mediaTrack) : CommunityToolkit.Mvvm.ComponentModel.ObservableObject
{
    private static IEnumerable<string>? languages;

    [CommunityToolkit.Mvvm.ComponentModel.ObservableProperty]
    private string? selectedLanguage = mediaTrack.Language;

    /// <summary>
    /// Gets the ID.
    /// </summary>
    public int Id { get; init; } = mediaTrack.Id;

    /// <summary>
    /// Gets the type.
    /// </summary>
    public MediaTrackType Type { get; init; } = mediaTrack.Type;

    /// <summary>
    /// Gets the languages.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "For data binding")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S2325:Methods and properties that don't access instance data should be static", Justification = "For data binding")]
    public IEnumerable<string> Languages => GetLanguages();

    /// <summary>
    /// Converts this to a video.
    /// </summary>
    /// <returns>The video.</returns>
    public MediaTrack ToMediaTrack() => new(this.Id, this.Type, this.SelectedLanguage);

    private static IEnumerable<string> GetLanguages()
    {
        return languages ??= [.. ReadLanguages()];

        static IEnumerable<string> ReadLanguages()
        {
            var stream = typeof(App).Assembly.GetManifestResourceStream(typeof(App), "ISO-639-2_utf-8.txt") ?? throw new InvalidOperationException();
            using var reader = new StreamReader(stream, System.Text.Encoding.UTF8, leaveOpen: false);

            while (reader.ReadLine() is { } line)
            {
                var split = line.Split('|');
                var index = split.Length > 1 && !string.IsNullOrEmpty(split[1]) ? 1 : 0;
                yield return split[index];
            }
        }
    }
}