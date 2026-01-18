// -----------------------------------------------------------------------
// <copyright file="MainViewModel.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Media.Metadata.ViewModels;

using Microsoft.UI.Xaml.Controls;

/// <summary>
/// The main view model.
/// </summary>
/// <param name="reader">The reader.</param>
/// <param name="updater">The updater.</param>
internal sealed partial class MainViewModel(IReader reader, IUpdater updater) : CommunityToolkit.Mvvm.ComponentModel.ObservableObject
{
    private readonly IReader reader = reader;

    private readonly IUpdater updater = updater;

    /// <summary>
    /// Gets or sets the selected video.
    /// </summary>
    [CommunityToolkit.Mvvm.ComponentModel.ObservableProperty]
    [CommunityToolkit.Mvvm.ComponentModel.NotifyPropertyChangedFor(nameof(SelectedEditableVideo))]
    [CommunityToolkit.Mvvm.ComponentModel.NotifyPropertyChangedFor(nameof(CanSave))]
    [CommunityToolkit.Mvvm.ComponentModel.NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    [CommunityToolkit.Mvvm.ComponentModel.NotifyPropertyChangedFor(nameof(CanSearch))]
    [CommunityToolkit.Mvvm.ComponentModel.NotifyCanExecuteChangedFor(nameof(SearchCommand))]
    public partial Video? SelectedVideo { get; set; }

    /// <summary>
    /// Gets a value indicating whether this instance is saving.
    /// </summary>
    [CommunityToolkit.Mvvm.ComponentModel.ObservableProperty]
    [CommunityToolkit.Mvvm.ComponentModel.NotifyPropertyChangedFor(nameof(CanSave))]
    [CommunityToolkit.Mvvm.ComponentModel.NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    [CommunityToolkit.Mvvm.ComponentModel.NotifyPropertyChangedFor(nameof(CanSearch))]
    [CommunityToolkit.Mvvm.ComponentModel.NotifyCanExecuteChangedFor(nameof(SearchCommand))]
    public partial bool IsSaving { get; private set; }

#pragma warning disable IDE0028 // Simplify collection initialization
    /// <summary>
    /// Gets the videos.
    /// </summary>
    public IList<Video> Videos { get; } = new System.Collections.ObjectModel.ObservableCollection<Video>();

    /// <summary>
    /// Gets the selected videos.
    /// </summary>
    public IList<Video> SelectedVideos { get; } = new System.Collections.ObjectModel.ObservableCollection<Video>();
#pragma warning restore IDE0028 // Simplify collection initialization

    /// <summary>
    /// Gets the selected editable video.
    /// </summary>
    public VideoViewModel? SelectedEditableVideo { get; private set; }

    /// <summary>
    /// Gets a value indicating whether this instance can save.
    /// </summary>
    public bool CanSave => !this.IsSaving && this.SelectedEditableVideo is not null;

    /// <summary>
    /// Gets a value indicating whether this instance can search.
    /// </summary>
    public bool CanSearch => !this.IsSaving && this.SelectedEditableVideo is not null;

    /// <summary>
    /// Adds videos.
    /// </summary>
    /// <returns>The task.</returns>
    [CommunityToolkit.Mvvm.Input.RelayCommand]
    public async Task AddVideos()
    {
        // Open a text file.
        var picker = new Windows.Storage.Pickers.FileOpenPicker
        {
            SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.VideosLibrary,
            ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail,
            FileTypeFilter = { ".mp4", ".m4v", "*" },
        };

        picker.Init();

        if (await picker.PickMultipleFilesAsync() is { } files)
        {
            foreach (var file in files)
            {
                if (await this.ReadVideoAsync(file.Path).ConfigureAwait(true) is { } video)
                {
                    this.Videos.Add(video);
                }
            }
        }
    }

    /// <summary>
    /// Removes the selected video.
    /// </summary>
    /// <returns>The task.</returns>
    [CommunityToolkit.Mvvm.Input.RelayCommand]
    public async Task RemoveVideo()
    {
        var selectedVideos = (this.SelectedVideos, this.SelectedVideo) switch
        {
            ({ Count: > 0 }, _) => GetSelectedVideos(this.SelectedVideos),
            (_, Video v) => [v],
            _ => [],
        };

        foreach (var video in selectedVideos)
        {
            _ = this.Videos.Remove(video);
            await video.DisposeAsync().ConfigureAwait(true);
        }

        this.SelectedVideo = default;

        static IEnumerable<Video> GetSelectedVideos(ICollection<Video> videos)
        {
            return [.. videos];
        }
    }

    /// <summary>
    /// Clears the videos.
    /// </summary>
    /// <returns>The task.</returns>
    [CommunityToolkit.Mvvm.Input.RelayCommand]
    public async Task ClearVideos()
    {
        while (this.Videos.Count > 0)
        {
            var video = this.Videos[0];
            this.Videos.RemoveAt(0);
            await video.DisposeAsync().ConfigureAwait(true);
        }
    }

    /// <summary>
    /// Saves the current video.
    /// </summary>
    /// <returns>The task.</returns>
    [CommunityToolkit.Mvvm.Input.RelayCommand(AllowConcurrentExecutions = false, CanExecute = nameof(CanSave))]
    public async Task Save()
    {
        if (this.SelectedEditableVideo is { } selectedEditableVideo)
        {
            var video = await selectedEditableVideo.ToVideoAsync().ConfigureAwait(true);
            if (video is ILocalVideo localVideo)
            {
                this.IsSaving = true;
                await Task.Run(() => this.updater.UpdateVideo(localVideo.FileInfo.FullName, video, GetLanguages(video.Tracks))).ConfigureAwait(true);
                await Refresh(localVideo).ConfigureAwait(true);
                this.IsSaving = false;
            }

            static IDictionary<MediaTrackType, string> GetLanguages(IEnumerable<MediaTrack> tracks)
            {
                return tracks.ToDictionary(track => (MediaTrackType)track.Id, track => track.Language ?? "und");
            }

            // refresh the local video from the file
            async Task Refresh(ILocalVideo localVideo)
            {
                for (var i = 0; i < this.Videos.Count; i++)
                {
                    if (this.Videos[i] == this.SelectedVideo)
                    {
                        if (await this.ReadVideoAsync(localVideo.FileInfo.FullName).ConfigureAwait(true) is { } video)
                        {
                            if (this.Videos[i] is IAsyncDisposable asyncDisposable)
                            {
                                await asyncDisposable.DisposeAsync().ConfigureAwait(true);
                            }
                            else if (this.Videos[i] is IDisposable disposable)
                            {
                                disposable.Dispose();
                            }

                            this.SelectedVideo = this.Videos[i] = video;
                        }

                        break;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Searches for the current video.
    /// </summary>
    /// <returns>The task.</returns>
    [CommunityToolkit.Mvvm.Input.RelayCommand(AllowConcurrentExecutions = false, CanExecute = nameof(CanSearch))]
    public async Task Search()
    {
        VideoSearchViewModel? viewModel = this.SelectedEditableVideo switch
        {
            MovieViewModel movie => new MovieSearchViewModel(CommunityToolkit.Mvvm.DependencyInjection.Ioc.Default.GetRequiredService<IMovieSearch>()) { Name = movie.Name, Year = GetYear(movie.Release) },
            EpisodeViewModel episode => new EpisodeSearchViewModel(CommunityToolkit.Mvvm.DependencyInjection.Ioc.Default.GetRequiredService<IShowSearch>()) { Name = episode.Show },
            _ => default,
        };

        object? view = viewModel switch
        {
            MovieSearchViewModel movieViewModel => new Views.MovieSearchView(movieViewModel),
            EpisodeSearchViewModel episodeViewModel => new Views.EpisodeSearchView(episodeViewModel),
            _ => default,
        };

        var dialog = new ContentDialog
        {
            Title = "Search",
            PrimaryButtonText = "Save",
            IsSecondaryButtonEnabled = false,
            CloseButtonText = "Cancel",
            DefaultButton = ContentDialogButton.Primary,
            Content = view,
            XamlRoot = GetXamlRoot(),
        };

        if (await dialog.ShowAsync(ContentDialogPlacement.Popup) is ContentDialogResult.Primary
            && this.SelectedEditableVideo is { } selectedEditableVideo
            && await GetVideoWithImageSource(viewModel).ConfigureAwait(true) is { } videoWithImageSource)
        {
            // apply this to the editable video
            selectedEditableVideo.Update(videoWithImageSource);
        }

        static int? GetYear(DateTimeOffset? dateTimeOffset)
        {
            return dateTimeOffset?.Year;
        }

        static Microsoft.UI.Xaml.XamlRoot? GetXamlRoot()
        {
            return App.Current?.GetWindow() is { } window
                ? window.Content.XamlRoot
                : default;
        }

        static async Task<Video?> GetVideoWithImageSource(VideoSearchViewModel? viewModel)
        {
            return viewModel?.SelectedVideo switch
            {
                Episode episode => await Models.EpisodeWithImageSource.CreateAsync(episode).ConfigureAwait(true),
                Movie movie => await Models.MovieWithImageSource.CreateAsync(movie).ConfigureAwait(true),
                Video video => await Models.VideoWithImageSource.CreateAsync(video).ConfigureAwait(true),
                _ => default,
            };
        }
    }

    /// <summary>
    /// Sets the editable video.
    /// </summary>
    /// <param name="value">The video to edit.</param>
    internal void SetEditableVideo(Video? value) => this.SelectedEditableVideo = value switch
    {
        Models.LocalEpisodeWithImageSource episode => new EpisodeViewModel(episode),
        Models.LocalMovieWithImageSource movie => new MovieViewModel(movie),
        Models.LocalVideoWithImageSource video => new VideoViewModel(video),
        _ => default,
    };

    private async Task<Video?> ReadVideoAsync(string path) => this.reader.ReadVideo(path) switch
    {
        LocalMovie movie => await Models.LocalMovieWithImageSource.CreateAsync(movie).ConfigureAwait(true),
        LocalEpisode episode => await Models.LocalEpisodeWithImageSource.CreateAsync(episode).ConfigureAwait(true),
        LocalVideo video => await Models.LocalVideoWithImageSource.CreateAsync(video).ConfigureAwait(true),
        _ => default,
    };
}