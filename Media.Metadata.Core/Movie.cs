// -----------------------------------------------------------------------
// <copyright file="Movie.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Media.Metadata;

/// <summary>
/// Represents a movie.
/// </summary>
/// <param name="Name">The name.</param>
/// <param name="Description">The description.</param>
/// <param name="Producers">The producers.</param>
/// <param name="Directors">The directors.</param>
/// <param name="Studios">The studios.</param>
/// <param name="Genre">The genre.</param>
/// <param name="ScreenWriters">The screenwriters.</param>
/// <param name="Cast">The cast.</param>
/// <param name="Composers">The composers.</param>
public abstract record class Movie(
    string? Name,
    string? Description,
    IEnumerable<string>? Producers,
    IEnumerable<string>? Directors,
    IEnumerable<string>? Studios,
    IEnumerable<string>? Genre,
    IEnumerable<string>? ScreenWriters,
    IEnumerable<string>? Cast,
    IEnumerable<string>? Composers) : Video(Name, Description, Producers, Directors, Studios, Genre, ScreenWriters, Cast, Composers)
{
    /// <summary>
    /// Gets the edition.
    /// </summary>
    public string? Edition { get; init; }
}