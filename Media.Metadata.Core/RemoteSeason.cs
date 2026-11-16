// -----------------------------------------------------------------------
// <copyright file="RemoteSeason.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Media.Metadata;

/// <inheritdoc cref="Season"/>
public record class RemoteSeason(int Number, IEnumerable<RemoteEpisode> RemoteEpisodes) : Season(Number, RemoteEpisodes), IRemoteImage
{
    /// <inheritdoc/>
    public Uri? ImageUri { get; init; }

    /// <inheritdoc/>
    protected override ValueTask<(Image Image, SixLabors.ImageSharp.Formats.IImageFormat ImageFormat)> GetImageAsync() => this.DownloadImageAsync();
}