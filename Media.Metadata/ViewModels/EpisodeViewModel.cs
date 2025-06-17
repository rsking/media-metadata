// -----------------------------------------------------------------------
// <copyright file="EpisodeViewModel.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Media.Metadata.ViewModels;

/// <summary>
/// An editable <see cref="Episode"/>.
/// </summary>
internal sealed partial class EpisodeViewModel : VideoViewModel
{
    [CommunityToolkit.Mvvm.ComponentModel.ObservableProperty]
    private string? show;

    [CommunityToolkit.Mvvm.ComponentModel.ObservableProperty]
    private string? network;

    [CommunityToolkit.Mvvm.ComponentModel.ObservableProperty]
    private int? season;

    [CommunityToolkit.Mvvm.ComponentModel.ObservableProperty]
    private int? number;

    [CommunityToolkit.Mvvm.ComponentModel.ObservableProperty]
    private string? id;

    [CommunityToolkit.Mvvm.ComponentModel.ObservableProperty]
    private int? part;

    /// <summary>
    /// Initialises a new instance of the <see cref="EpisodeViewModel"/> class.
    /// </summary>
    /// <param name="episode">The episode.</param>
    public EpisodeViewModel(Models.LocalEpisodeWithImageSource episode)
        : base(episode, episode.FileInfo, episode.Image, episode.ImageFormat, episode.ImageSource)
    {
        this.Show = episode.Show;
        this.Network = episode.Network;
        this.Season = episode.Season;
        this.Number = episode.Number;
        this.Id = episode.Id;
        this.Part = episode.Part;
    }

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

        if (video is Episode episode)
        {
            this.Show = episode.Show;
            this.Network = episode.Network;
            this.Season = episode.Season;
            this.Number = episode.Number;
            this.Id = episode.Id;
            this.Part = episode.Part;
        }
    }
}