// -----------------------------------------------------------------------
// <copyright file="RatingViewModel.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Media.Metadata.ViewModels;

/// <summary>
/// An editable rating.
/// </summary>
internal sealed partial class RatingViewModel : CommunityToolkit.Mvvm.ComponentModel.ObservableObject
{
    private readonly System.Collections.ObjectModel.ObservableCollection<Rating> ratings = [..GetInitialRatings(rating)];

    [CommunityToolkit.Mvvm.ComponentModel.ObservableProperty]
    private Country? selectedCountry;

    [CommunityToolkit.Mvvm.ComponentModel.ObservableProperty]
    private Rating? selectedRating;

    /// <summary>
    /// Initialises a new instance of the <see cref="RatingViewModel"/> class.
    /// </summary>
    /// <param name="rating">The rating.</param>
    public RatingViewModel(Rating? rating)
    {
        this.Countries = Country.All;
        this.SelectedCountry = Rating.GetCountry(rating);
        this.SelectedRating = rating;
    }

    /// <summary>
    /// Gets the countries.
    /// </summary>
    public IEnumerable<Country> Countries { get; }

    /// <summary>
    /// Gets the ratings.
    /// </summary>
    public IEnumerable<Rating> Ratings => this.ratings;

    partial void OnSelectedCountryChanged(Country? value)
    {
        this.ratings.Clear();
        if (value is { } country)
        {
            foreach (var rating in Rating.GetRatings(country))
            {
                this.ratings.Add(rating);
            }
        }
    }

    partial void OnSelectedRatingChanging(Rating? value)
    {
        if (value is { } rating)
        {
            // set the country
            this.SelectedCountry = Rating.GetCountry(rating);
        }
    }
}