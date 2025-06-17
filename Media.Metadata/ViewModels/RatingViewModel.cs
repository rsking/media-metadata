// -----------------------------------------------------------------------
// <copyright file="RatingViewModel.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Media.Metadata.ViewModels;

/// <summary>
/// An editable rating.
/// </summary>
/// <param name="rating">The rating.</param>
internal sealed partial class RatingViewModel(Rating? rating) : CommunityToolkit.Mvvm.ComponentModel.ObservableObject
{
    private readonly System.Collections.ObjectModel.ObservableCollection<Rating> ratings = [.. GetInitialRatings(rating)];

    /// <summary>
    /// Gets or sets the selected country.
    /// </summary>
    [CommunityToolkit.Mvvm.ComponentModel.ObservableProperty]
    public partial Country? SelectedCountry { get; set; } = Rating.GetCountry(rating);

    /// <summary>
    /// Gets or sets the selected rating.
    /// </summary>
    [CommunityToolkit.Mvvm.ComponentModel.ObservableProperty]
    public partial Rating? SelectedRating { get; set; } = rating;

    /// <summary>
    /// Gets the countries.
    /// </summary>
    public IEnumerable<Country> Countries { get; } = Country.All;

    /// <summary>
    /// Gets the ratings.
    /// </summary>
    public IEnumerable<Rating> Ratings => this.ratings;

    private static IEnumerable<Rating> GetInitialRatings(Rating? rating) => Rating.GetCountry(rating) is { } country ? Rating.GetRatings(country) : [];

    partial void OnSelectedCountryChanged(Country? value)
    {
        this.ratings.Clear();
        if (value is { } country)
        {
            foreach (var ratingToAdd in Rating.GetRatings(country))
            {
                this.ratings.Add(ratingToAdd);
            }
        }
    }

    partial void OnSelectedRatingChanging(Rating? value)
    {
        if (value is { } valueAsRating)
        {
            // set the country
            this.SelectedCountry = Rating.GetCountry(valueAsRating);
        }
    }
}