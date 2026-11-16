// -----------------------------------------------------------------------
// <copyright file="Episode.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Media.Metadata;

/// <summary>
/// Represents an episode.
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
public abstract record Episode(
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
    /// Gets the show.
    /// </summary>
    public string? Show { get; init; }

    /// <summary>
    /// Gets the network.
    /// </summary>
    public string? Network { get; init; }

    /// <summary>
    /// Gets the season.
    /// </summary>
    public int? Season { get; init; }

    /// <summary>
    /// Gets the number.
    /// </summary>
    public int? Number { get; init; }

    /// <summary>
    /// Gets the ID.
    /// </summary>
    public string? Id { get; init; }

    /// <summary>
    /// Gets the part.
    /// </summary>
    public int? Part { get; init; }
}