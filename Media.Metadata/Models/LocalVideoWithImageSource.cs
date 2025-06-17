// -----------------------------------------------------------------------
// <copyright file="LocalVideoWithImageSource.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Media.Metadata.Models;

/// <summary>
/// A local video with an <see cref="ImageSource"/>.
/// </summary>
/// <inheritdoc />
internal partial record class LocalVideoWithImageSource(
    FileInfo FileInfo,
    string? Name,
    string? Description,
    IEnumerable<string>? Producers,
    IEnumerable<string>? Directors,
    IEnumerable<string>? Studios,
    IEnumerable<string>? Genre,
    IEnumerable<string>? ScreenWriters,
    IEnumerable<string>? Cast,
    IEnumerable<string>? Composers) : VideoWithImageSource(Name, Description, Producers, Directors, Studios, Genre, ScreenWriters, Cast, Composers), ILocalVideo
{
    /// <summary>
    /// Initialises a new instance of the <see cref="LocalVideoWithImageSource"/> class.
    /// </summary>
    /// <param name="fileInfo">The file info.</param>
    /// <param name="video">The video.</param>
    public LocalVideoWithImageSource(FileInfo fileInfo, VideoWithImageSource video)
        : this(fileInfo, video.Name, video.Description, video.Producers, video.Directors, video.Studios, video.Genre, video.ScreenWriters, video.Cast, video.Composers)
    {
        this.Release = video.Release;
        this.Rating = video.Rating;
        this.Work = video.Work;
        this.Tracks = video.Tracks;
        this.Image = video.Image;
        this.ImageFormat = video.ImageFormat;
        this.ImageSource = video.ImageSource;
    }

    /// <summary>
    /// Creates a <see cref="LocalVideoWithImageSource"/> from a <see cref="LocalVideo"/>.
    /// </summary>
    /// <param name="video">The video.</param>
    /// <returns>The video with image source.</returns>
    public static async Task<LocalVideoWithImageSource> CreateAsync(LocalVideo video) => new LocalVideoWithImageSource(video.FileInfo, await VideoWithImageSource.CreateAsync(video).ConfigureAwait(false));
}