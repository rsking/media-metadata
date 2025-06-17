// -----------------------------------------------------------------------
// <copyright file="TheTVDbShowSearch.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Media.Metadata.TheTVDB;

/// <summary>
/// The TVDb show search.
/// </summary>
/// <remarks>
/// Initialises a new instance of the <see cref="TheTVDbShowSearch"/> class.
/// </remarks>
/// <param name="options">The options.</param>
public sealed class TheTVDbShowSearch(Microsoft.Extensions.Options.IOptions<TheTVDbOptions> options) : IShowSearch
{
    private static readonly System.Text.Json.JsonSerializerOptions Options = new() { PropertyNamingPolicy = new LowerCaseJsonNamingPolicy() };

    private readonly ApiSdk.ApiClient client = CreateClient(new(options.Value.Url), options.Value.Pin ?? throw new ArgumentNullException(nameof(options), "Pin cannot be null"));

    /// <inheritdoc/>
    async IAsyncEnumerable<Series> IShowSearch.SearchAsync(string name, int year, string country, [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await foreach (var series in this
            .SearchSeriesAsync(name, year, cancellationToken)
            .ConfigureAwait(false))
        {
            if (series is { TvdbId: { } id, Name: { } seriesName, ImageUrl: var imageUrl })
            {
                yield return new RemoteSeries(seriesName, GetSeasons(this.client, id, seriesName, country, cancellationToken).ToEnumerable())
                {
                    ImageUri = GetUri(imageUrl),
                };
            }
        }

        static Uri? GetUri(string? imageUrl)
        {
            return imageUrl is not null && Uri.TryCreate(imageUrl, UriKind.Absolute, out var uri)
                ? uri
                : default;
        }

        static async IAsyncEnumerable<RemoteSeason> GetSeasons(ApiSdk.ApiClient client, string? id, string? name, string country, [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken)
        {
            if (int.TryParse(id, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out var seriesId)
                && await client.Series[seriesId].Extended.GetAsync(cancellationToken: cancellationToken).ConfigureAwait(false) is { Data: { Seasons: { } seasons } series })
            {
                // only official, aired dates, ordered by the season number
                foreach (var season in seasons
                    .Where(s => s is { Type.Id: 1 })
                    .OrderBy(s => s.Number))
                {
                    if (season.Id is { } seasonId
                        && await client.Seasons[seasonId].Extended.GetAsync(cancellationToken: cancellationToken).ConfigureAwait(false) is { Data: { Number: { } number, Episodes: { } episodes } extendedSeason })
                    {
                        yield return new RemoteSeason(
                            (int)number,
                            GetEpisodes(
                                client,
                                name,
                                episodes,
                                country,
                                series.Characters ?? [],
                                series.Companies ?? [],
                                cancellationToken).ToEnumerable())
                        {
                            ImageUri = GetUriFromArtwork(extendedSeason.Artwork, "eng") ?? GetUri(extendedSeason.Image),
                        };

                        static Uri? GetUriFromArtwork(IEnumerable<ApiSdk.Models.ArtworkBaseRecord>? artwork, string language)
                        {
                            return artwork switch
                            {
                                { } a => a.Where(a => string.Equals(a.Language, language, StringComparison.Ordinal)).Select(a => GetUri(a.Image)).FirstOrDefault(),
                                _ => default,
                            };
                        }
                    }
                }

                static async IAsyncEnumerable<RemoteEpisode> GetEpisodes(ApiSdk.ApiClient client, string? name, IEnumerable<ApiSdk.Models.EpisodeBaseRecord> episodes, string country, IEnumerable<ApiSdk.Models.Character> characters, IEnumerable<ApiSdk.Models.Company> companies, [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken)
                {
                    var extendedEpisodes = episodes
                        .Where(ebr => ebr is { Id: not null })
                        .OrderBy(ebr => ebr.Number)
                        .ToAsyncEnumerable()
                        .SelectAwait(async episode =>
                        {
                            var episodeResponse = await client.Episodes[(double)episode.Id!].Extended.GetAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
                            return episodeResponse?.Data;
                        })
                        .Where(episode => episode is { Name: not null })
                        .Select(episode => episode!);

                    await foreach (var episode in extendedEpisodes
                        .Select(episode =>
                        {
                            var fullCharacters = episode.Characters is null
                                ? characters.ToList()
                                : [.. episode.Characters, .. characters];

                            var fullCompanies = episode.Companies is null
                                ? companies.ToList()
                                : [.. episode.Companies, .. companies];

                            return new RemoteEpisode(episode.Name, episode.Overview)
                            {
                                Season = episode.SeasonNumber,
                                Number = episode.Number,
                                Id = episode.ProductionCode,
                                Show = name,
                                ScreenWriters = GetWriters(fullCharacters, episode.Id),
                                Cast = GetCast(fullCharacters, episode.Id),
                                Directors = GetDirectors(fullCharacters, episode.Id),
                                Release = GetReleaseDate(episode.Aired),
                                Studios = GetStudios(fullCompanies),
                                Network = GetNetwork(fullCompanies),
                                Rating = GetRating(episode.ContentRatings, country),
                                ImageUri = GetImageUri(episode.Image),
                                Producers = GetProducers(fullCharacters, episode.Id),
                            };

                            static IEnumerable<string>? GetWriters(IEnumerable<ApiSdk.Models.Character>? characters, long? episodeId)
                            {
                                return GetCharacters(characters, episodeId, "Writer")?.ToList();
                            }

                            static IEnumerable<string>? GetDirectors(IEnumerable<ApiSdk.Models.Character>? characters, long? episodeId)
                            {
                                return GetCharacters(characters, episodeId, "Director")?.ToList();
                            }

                            static IEnumerable<string>? GetProducers(IEnumerable<ApiSdk.Models.Character>? characters, long? episodeId)
                            {
                                return GetCharacters(characters, episodeId, "Producer", "Executive Producer")?.ToList();
                            }

                            static IEnumerable<string>? GetCast(IEnumerable<ApiSdk.Models.Character>? characters, long? episodeId)
                            {
                                return GetCharacters(characters, episodeId, "Actor", "Guest Star");
                            }

                            static IEnumerable<string>? GetCharacters(IEnumerable<ApiSdk.Models.Character>? characters, long? episodeId, params string[] peopleTypes)
                            {
                                return characters switch
                                {
                                    null => default,
                                    _ => characters
                                        .Where(character => Array.Exists(peopleTypes, peopleType =>
                                            string.Equals(character.PeopleType, peopleType, StringComparison.Ordinal))
                                            && character.PersonName is not null
                                            && (!character.EpisodeId.HasValue || character.EpisodeId.Value == episodeId))
                                        .Select(character => character.PersonName!)
                                        .Distinct(StringComparer.OrdinalIgnoreCase),
                                };
                            }

                            static string? GetNetwork(ICollection<ApiSdk.Models.Company> companies)
                            {
                                var company = companies.FirstOrDefault(company => company.PrimaryCompanyType is 1);
                                return company?.Name;
                            }

                            static IEnumerable<string>? GetStudios(ICollection<ApiSdk.Models.Company> companies)
                            {
                                return WhereNotNull(companies.Where(company => company.PrimaryCompanyType is 2).Select(company => company.Name));

                                [System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S3267:Loops should be simplified with \"LINQ\" expressions", Justification = "This is to change nullability")]
                                static IEnumerable<string> WhereNotNull(IEnumerable<string?> source)
                                {
                                    foreach (var item in source)
                                    {
                                        if (item is not null)
                                        {
                                            yield return item;
                                        }
                                    }
                                }
                            }

                            static Rating? GetRating(ICollection<ApiSdk.Models.ContentRating>? contentRatings, string? country)
                            {
                                if (contentRatings is null)
                                {
                                    return default;
                                }

                                country = GetCountry(country);
                                return contentRatings.FirstOrDefault(contentRating => string.Equals(contentRating.Country, country, StringComparison.OrdinalIgnoreCase)) is { Name: { } n, Country: { } c }
                                    ? Rating.FindBest(n, GetCountry(c))
                                    : default;

                                static string GetCountry(string? country)
                                {
                                    return country switch
                                    {
                                        "aus" => "AU",
                                        "AU" => "aus",
                                        "us" => "US",
                                        "US" => "us",
                                        null => "AU",
                                        _ => country,
                                    };
                                }
                            }

                            static Uri? GetImageUri(string? imageUri)
                            {
                                return imageUri is null ? default : new Uri(imageUri);
                            }

                            static DateTime? GetReleaseDate(string? aired)
                            {
                                return aired switch
                                {
                                    not null when DateTime.TryParse(aired, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var releaseDate) => releaseDate,
                                    not null when DateTime.TryParse(aired, System.Globalization.CultureInfo.CurrentCulture, System.Globalization.DateTimeStyles.None, out var releaseDate) => releaseDate,
                                    _ => default,
                                };
                            }
                        })
                        .WithCancellation(cancellationToken)
                        .ConfigureAwait(false))
                    {
                        yield return episode;
                    }
                }
            }
        }
    }

    private static ApiSdk.ApiClient CreateClient(Uri baseUri, string pin)
    {
        var tokenProvider = new TokenProvider(baseUri, pin);
        var authenticationProvider = new Microsoft.Kiota.Abstractions.Authentication.BaseBearerTokenAuthenticationProvider(tokenProvider);
        var requestAdapter = new Microsoft.Kiota.Http.HttpClientLibrary.HttpClientRequestAdapter(authenticationProvider)
        {
            BaseUrl = baseUri.ToString(),
        };

        return new(requestAdapter);
    }

    private async IAsyncEnumerable<ApiSdk.Models.SearchResult> SearchSeriesAsync(string name, int year, [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var response = await this.client.Search.GetAsync(
            config =>
            {
                config.QueryParameters.Query = name;
                config.QueryParameters.Type = "series";
                if (year > 0)
                {
                    config.QueryParameters.Year = year;
                }
            },
            cancellationToken).ConfigureAwait(false);

        if (response is { Data: { } data })
        {
            foreach (var series in data)
            {
                yield return series;
            }
        }
    }

    private sealed class LowerCaseJsonNamingPolicy : System.Text.Json.JsonNamingPolicy
    {
        public override string ConvertName(string name) => name.ToLowerInvariant();
    }

    private sealed class TokenProvider(Uri baseUri, string pin) : Microsoft.Kiota.Abstractions.Authentication.IAccessTokenProvider
    {
        private ApiSdk.Login.LoginPostResponse_data? token;

        public Microsoft.Kiota.Abstractions.Authentication.AllowedHostsValidator AllowedHostsValidator { get; } = new();

        public async Task<string> GetAuthorizationTokenAsync(Uri uri, Dictionary<string, object>? additionalAuthenticationContext = null, CancellationToken cancellationToken = default)
        {
            if (this.token is { Token: { } cachedToken })
            {
                return cachedToken;
            }

            this.token = await GetTokenAsync(baseUri, pin, force: false, cancellationToken).ConfigureAwait(false);
            return this.token?.Token ?? throw new UnauthorizedAccessException();
        }

        private static async Task<ApiSdk.Login.LoginPostResponse_data?> GetTokenAsync(Uri baseUri, string? pin, bool force, CancellationToken cancellationToken = default)
        {
            if (!force && await GetTokenFromFile(cancellationToken: cancellationToken).ConfigureAwait(false) is { } tokenFromFile)
            {
                return tokenFromFile;
            }

            // create the temporary client
            var client = new ApiSdk.ApiClient(new Microsoft.Kiota.Http.HttpClientLibrary.HttpClientRequestAdapter(new Microsoft.Kiota.Abstractions.Authentication.AnonymousAuthenticationProvider()) { BaseUrl = baseUri.ToString() });

            if (await RequestToken(client, pin, cancellationToken).ConfigureAwait(false) is { } tokenFromWeb)
            {
                // write this to the local cache
                await WriteTokenToFile(tokenFromWeb, cancellationToken: cancellationToken).ConfigureAwait(false);
                return tokenFromWeb;
            }

            return default;

            static async Task<ApiSdk.Login.LoginPostResponse_data?> RequestToken(ApiSdk.ApiClient client, string? pin, CancellationToken cancellationToken)
            {
                var request = new ApiSdk.Login.LoginPostRequestBody
                {
                    Apikey = TheTVDbHelpers.ApiKey,
                    Pin = pin,
                };

                var response = await client.Login.PostAsync(request, cancellationToken: cancellationToken).ConfigureAwait(false);

                return response?.Data;
            }

            static async Task<ApiSdk.Login.LoginPostResponse_data?> GetTokenFromFile(string? fileName = null, CancellationToken cancellationToken = default)
            {
                fileName ??= GenerateFileName();
                if (File.Exists(fileName))
                {
                    var stream = File.OpenRead(fileName);
#if NETSTANDARD2_1_OR_GREATER
                    await using (stream.ConfigureAwait(false))
#else
                    using (stream)
#endif
                    {
                        return await System.Text.Json.JsonSerializer.DeserializeAsync<ApiSdk.Login.LoginPostResponse_data>(stream, Options, cancellationToken).ConfigureAwait(false);
                    }
                }

                return default;
            }

            static async Task WriteTokenToFile(ApiSdk.Login.LoginPostResponse_data tokenResponse, string? fileName = null, CancellationToken cancellationToken = default)
            {
                fileName ??= GenerateFileName();
                _ = Directory.CreateDirectory(Path.GetDirectoryName(fileName));
                var stream = File.OpenWrite(fileName);
#if NETSTANDARD2_1_OR_GREATER
                await using (stream.ConfigureAwait(false))
#else
                using (stream)
#endif
                {
                    await System.Text.Json.JsonSerializer.SerializeAsync(stream, tokenResponse, Options, cancellationToken: cancellationToken).ConfigureAwait(false);
                }
            }

            static string GenerateFileName()
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Media.Metadata", "TheTVDb.token.json");
            }
        }
    }
}