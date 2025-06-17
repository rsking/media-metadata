// -----------------------------------------------------------------------
// <copyright file="TrackList.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Media.Metadata;

/// <summary>
/// Represents the collection of tracks in a file.
/// </summary>
internal sealed class TrackList : IReadOnlyList<Track>
{
    private readonly IList<Track> tracks;

    private TrackList(IEnumerable<Track> tracks) => this.tracks = [.. tracks];

    private TrackList() => this.tracks = [];

    /// <summary>
    /// Gets the number of <see cref="Track">Tracks</see> contained in this <see cref="TrackList"/>.
    /// </summary>
    public int Count => this.tracks.Count;

    /// <inheritdoc/>
    public Track this[int index] => this.tracks[index];

    /// <inheritdoc/>
    public IEnumerator<Track> GetEnumerator() => this.tracks.GetEnumerator();

    /// <inheritdoc/>
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => this.tracks.GetEnumerator();

    /// <summary>
    /// Creates the track information from the specified video.
    /// </summary>
    /// <param name="video">The video.</param>
    /// <returns>A new instance of a <see cref="TrackList"/> object containing the information about the track for the file.</returns>
    internal static TrackList Create(Video video) => Create(video.Tracks);

    /// <summary>
    /// Creates the track information from the specified tracks.
    /// </summary>
    /// <param name="tracks">The tracks.</param>
    /// <returns>A new instance of a <see cref="TrackList"/> object containing the information about the track for the file.</returns>
    internal static TrackList Create(IEnumerable<MediaTrack> tracks)
    {
        return new TrackList(tracks.Select(t => new Track(t.Id, GetType(t.Type)) { Language = t.Language }));

        static string? GetType(MediaTrackType type)
        {
            return type switch
            {
                MediaTrackType.Video => NativeMethods.MP4VideoTrackType,
                MediaTrackType.Audio => NativeMethods.MP4AudioTrackType,
                MediaTrackType.Text => NativeMethods.MP4TextTrackType,
                _ => default,
            };
        }
    }

    /// <summary>
    /// Reads the track information from the specified file.
    /// </summary>
    /// <param name="fileHandle">The handle to the file from which to read the track information.</param>
    /// <returns>A new instance of a <see cref="TrackList"/> object containing the information about the track for the file.</returns>
    internal static TrackList ReadFromFile(IntPtr fileHandle)
    {
        var list = new TrackList();
        for (short i = 0; i < NativeMethods.MP4GetNumberOfTracks(fileHandle, type: null, 0); i++)
        {
            var currentTrackId = NativeMethods.MP4FindTrackId(fileHandle, i, type: null, 0);
            var trackType = NativeMethods.MP4GetTrackType(fileHandle, currentTrackId);
            var language = GetLanguage(fileHandle, currentTrackId);
            list.tracks.Add(new Track(currentTrackId, trackType) { Language = language });
        }

        return list;
    }

    /// <summary>
    /// Writes the track information to the file.
    /// </summary>
    /// <param name="fileHandle">The handle to the file to which to write the chapter information.</param>
    internal void WriteToFile(IntPtr fileHandle)
    {
        foreach (var track in this.tracks)
        {
            var language = track.Language?.ToLowerInvariant() ?? "und";
            var currentLanguage = GetLanguage(fileHandle, track.Id);
            if (!string.Equals(currentLanguage, language, StringComparison.Ordinal))
            {
                var languageToSet = new byte[4];
                System.Text.Encoding.ASCII.GetBytes(language).CopyTo(languageToSet, 0);
                _ = NativeMethods.MP4SetTrackLanguage(fileHandle, track.Id, languageToSet);
            }
        }
    }

    private static string? GetLanguage(IntPtr fileHandle, int trackId)
    {
        var languageBytes = new byte[4];
        return NativeMethods.MP4GetTrackLanguage(fileHandle, trackId, languageBytes)
            ? System.Text.Encoding.ASCII.GetString(languageBytes).TrimEnd('\0').ToLowerInvariant()
            : default;
    }
}