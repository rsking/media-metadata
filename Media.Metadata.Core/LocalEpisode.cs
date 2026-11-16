// -----------------------------------------------------------------------
// <copyright file="LocalEpisode.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Media.Metadata;

/// <summary>
/// Represents a local episode.
/// </summary>
/// <inheritdoc cref="Episode" />
public record class LocalEpisode(
    FileInfo FileInfo,
    string? Name,
    string? Description,
    IEnumerable<string>? Producers,
    IEnumerable<string>? Directors,
    IEnumerable<string>? Studios,
    IEnumerable<string>? Genre,
    IEnumerable<string>? ScreenWriters,
    IEnumerable<string>? Cast,
    IEnumerable<string>? Composers) : Episode(Name, Description, Producers, Directors, Studios, Genre, ScreenWriters, Cast, Composers), ILocalVideo
{
    /// <summary>
    /// Initialises a new instance of the <see cref="LocalEpisode"/> class.
    /// </summary>
    /// <param name="video">The video.</param>
    public LocalEpisode(LocalVideo video)
        : this(video.FileInfo, video.Name, video.Description, video.Producers, video.Directors, video.Studios, video.Genre, video.ScreenWriters, video.Cast, video.Composers)
    {
    }
}