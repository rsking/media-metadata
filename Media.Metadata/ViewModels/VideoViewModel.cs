// -----------------------------------------------------------------------
// <copyright file="VideoViewModel.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Media.Metadata.ViewModels;

/// <summary>
/// The editable video.
/// </summary>
[System.Diagnostics.CodeAnalysis.SuppressMessage("Critical Bug", "S4275:Getters and setters should access the expected fields", Justification = "These are referenced.")]
internal partial class VideoViewModel : CommunityToolkit.Mvvm.ComponentModel.ObservableObject, ILocalVideo, IHasImage, Models.IHasImageSource, IAsyncDisposable, IDisposable
{
    // list backing fields
    private readonly System.Collections.ObjectModel.ObservableCollection<string> producers = [];
    private readonly System.Collections.ObjectModel.ObservableCollection<string> directors = [];
    private readonly System.Collections.ObjectModel.ObservableCollection<string> studios = [];
    private readonly System.Collections.ObjectModel.ObservableCollection<string> genre = [];
    private readonly System.Collections.ObjectModel.ObservableCollection<string> screenWriters = [];
    private readonly System.Collections.ObjectModel.ObservableCollection<string> cast = [];
    private readonly System.Collections.ObjectModel.ObservableCollection<string> composers = [];

    [CommunityToolkit.Mvvm.ComponentModel.ObservableProperty]
    private string? name;

    [CommunityToolkit.Mvvm.ComponentModel.ObservableProperty]
    private string? description;

    [CommunityToolkit.Mvvm.ComponentModel.ObservableProperty]
    private DateTimeOffset? release;

    [CommunityToolkit.Mvvm.ComponentModel.ObservableProperty]
    private string? work;

    private ImageSource? imageSource;

    private SixLabors.ImageSharp.Image? image;

    private SixLabors.ImageSharp.Formats.IImageFormat? imageFormat;

    private bool disposedValue;

    /// <summary>
    /// Initialises a new instance of the <see cref="VideoViewModel"/> class.
    /// </summary>
    /// <param name="video">The video.</param>
    public VideoViewModel(Models.LocalVideoWithImageSource video)
        : this(video, video.FileInfo, video.Image, video.ImageFormat, video.ImageSource)
    {
    }

    /// <summary>
    /// Initialises a new instance of the <see cref="VideoViewModel"/> class.
    /// </summary>
    /// <param name="video">The video.</param>
    /// <param name="fileInfo">The file information.</param>
    /// <param name="image">The image.</param>
    /// <param name="imageFormat">The image format.</param>
    /// <param name="imageSource">The image source.</param>
    protected VideoViewModel(Video video, FileInfo fileInfo, SixLabors.ImageSharp.Image? image, SixLabors.ImageSharp.Formats.IImageFormat? imageFormat, ImageSource? imageSource)
    {
        this.FileInfo = fileInfo;
        this.Name = video.Name;
        this.Description = video.Description;
        FillFrom(video.Producers, this.producers);
        FillFrom(video.Directors, this.directors);
        FillFrom(video.Studios, this.studios);
        FillFrom(video.Genre, this.genre);
        FillFrom(video.ScreenWriters, this.screenWriters);
        FillFrom(video.Cast, this.cast);
        FillFrom(video.Composers, this.composers);
        this.Release = video is { Release: { Ticks: not 0 } videoRelease } ? new DateTimeOffset(videoRelease) : default(DateTimeOffset?);
        this.Rating = new RatingViewModel(video.Rating);
        this.work = video.Work;
        this.Tracks = [.. video.Tracks.Select(track => new MediaTrackViewModel(track))];
        this.Image = image;
        this.ImageFormat = imageFormat;
        this.ImageSource = imageSource;
    }

    /// <summary>
    /// Gets or sets the video type.
    /// </summary>
    public Models.VideoType VideoType { get; set; } = Models.VideoType.NotSet;

    /// <inheritdoc/>
    public FileInfo FileInfo { get; }

    /// <summary>
    /// Gets the rating.
    /// </summary>
    public RatingViewModel Rating { get; init; }

    /// <summary>
    /// Gets the tracks.
    /// </summary>
    public IEnumerable<MediaTrackViewModel> Tracks { get; init; }

    /// <summary>
    /// Gets the producers.
    /// </summary>
    public IList<string> Producers { get => this.producers; init => FillFrom(value, this.producers); }

    /// <summary>
    /// Gets the directors.
    /// </summary>
    public IList<string> Directors { get => this.directors; init => FillFrom(value, this.directors); }

    /// <summary>
    /// Gets the directors.
    /// </summary>
    public IList<string> Studios { get => this.studios; init => FillFrom(value, this.studios); }

    /// <summary>
    /// Gets the directors.
    /// </summary>
    public IList<string> Genre { get => this.genre; init => FillFrom(value, this.genre); }

    /// <summary>
    /// Gets the directors.
    /// </summary>
    public IList<string> ScreenWriters { get => this.screenWriters; init => FillFrom(value, this.screenWriters); }

    /// <summary>
    /// Gets the directors.
    /// </summary>
    public IList<string> Cast { get => this.cast; init => FillFrom(value, this.cast); }

    /// <summary>
    /// Gets the directors.
    /// </summary>
    public IList<string> Composers { get => this.composers; init => FillFrom(value, this.composers); }

    /// <inheritdoc/>
    public ImageSource? ImageSource
    {
        get => this.imageSource;
        private set => this.SetProperty(ref this.imageSource, value);
    }

    /// <inheritdoc/>
    public SixLabors.ImageSharp.Image? Image
    {
        get => this.image;
        set => this.SetProperty(ref this.image, value);
    }

    /// <inheritdoc/>
    public SixLabors.ImageSharp.Formats.IImageFormat? ImageFormat
    {
        get => this.imageFormat;
        set => this.SetProperty(ref this.imageFormat, value);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        // Perform async cleanup.
        await this.DisposeAsyncCore().ConfigureAwait(false);

        // Dispose of unmanaged resources.
        this.Dispose(disposing: false);

        // Suppress finalization.
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Converts this to a video.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The video.</returns>
    public virtual Task<Video> ToVideoAsync(CancellationToken cancellationToken = default)
    {
        var localVideo = new LocalVideo(this.FileInfo, this.Name, this.Description, this.Producers, this.Directors, this.Studios, this.Genre, this.ScreenWriters, this.Cast, this.Composers)
        {
            Rating = this.Rating.SelectedRating,
            Release = this.Release?.DateTime,
            Work = this.Work,
            Tracks = [.. this.Tracks.Select(track => track.ToMediaTrack())],
            Image = this.Image,
            ImageFormat = this.ImageFormat,
        };

        Video returnValue = this.VideoType switch
        {
            Models.VideoType.Movie => new LocalMovie(localVideo),
            Models.VideoType.TVShow => new LocalEpisode(localVideo),
            _ => localVideo,
        };

        return Task.FromResult(returnValue);
    }

    /// <summary>
    /// Updates this instance with the information from the video.
    /// </summary>
    /// <param name="video">The video.</param>
    public virtual void Update(Video video)
    {
        if (video is null)
        {
            return;
        }

        this.Name = video.Name;
        this.Description = video.Description;

        FillFrom(video.Producers, this.producers);
        FillFrom(video.Directors, this.directors);
        FillFrom(video.Studios, this.studios);
        FillFrom(video.Genre, this.genre);
        FillFrom(video.ScreenWriters, this.screenWriters);
        FillFrom(video.Cast, this.cast);
        FillFrom(video.Composers, this.composers);
        this.Release = video is { Release: { Ticks: not 0 } videoRelease } ? new DateTimeOffset(videoRelease) : default(DateTimeOffset?);
        this.Rating.SelectedRating = video.Rating;
        this.Work = video.Work;
        this.Image = video.Image;
        this.ImageFormat = video.ImageFormat;

        if (video is Models.IHasImageSource hasImageSource)
        {
            this.ImageSource = hasImageSource.ImageSource;
        }
    }

    /// <summary>
    /// Disposes this instance.
    /// </summary>
    /// <param name="disposing">Set to <see langword="true"/> to dispose managed resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposedValue)
        {
            if (disposing)
            {
                if (this.imageSource is IDisposable imageSourceDisposable)
                {
                    imageSourceDisposable.Dispose();
                }

                this.imageSource = default;

                if (this.image is IDisposable imageDisposable)
                {
                    imageDisposable.Dispose();
                }

                this.image = default;
            }

            this.disposedValue = true;
        }
    }

    /// <summary>
    /// Disposes this instance asynchronously.
    /// </summary>
    /// <returns>The value task.</returns>
    protected virtual async ValueTask DisposeAsyncCore()
    {
        await DisposeAsync(this.imageSource).ConfigureAwait(false);
        this.imageSource = default;
        await DisposeAsync(this.image).ConfigureAwait(false);
        this.image = default;

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

    private static void FillFrom<T>(IEnumerable<T>? source, System.Collections.ObjectModel.ObservableCollection<T> destination)
    {
        destination.Clear();
        if (source is not null)
        {
            foreach (var item in source)
            {
                destination.Add(item);
            }
        }
    }
}