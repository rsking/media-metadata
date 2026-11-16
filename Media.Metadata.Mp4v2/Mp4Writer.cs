// -----------------------------------------------------------------------
// <copyright file="Mp4Writer.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Media.Metadata;

/// <summary>
/// The MP4v2 writer.
/// </summary>
public class Mp4Writer : IUpdater
{
    /// <inheritdoc/>
    public void UpdateVideo(string fileName, Video video, IDictionary<MediaTrackType, string>? languages = default)
    {
        if (video is Episode episode)
        {
            this.UpdateEpisode(fileName, episode, languages);
        }
        else if (video is Movie movie)
        {
            this.UpdateMovie(fileName, movie, languages);
        }
        else
        {
            var file = Mp4File.Create(TagLib.File.Create(fileName));
            var mediaType = file.Tags?.MediaType ?? MediaKind.NotSet;
            Update(file, video, mediaType, languages);
            file.Save();
        }
    }

    /// <inheritdoc/>
    public void UpdateEpisode(string fileName, Episode episode, IDictionary<MediaTrackType, string>? languages = default)
    {
        var file = Mp4File.Create(TagLib.File.Create(fileName));
        Update(file, episode, MediaKind.TVShow, languages);
        if (file.Tags is { } tags)
        {
            // episode
            tags.EpisodeNumber = episode.Number;
            tags.SeasonNumber = episode.Season;
            tags.TVShow = episode.Show;
            tags.EpisodeId = episode.Id;
            tags.TVNetwork = episode.Network;
            tags.ContentId = episode.Part;
        }

        file.Save();
    }

    /// <inheritdoc/>
    public void UpdateMovie(string fileName, Movie movie, IDictionary<MediaTrackType, string>? languages = default)
    {
        var file = Mp4File.Create(TagLib.File.Create(fileName));
        Update(file, movie, MediaKind.Movie, languages);
        if (file.Tags is { } tags)
        {
            tags.Category = movie.Edition;
        }

        file.Save();
    }

    private static void Update(Mp4File file, Video video, MediaKind mediaKind, IDictionary<MediaTrackType, string>? languages)
    {
        const string DateFormat = "yyyy-MM-dd";
        if (file.Tags is { } tags)
        {
            tags.MediaType = mediaKind;
            tags.Title = video.Name;
            tags.Description = video.Description;
            tags.LongDescription = video.Description;
            tags.Genre = ToString(video.Genre);
            tags.Composer = ToString(video.Composers);
            tags.AlbumArtist = ToString(video.Cast);
            tags.ReleaseDate = video.Release?.ToString(DateFormat, System.Globalization.CultureInfo.InvariantCulture);
            tags.SetArtwork(video.Image, video.ImageFormat);
            tags.Work = video.Work;

            tags.RatingInfo = default;
            if (video.Rating is { } rating)
            {
                tags.RatingInfo = new RatingInfo
                {
                    RatingSource = rating.Standard,
                    Rating = rating.ContentRating,
                    SortValue = rating.Score,
                };
            }

            tags.MovieInfo ??= new MovieInfo();
            tags.MovieInfo.RemoveProducers();
            if (video.Producers is { } producers)
            {
                foreach (var producer in producers)
                {
                    tags.MovieInfo.Producers.Add(producer);
                }
            }

            tags.MovieInfo.RemoveDirectors();
            if (video.Directors is { } directors)
            {
                foreach (var director in directors)
                {
                    tags.MovieInfo.Directors.Add(director);
                }
            }

            tags.MovieInfo.RemoveCast();
            if (video.Cast is { } cast)
            {
                foreach (var member in cast)
                {
                    tags.MovieInfo.Cast.Add(member);
                }
            }

            tags.MovieInfo.RemoveScreenwriters();
            if (video.ScreenWriters is { } screenWriters)
            {
                foreach (var screenWriter in screenWriters)
                {
                    tags.MovieInfo.Screenwriters.Add(screenWriter);
                }
            }

            tags.MovieInfo.Studio = ToString(video.Studios);

            static string? ToString(IEnumerable<string>? value)
            {
                if (value is null)
                {
                    return default;
                }

                var stringValue = string.Join(", ", value);
                return string.IsNullOrEmpty(stringValue)
                    ? default
                    : stringValue;
            }
        }

        if (languages is { } l && file.Tracks is { } tracks)
        {
            foreach (var language in l)
            {
                foreach (var track in GetTracks(tracks, language.Key))
                {
                    track.Language = language.Value;
                }

                static IEnumerable<Track> GetTracks(IEnumerable<Track> tracks, MediaTrackType key)
                {
                    return key switch
                    {
                        MediaTrackType.All => tracks,
                        MediaTrackType.Audio => tracks.Where(track => string.Equals(track.Type, NativeMethods.MP4AudioTrackType, StringComparison.Ordinal)),
                        MediaTrackType.Video => tracks.Where(track => string.Equals(track.Type, NativeMethods.MP4VideoTrackType, StringComparison.Ordinal)),
                        _ => GetTrackById((int)key),
                    };

                    IEnumerable<Track> GetTrackById(int id)
                    {
                        return tracks.Where(track => track.Id == id);
                    }
                }
            }
        }
    }
}