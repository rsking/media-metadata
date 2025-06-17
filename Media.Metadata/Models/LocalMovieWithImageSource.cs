// -----------------------------------------------------------------------
// <copyright file="LocalMovieWithImageSource.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Media.Metadata.Models;

/// <summary>
/// A movie with an <see cref="ImageSource"/>.
/// </summary>
/// <inheritdoc />
internal partial record class LocalMovieWithImageSource(
    FileInfo FileInfo,
    string? Name,
    string? Description,
    IEnumerable<string>? Producers,
    IEnumerable<string>? Directors,
    IEnumerable<string>? Studios,
    IEnumerable<string>? Genre,
    IEnumerable<string>? ScreenWriters,
    IEnumerable<string>? Cast,
    IEnumerable<string>? Composers) : MovieWithImageSource(Name, Description, Producers, Directors, Studios, Genre, ScreenWriters, Cast, Composers), ILocalVideo
{
    /// <summary>
    /// Initialises a new instance of the <see cref="LocalMovieWithImageSource"/> class.
    /// </summary>
    /// <param name="fileInfo">The file info.</param>
    /// <param name="movie">The movie.</param>
    public LocalMovieWithImageSource(FileInfo fileInfo, MovieWithImageSource movie)
        : this(fileInfo, movie.Name, movie.Description, movie.Producers, movie.Directors, movie.Studios, movie.Genre, movie.ScreenWriters, movie.Cast, movie.Composers)
    {
        this.Release = movie.Release;
        this.Rating = movie.Rating;
        this.Tracks = movie.Tracks;
        this.Work = movie.Work;
        this.Edition = movie.Edition;
        this.Image = movie.Image;
        this.ImageFormat = movie.ImageFormat;
        this.ImageSource = movie.ImageSource;
    }

    /// <summary>
    /// Creates a <see cref="MovieWithImageSource"/> from a <see cref="Movie"/>.
    /// </summary>
    /// <param name="movie">The movie.</param>
    /// <returns>The video with image source.</returns>
    public static async Task<LocalMovieWithImageSource> CreateAsync(LocalMovie movie) => new LocalMovieWithImageSource(movie.FileInfo, await MovieWithImageSource.CreateAsync(movie).ConfigureAwait(false));
}