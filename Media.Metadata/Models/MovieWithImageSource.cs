// -----------------------------------------------------------------------
// <copyright file="MovieWithImageSource.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Media.Metadata.Models;

/// <summary>
/// A movie with an <see cref="ImageSource"/>.
/// </summary>
/// <inheritdoc />
internal partial record class MovieWithImageSource(
    string? Name,
    string? Description,
    IEnumerable<string>? Producers,
    IEnumerable<string>? Directors,
    IEnumerable<string>? Studios,
    IEnumerable<string>? Genre,
    IEnumerable<string>? ScreenWriters,
    IEnumerable<string>? Cast,
    IEnumerable<string>? Composers) : Movie(Name, Description, Producers, Directors, Studios, Genre, ScreenWriters, Cast, Composers), IHasImageSource
{
    /// <inheritdoc/>
    public ImageSource? ImageSource { get; init; }

    /// <summary>
    /// Creates a <see cref="MovieWithImageSource"/> from a <see cref="Movie"/>.
    /// </summary>
    /// <param name="movie">The movie.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The video with image source.</returns>
    public static async Task<MovieWithImageSource> CreateAsync(Movie movie, CancellationToken cancellationToken = default) => new MovieWithImageSource(movie.Name, movie.Description, movie.Producers, movie.Directors, movie.Studios, movie.Genre, movie.ScreenWriters, movie.Cast, movie.Composers)
    {
        Release = movie.Release,
        Rating = movie.Rating,
        Tracks = movie.Tracks,
        Work = movie.Work,
        Edition = movie.Edition,
        Image = movie.Image,
        ImageFormat = movie.ImageFormat,
        ImageSource = await movie.CreateImageSource(cancellationToken).ConfigureAwait(true),
    };
}