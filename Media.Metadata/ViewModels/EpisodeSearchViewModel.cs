// -----------------------------------------------------------------------
// <copyright file="EpisodeSearchViewModel.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Media.Metadata.ViewModels;

/// <summary>
/// The <see cref="Episode"/> <see cref="VideoSearchViewModel"/>.
/// </summary>
/// <param name="showSearch">The series search.</param>
internal sealed partial class EpisodeSearchViewModel(IShowSearch showSearch) : VideoSearchViewModel
{
    private readonly System.Collections.ObjectModel.ObservableCollection<Series> series = [];

    private readonly System.Collections.ObjectModel.ObservableCollection<Season> seasons = [];

    private readonly IShowSearch showSearch = showSearch;

    [CommunityToolkit.Mvvm.ComponentModel.ObservableProperty]
    private Series? selectedSeries;

    [CommunityToolkit.Mvvm.ComponentModel.ObservableProperty]
    private Season? selectedSeason;

    /// <summary>
    /// Gets the series.
    /// </summary>
    public IEnumerable<Series> Series => this.series;

    /// <summary>
    /// Gets the seasons.
    /// </summary>
    public IEnumerable<Season> Seasons => this.seasons;

    /// <summary>
    /// Gets or sets the year.
    /// </summary>
    public int? Year { get; set; }

    /// <summary>
    /// Searches for the episode.
    /// </summary>
    /// <returns>The task.</returns>
    [CommunityToolkit.Mvvm.Input.RelayCommand(AllowConcurrentExecutions = false)]
    public async Task SearchSeries()
    {
        this.series.Clear();
        await foreach (var s in this.showSearch.SearchAsync(this.Name!, this.Year ?? 0, this.SelectedCountry?.Abbreviation ?? Country.Australia.Abbreviation).ConfigureAwait(true))
        {
            this.series.Add(s);
        }
    }
}