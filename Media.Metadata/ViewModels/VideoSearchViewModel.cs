// -----------------------------------------------------------------------
// <copyright file="VideoSearchViewModel.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Media.Metadata.ViewModels;

using CommunityToolkit.WinUI;

/// <summary>
/// The <see cref="Video"/> <see cref="VideoSearchViewModel"/>.
/// </summary>
internal abstract partial class VideoSearchViewModel : CommunityToolkit.Mvvm.ComponentModel.ObservableObject
{
    private readonly System.Collections.ObjectModel.ObservableCollection<Video> videos = [];

    /// <summary>
    /// Gets or sets the selected video.
    /// </summary>
    [CommunityToolkit.Mvvm.ComponentModel.ObservableProperty]
    public partial Video? SelectedVideo { get; set; }

    /// <summary>
    /// Gets or sets the selected country.
    /// </summary>
    [CommunityToolkit.Mvvm.ComponentModel.ObservableProperty]
    public partial Country? SelectedCountry { get; set; } = Country.Australia;

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets the countries.
    /// </summary>
    public IEnumerable<Country> Countries { get; } = Country.All;

    /// <summary>
    /// Gets the videos.
    /// </summary>
    public IEnumerable<Video> Videos => this.videos;

    /// <summary>
    /// Sets the videos.
    /// </summary>
    /// <param name="videos">The videos.</param>
    /// <returns>The task.</returns>
    protected async Task SetVideos(IAsyncEnumerable<Video> videos)
    {
        this.videos.Clear();
        var dispatcher = Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread();
        await foreach (var video in videos.ConfigureAwait(true))
        {
            await dispatcher.EnqueueAsync(async () => this.videos.Add(await Models.VideoWithImageSource.CreateAsync(video).ConfigureAwait(true))).ConfigureAwait(true);
        }
    }
}