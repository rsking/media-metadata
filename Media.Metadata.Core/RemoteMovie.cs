// -----------------------------------------------------------------------
// <copyright file="RemoteMovie.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Media.Metadata;

/// <summary>
/// Represents a remote movie.
/// </summary>
/// <inheritdoc cref="Movie" />
public record class RemoteMovie(
    string? Name,
    string? Description,
    IEnumerable<string>? Producers,
    IEnumerable<string>? Directors,
    IEnumerable<string>? Studios,
    IEnumerable<string>? Genre,
    IEnumerable<string>? ScreenWriters,
    IEnumerable<string>? Cast,
    IEnumerable<string>? Composers) : Movie(Name, Description, Producers, Directors, Studios, Genre, ScreenWriters, Cast, Composers), IRemoteImage
{
    /// <summary>
    /// Gets the image URI.
    /// </summary>
    public Uri? ImageUri { get; init; }

    /// <inheritdoc />
    protected override ValueTask<(Image Image, SixLabors.ImageSharp.Formats.IImageFormat ImageFormat)> GetImageAsync() => this.DownloadImageAsync();
}