// -----------------------------------------------------------------------
// <copyright file="LocalVideo.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Media.Metadata;

/// <summary>
/// Represents a local video.
/// </summary>
/// <inheritdoc cref="Video" />
public record class LocalVideo(
    FileInfo FileInfo,
    string? Name,
    string? Description = default,
    IEnumerable<string>? Producers = default,
    IEnumerable<string>? Directors = default,
    IEnumerable<string>? Studios = default,
    IEnumerable<string>? Genre = default,
    IEnumerable<string>? ScreenWriters = default,
    IEnumerable<string>? Cast = default,
    IEnumerable<string>? Composers = default) : Video(Name, Description, Producers, Directors, Studios, Genre, ScreenWriters, Cast, Composers), ILocalVideo;