// -----------------------------------------------------------------------
// <copyright file="TMDbMovieSearch.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Media.Metadata.TMDb;

/// <summary>
/// The Movie DB move search.
/// </summary>
public class TMDbMovieSearch : IMovieSearch
{
    /// <inheritdoc/>
    public async IAsyncEnumerable<Movie> SearchAsync(string name, int year = 0, string country = "AU", [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        using var client = new TMDbLib.Client.TMDbClient("b866acbf3519560a3879b55d2d9cb1cb")
        {
            DefaultCountry = country,
            DefaultLanguage = "en",
            DefaultImageLanguage = "en",
        };

        var movies = await client.SearchMovieAsync(name, year: year, cancellationToken: cancellationToken).ConfigureAwait(false);

        if (movies?.Results is null)
        {
            yield break;
        }

        foreach (var result in movies.Results)
        {
            var movie = await client.GetMovieAsync(result.Id, TMDbLib.Objects.Movies.MovieMethods.Credits | TMDbLib.Objects.Movies.MovieMethods.ReleaseDates | TMDbLib.Objects.Movies.MovieMethods.Images, cancellationToken).ConfigureAwait(false);
            if (!client.HasConfig)
            {
                _ = await client.GetConfigAsync().ConfigureAwait(false);
            }

            if (movie?.ReleaseDates?.Results is null)
            {
                continue;
            }

            var poster = movie.Images?.Posters?.FirstOrDefault();
            Rating? countryRating = default;
            Rating? invariantRating = default;
            Rating? usRating = default;

            var releases = movie.ReleaseDates.Results.SelectMany(release => release.ReleaseDates?.Select(r => (release.Iso_3166_1, r.Certification)) ?? []);

            foreach (var (iso, certification) in releases)
            {
                if (iso is null || certification is null)
                {
                    continue;
                }

                if (!invariantRating.HasValue)
                {
                    invariantRating = Rating.FindBest(certification, iso);
                }

                if (country is { } c && string.Equals(iso, c, StringComparison.InvariantCultureIgnoreCase))
                {
                    countryRating = Rating.FindBest(certification, iso);
                }

                if (string.Equals(iso, "us", StringComparison.InvariantCultureIgnoreCase))
                {
                    usRating = Rating.FindBest(certification, iso);
                }
            }

            var rating = countryRating ?? usRating ?? invariantRating;
            if (rating is not null && rating.Value.ContentRating is null)
            {
                rating = default;
            }

            yield return new RemoteMovie(
                movie.Title,
                movie.Overview,
                GetProducers(movie.Credits?.Crew),
                GetDirectors(movie.Credits?.Crew),
                movie.ProductionCompanies?.Select(productionCompany => productionCompany.Name).WhereNotNull().Materialize(),
                movie.Genres?.Select(genre => genre.Name).WhereNotNull().Materialize(),
                GetWriters(movie.Credits?.Crew),
                movie.Credits?.Cast?.Select(cast => cast.Name).WhereNotNull().Materialize(),
                GetComposers(movie.Credits?.Crew))
            {
                Release = movie.ReleaseDate,
                Rating = rating,
                ImageUri = poster is { FilePath: { } filePath } ? client.GetImageUrl(GetBestPosterSize(client.Config) ?? "original", filePath) : null,
            };

            static ICollection<string> GetProducers(IEnumerable<TMDbLib.Objects.General.Crew>? crew)
            {
                return GetCrew(crew, crew => string.Equals(crew.Department, "Production", StringComparison.OrdinalIgnoreCase) && (string.Equals(crew.Job, "Producer", StringComparison.OrdinalIgnoreCase) || string.Equals(crew.Job, "Executive Producer", StringComparison.OrdinalIgnoreCase)));
            }

            static ICollection<string> GetDirectors(IEnumerable<TMDbLib.Objects.General.Crew>? crew)
            {
                return GetCrew(crew, crew => string.Equals(crew.Department, "Directing", StringComparison.OrdinalIgnoreCase) && string.Equals(crew.Job, "Director", StringComparison.OrdinalIgnoreCase));
            }

            static ICollection<string> GetWriters(IEnumerable<TMDbLib.Objects.General.Crew>? crew)
            {
                return GetCrew(crew, crew => string.Equals(crew.Department, "Writing", StringComparison.OrdinalIgnoreCase) && (string.Equals(crew.Job, "Screenplay", StringComparison.OrdinalIgnoreCase) || string.Equals(crew.Job, "Author", StringComparison.OrdinalIgnoreCase) || string.Equals(crew.Job, "Writer", StringComparison.OrdinalIgnoreCase)));
            }

            static ICollection<string> GetComposers(IEnumerable<TMDbLib.Objects.General.Crew>? crew)
            {
                return GetCrew(crew, crew => string.Equals(crew.Department, "Sound", StringComparison.OrdinalIgnoreCase) && string.Equals(crew.Job, "Original Music Composer", StringComparison.OrdinalIgnoreCase));
            }

            static ICollection<string> GetCrew(IEnumerable<TMDbLib.Objects.General.Crew>? crew, Predicate<TMDbLib.Objects.General.Crew> predicate)
            {
                return crew?.Where(crew => predicate(crew)).Select(crew => crew.Name).WhereNotNull() is { } c ? [.. c] : [];
            }
        }
    }

    private static string? GetBestPosterSize(TMDbLib.Objects.General.TMDbConfig configuration) => configuration.Images?.PosterSizes?.OrderDescending(SizeComparer.Instance).FirstOrDefault();

    private sealed class SizeComparer : IComparer<string>
    {
        public static IComparer<string> Instance { get; } = new SizeComparer();

        public int Compare(string? x, string? y)
        {
            if (x is null)
            {
                return y is null ? 0 : -1;
            }

            if (y is null)
            {
                return 1;
            }

            if (int.TryParse(x.TrimStart('w'), System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out var valueX))
            {
                if (int.TryParse(y.TrimStart('w'), System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out var valueY))
                {
                    return valueX.CompareTo(valueY);
                }

                // x is valid but y is not
                return 1;
            }

            if (int.TryParse(y.TrimStart('w'), System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out _))
            {
                // y is valid but x is not
                return -1;
            }

            return 0;
        }
    }
}