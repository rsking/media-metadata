// -----------------------------------------------------------------------
// <copyright file="LocalEpisodeWithImageSource.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Media.Metadata.Models;

/// <summary>
/// A episode with an <see cref="ImageSource"/>.
/// </summary>
/// <inheritdoc />
internal partial record class LocalEpisodeWithImageSource(
    FileInfo FileInfo,
    string? Name,
    string? Description,
    IEnumerable<string>? Producers,
    IEnumerable<string>? Directors,
    IEnumerable<string>? Studios,
    IEnumerable<string>? Genre,
    IEnumerable<string>? ScreenWriters,
    IEnumerable<string>? Cast,
    IEnumerable<string>? Composers) : EpisodeWithImageSource(Name, Description, Producers, Directors, Studios, Genre, ScreenWriters, Cast, Composers), ILocalVideo
{
    /// <summary>
    /// Initialises a new instance of the <see cref="LocalEpisodeWithImageSource"/> class.
    /// </summary>
    /// <param name="fileInfo">The file info.</param>
    /// <param name="episode">The episode.</param>
    public LocalEpisodeWithImageSource(FileInfo fileInfo, EpisodeWithImageSource episode)
        : this(fileInfo, episode.Name, episode.Description, episode.Producers, episode.Directors, episode.Studios, episode.Genre, episode.ScreenWriters, episode.Cast, episode.Composers)
    {
        this.Release = episode.Release;
        this.Rating = episode.Rating;
        this.Tracks = episode.Tracks;
        this.Work = episode.Work;
        this.Show = episode.Show;
        this.Network = episode.Network;
        this.Season = episode.Season;
        this.Number = episode.Number;
        this.Id = episode.Id;
        this.Part = episode.Part;
        this.Image = episode.Image;
        this.ImageFormat = episode.ImageFormat;
        this.ImageSource = episode.ImageSource;
    }

    /// <summary>
    /// Creates a <see cref="EpisodeWithImageSource"/> from a <see cref="Episode"/>.
    /// </summary>
    /// <param name="episode">The episode.</param>
    /// <returns>The video with image source.</returns>
    public static async Task<LocalEpisodeWithImageSource> CreateAsync(LocalEpisode episode) => new LocalEpisodeWithImageSource(episode.FileInfo, await EpisodeWithImageSource.CreateAsync(episode).ConfigureAwait(false));
}