// -----------------------------------------------------------------------
// <copyright file="VideoWithImageSource.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Media.Metadata.Models;

/// <summary>
/// A video with an <see cref="ImageSource"/>.
/// </summary>
/// <inheritdoc />
internal partial record class VideoWithImageSource(
    string? Name,
    string? Description,
    IEnumerable<string>? Producers,
    IEnumerable<string>? Directors,
    IEnumerable<string>? Studios,
    IEnumerable<string>? Genre,
    IEnumerable<string>? ScreenWriters,
    IEnumerable<string>? Cast,
    IEnumerable<string>? Composers) : Video(Name, Description, Producers, Directors, Studios, Genre, ScreenWriters, Cast, Composers), IHasImageSource
{
    private bool disposedValue;

    /// <inheritdoc/>
    public ImageSource? ImageSource { get; init; }

    /// <summary>
    /// Creates a <see cref="VideoWithImageSource"/> from a <see cref="LocalVideo"/>.
    /// </summary>
    /// <param name="video">The video.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The video with image source.</returns>
    public static async Task<VideoWithImageSource> CreateAsync(Video video, CancellationToken cancellationToken = default) => new VideoWithImageSource(video.Name, video.Description, video.Producers, video.Directors, video.Studios, video.Genre, video.ScreenWriters, video.Cast, video.Composers)
    {
        Release = video.Release,
        Rating = video.Rating,
        Work = video.Work,
        Tracks = video.Tracks,
        Image = video.Image,
        ImageFormat = video.ImageFormat,
        ImageSource = await video.CreateImageSource(cancellationToken).ConfigureAwait(true),
    };

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing && !this.disposedValue && this.ImageSource is IDisposable imageSourceDisposable)
        {
            imageSourceDisposable.Dispose();
        }
    }

    /// <inheritdoc/>
    protected override async ValueTask DisposeAsyncCore()
    {
        await base.DisposeAsyncCore().ConfigureAwait(false);

        await DisposeAsync(this.ImageSource).ConfigureAwait(false);
        this.disposedValue = true;

        static async ValueTask DisposeAsync(object? value)
        {
            if (value is IAsyncDisposable asyncDisposable)
            {
                await asyncDisposable.DisposeAsync().ConfigureAwait(false);
            }
            else if (value is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
}