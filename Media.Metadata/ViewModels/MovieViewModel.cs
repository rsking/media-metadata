// -----------------------------------------------------------------------
// <copyright file="MovieViewModel.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Media.Metadata.ViewModels;

/// <summary>
/// An editable <see cref="Movie"/>.
/// </summary>
/// <param name="movie">The movie.</param>
internal sealed partial class MovieViewModel(Models.LocalMovieWithImageSource movie) : VideoViewModel(movie, movie.FileInfo, movie.Image, movie.ImageFormat, movie.ImageSource)
{
    /// <summary>
    /// Gets or sets the edition.
    /// </summary>
    [CommunityToolkit.Mvvm.ComponentModel.ObservableProperty]
    public partial string? Edition { get; set; } = movie.Edition;

    /// <inheritdoc/>
    public override Task<Video> ToVideoAsync(CancellationToken cancellationToken = default) => Task.FromResult<Video>(
        new LocalMovie(this.FileInfo, this.Name, this.Description, this.Producers, this.Directors, this.Studios, this.Genre, this.ScreenWriters, this.Cast, this.Composers)
        {
            Rating = this.Rating.SelectedRating,
            Release = this.Release?.DateTime,
            Work = this.Work,
            Tracks = [.. this.Tracks.Select(track => track.ToMediaTrack())],
            Edition = this.Edition,
            Image = this.Image,
            ImageFormat = this.ImageFormat,
        });

    /// <inheritdoc/>
    public override void Update(Video video)
    {
        base.Update(video);
        if (video is Movie videoAsMovie)
        {
            this.Edition = videoAsMovie.Edition;
        }
    }
}