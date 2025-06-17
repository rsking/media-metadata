// -----------------------------------------------------------------------
// <copyright file="EpisodeWithImageSource.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Media.Metadata.Models;

/// <summary>
/// A episode with an <see cref="ImageSource"/>.
/// </summary>
/// <inheritdoc />
internal partial record class EpisodeWithImageSource(
    string? Name,
    string? Description,
    IEnumerable<string>? Producers,
    IEnumerable<string>? Directors,
    IEnumerable<string>? Studios,
    IEnumerable<string>? Genre,
    IEnumerable<string>? ScreenWriters,
    IEnumerable<string>? Cast,
    IEnumerable<string>? Composers) : Episode(Name, Description, Producers, Directors, Studios, Genre, ScreenWriters, Cast, Composers), IHasImageSource
{
    /// <inheritdoc/>
    public ImageSource? ImageSource { get; init; }

    /// <summary>
    /// Creates a <see cref="EpisodeWithImageSource"/> from a <see cref="Episode"/>.
    /// </summary>
    /// <param name="episode">The episode.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The video with image source.</returns>
    public static async Task<EpisodeWithImageSource> CreateAsync(Episode episode, CancellationToken cancellationToken = default) => new EpisodeWithImageSource(episode.Name, episode.Description, episode.Producers, episode.Directors, episode.Studios, episode.Genre, episode.ScreenWriters, episode.Cast, episode.Composers)
    {
        Release = episode.Release,
        Rating = episode.Rating,
        Tracks = episode.Tracks,
        Work = episode.Work,
        Show = episode.Show,
        Network = episode.Network,
        Season = episode.Season,
        Number = episode.Number,
        Id = episode.Id,
        Part = episode.Part,
        Image = episode.Image,
        ImageFormat = episode.ImageFormat,
        ImageSource = await episode.CreateImageSource(cancellationToken).ConfigureAwait(true),
    };
}