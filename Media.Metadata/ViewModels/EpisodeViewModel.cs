// -----------------------------------------------------------------------
// <copyright file="EpisodeViewModel.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Media.Metadata.ViewModels;

/// <summary>
/// An editable <see cref="Episode"/>.
/// </summary>
/// <param name="episode">The episode.</param>
internal sealed partial class EpisodeViewModel(Models.LocalEpisodeWithImageSource episode) : VideoViewModel(episode, episode.FileInfo, episode.Image, episode.ImageFormat, episode.ImageSource)
{
    /// <summary>
    /// Gets or sets the show.
    /// </summary>
    [CommunityToolkit.Mvvm.ComponentModel.ObservableProperty]
    public partial string? Show { get; set; } = episode.Show;

    /// <summary>
    /// Gets or sets the network.
    /// </summary>
    [CommunityToolkit.Mvvm.ComponentModel.ObservableProperty]
    public partial string? Network { get; set; } = episode.Network;

    /// <summary>
    /// Gets or sets the season.
    /// </summary>
    [CommunityToolkit.Mvvm.ComponentModel.ObservableProperty]
    public partial int? Season { get; set; } = episode.Season;

    /// <summary>
    /// Gets or sets the number.
    /// </summary>
    [CommunityToolkit.Mvvm.ComponentModel.ObservableProperty]
    public partial int? Number { get; set; } = episode.Number;

    /// <summary>
    /// Gets or sets the ID.
    /// </summary>
    [CommunityToolkit.Mvvm.ComponentModel.ObservableProperty]
    public partial string? Id { get; set; } = episode.Id;

    /// <summary>
    /// Gets or sets the part.
    /// </summary>
    [CommunityToolkit.Mvvm.ComponentModel.ObservableProperty]
    public partial int? Part { get; set; } = episode.Part;

    /// <inheritdoc/>
    public override Task<Video> ToVideoAsync(CancellationToken cancellationToken = default) => Task.FromResult<Video>(
        new LocalEpisode(this.FileInfo, this.Name, this.Description, this.Producers, this.Directors, this.Studios, this.Genre, this.ScreenWriters, this.Cast, this.Composers)
        {
            Rating = this.Rating.SelectedRating,
            Release = this.Release?.DateTime,
            Work = this.Work,
            Show = this.Show,
            Network = this.Network,
            Season = this.Season,
            Number = this.Number,
            Id = this.Id,
            Part = this.Part,
            Tracks = [.. this.Tracks.Select(track => track.ToMediaTrack())],
            Image = this.Image,
            ImageFormat = this.ImageFormat,
        });

    /// <inheritdoc/>
    public override void Update(Video video)
    {
        base.Update(video);

        if (video is Episode videoAsEpisode)
        {
            this.Show = videoAsEpisode.Show;
            this.Network = videoAsEpisode.Network;
            this.Season = videoAsEpisode.Season;
            this.Number = videoAsEpisode.Number;
            this.Id = videoAsEpisode.Id;
            this.Part = videoAsEpisode.Part;
        }
    }
}