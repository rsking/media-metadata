// -----------------------------------------------------------------------
// <copyright file="Rating.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Media.Metadata;

/// <summary>
/// Represents the rating.
/// </summary>
/// <param name="Standard">The standard.</param>
/// <param name="ContentRating">The content rating.</param>
/// <param name="Score">The score.</param>
/// <param name="Annotation">The annotation.</param>
public readonly record struct Rating(string Standard, string ContentRating, int Score = default, string? Annotation = default)
{
    private static readonly Dictionary<Country, ILookup<RatingType, Rating>> Ratings = LoadRatings();

    /// <summary>
    /// Parses a <see cref="Rating"/> from a tag.
    /// </summary>
    /// <param name="tag">The tag.</param>
    /// <param name="rating">The rating.</param>
    /// <returns><see langword="true"/> if the parsing was successful; otherwise <see langword="false"/>.</returns>
    public static bool TryParse(string tag, out Rating rating)
    {
        if (tag is null)
        {
            rating = default;
            return default;
        }

        if (tag.Split('|') is { Length: >= 3 } split)
        {
            var score = int.TryParse(split[2], System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out var result)
                ? result
                : 0;
            var annotation = split.Length > 3
                ? split[3]
                : default;
            rating = new Rating(split[0], split[1], score, string.IsNullOrEmpty(annotation) ? default : annotation);
            return true;
        }

        rating = default;
        return default;
    }

    /// <summary>
    /// Finds the best rating by country and type.
    /// </summary>
    /// <param name="rating">The rating.</param>
    /// <param name="country">The country.</param>
    /// <param name="type">The type.</param>
    /// <returns>The best rating.</returns>
    public static Rating? FindBest(string rating, Country? country = default, RatingType type = default)
    {
        rating = RemoveSpaces(rating);
        return GetRatings(country ?? Country.UnitedStates, type).FirstOrDefault(type => string.Equals(RemoveSpaces(type.ContentRating), rating, StringComparison.OrdinalIgnoreCase));

        static string RemoveSpaces(string value)
        {
            return value.Replace(" ", string.Empty);
        }
    }

    /// <summary>
    /// Gets the country.
    /// </summary>
    /// <param name="rating">The rating to get the country from.</param>
    /// <returns>The country.</returns>
    public static Country? GetCountry(Rating? rating) =>
        rating is { } ratingValue && Ratings.FirstOrDefault(country => country.Value.Any(type => type.Any(r => r == ratingValue))) is { Key: { } country }
            ? country
            : default;

    /// <summary>
    /// Gets the ratings for the country.
    /// </summary>
    /// <param name="country">The country.</param>
    /// <param name="type">The type.</param>
    /// <returns>The ratings.</returns>
    public static IEnumerable<Rating> GetRatings(Country country, RatingType type = default) => (Ratings.TryGetValue(country, out var value), type) switch
    {
        (true, RatingType.None) => value.SelectMany(group => group),
        (true, var t) => value[t],
        _ => [],
    };

    /// <inheritdoc/>
    public override string ToString() => FormattableString.Invariant($"{this.Standard}|{this.ContentRating}|{this.Score}|{this.Annotation ?? string.Empty}");

    private static Dictionary<Country, ILookup<RatingType, Rating>> LoadRatings()
    {
        return GetRatings()
            .ToLookup(tuple => tuple.Country)
            .ToDictionary(
                group => group.Key,
                group => group.ToLookup(
                    tuple => tuple.Type,
                    tuple => new Rating(tuple.Key, tuple.Rating, tuple.Score)));

        static IEnumerable<(Country Country, RatingType Type, string Key, int Score, string Rating)> GetRatings()
        {
            yield return (Country.Australia, RatingType.Movie, "au-movie", 0, "Not Rated");
            yield return (Country.Australia, RatingType.Movie, "au-movie", 100, "G");
            yield return (Country.Australia, RatingType.Movie, "au-movie", 200, "PG");
            yield return (Country.Australia, RatingType.Movie, "au-movie", 350, "M");
            yield return (Country.Australia, RatingType.Movie, "au-movie", 375, "MA 15+");
            yield return (Country.Australia, RatingType.Movie, "au-movie", 400, "R18+");
            yield return (Country.Canada, RatingType.Movie, "ca-movie", 0, "Not Rated");
            yield return (Country.Canada, RatingType.Movie, "ca-movie", 100, "G");
            yield return (Country.Canada, RatingType.Movie, "ca-movie", 200, "PG");
            yield return (Country.Canada, RatingType.Movie, "ca-movie", 325, "14");
            yield return (Country.Canada, RatingType.Movie, "ca-movie", 400, "18");
            yield return (Country.Canada, RatingType.Movie, "ca-movie", 500, "R");
            yield return (Country.Canada, RatingType.TV, "ca-tv", 0, "Not Rated");
            yield return (Country.Canada, RatingType.TV, "ca-tv", 100, "C");
            yield return (Country.Canada, RatingType.TV, "ca-tv", 200, "C8");
            yield return (Country.Canada, RatingType.TV, "ca-tv", 300, "G");
            yield return (Country.Canada, RatingType.TV, "ca-tv", 400, "PG");
            yield return (Country.Canada, RatingType.TV, "ca-tv", 500, "14+");
            yield return (Country.Canada, RatingType.TV, "ca-tv", 600, "18+");
            yield return (Country.France, RatingType.TV, "fr-tv", 0, "Not Rated");
            yield return (Country.France, RatingType.TV, "fr-tv", 100, "-10");
            yield return (Country.France, RatingType.TV, "fr-tv", 200, "-12");
            yield return (Country.France, RatingType.TV, "fr-tv", 500, "-16");
            yield return (Country.France, RatingType.TV, "fr-tv", 600, "-18");
            yield return (Country.Germany, RatingType.TV, "de-tv", 0, "Not Rated");
            yield return (Country.Germany, RatingType.TV, "de-tv", 100, "ab 6 Jahren");
            yield return (Country.Germany, RatingType.TV, "de-tv", 200, "ab 12 Jahren");
            yield return (Country.Germany, RatingType.TV, "de-tv", 500, "ab 16 Jahren");
            yield return (Country.Germany, RatingType.TV, "de-tv", 600, "ab 18 Jahren");
            yield return (Country.NewZealand, RatingType.Movie, "nz-movie", 0, "Not Rated");
            yield return (Country.NewZealand, RatingType.Movie, "nz-movie", 100, "G");
            yield return (Country.NewZealand, RatingType.Movie, "nz-movie", 200, "PG");
            yield return (Country.NewZealand, RatingType.Movie, "nz-movie", 300, "M");
            yield return (Country.NewZealand, RatingType.Movie, "nz-movie", 325, "R13");
            yield return (Country.NewZealand, RatingType.Movie, "nz-movie", 350, "R15");
            yield return (Country.NewZealand, RatingType.Movie, "nz-movie", 375, "R16");
            yield return (Country.NewZealand, RatingType.Movie, "nz-movie", 400, "R18");
            yield return (Country.NewZealand, RatingType.Movie, "nz-movie", 500, "R");
            yield return (Country.GreatBritan, RatingType.Movie, "uk-movie", 0, "Not Rated");
            yield return (Country.GreatBritan, RatingType.Movie, "uk-movie", 100, "U");
            yield return (Country.GreatBritan, RatingType.Movie, "uk-movie", 150, "Uc");
            yield return (Country.GreatBritan, RatingType.Movie, "uk-movie", 200, "PG");
            yield return (Country.GreatBritan, RatingType.Movie, "uk-movie", 300, "12");
            yield return (Country.GreatBritan, RatingType.Movie, "uk-movie", 325, "12A");
            yield return (Country.GreatBritan, RatingType.Movie, "uk-movie", 350, "15");
            yield return (Country.GreatBritan, RatingType.Movie, "uk-movie", 400, "18");
            yield return (Country.GreatBritan, RatingType.Movie, "uk-movie", 600, "E");
            yield return (Country.GreatBritan, RatingType.TV, "uk-tv", 0, "Not Rated");
            yield return (Country.GreatBritan, RatingType.TV, "uk-tv", 500, "CAUTION");
            yield return (Country.UnitedStates, RatingType.Movie, "mpaa", 0, "Not Rated");
            yield return (Country.UnitedStates, RatingType.Movie, "mpaa", 100, "G");
            yield return (Country.UnitedStates, RatingType.Movie, "mpaa", 200, "PG");
            yield return (Country.UnitedStates, RatingType.Movie, "mpaa", 300, "PG-13");
            yield return (Country.UnitedStates, RatingType.Movie, "mpaa", 400, "R");
            yield return (Country.UnitedStates, RatingType.Movie, "mpaa", 500, "NC-17");
            yield return (Country.UnitedStates, RatingType.TV, "us-tv", 0, "Not Rated");
            yield return (Country.UnitedStates, RatingType.TV, "us-tv", 100, "TV-Y");
            yield return (Country.UnitedStates, RatingType.TV, "us-tv", 200, "TV-Y7");
            yield return (Country.UnitedStates, RatingType.TV, "us-tv", 300, "TV-G");
            yield return (Country.UnitedStates, RatingType.TV, "us-tv", 400, "TV-PG");
            yield return (Country.UnitedStates, RatingType.TV, "us-tv", 500, "TV-14");
            yield return (Country.UnitedStates, RatingType.TV, "us-tv", 600, "TV-MA");
        }
    }
}