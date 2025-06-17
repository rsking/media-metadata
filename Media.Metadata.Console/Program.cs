// -----------------------------------------------------------------------
// <copyright file="Program.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.CommandLine.Hosting;
using System.CommandLine.Parsing;
using Media.Metadata;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.Hosting;

System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

var searchCommand = new CliCommand("search")
{
    CreateSearchMovie(),
    CreateSearchShow(),
};

var readCommand = new CliCommand("read")
{
    CreateReadMovie(),
    CreateReadEpisode(),
};

var langOption = new CliOption<string[]>("--lang", "-l")
{
    Description = "`[tkID=]LAN` Set the language. LAN is the ISO 639 code (eng, swe, ...). If no track ID is given, sets language to all tracks",
    Arity = ArgumentArity.OneOrMore,
};

var updateCommand = new CliCommand("update")
{
    CreateUpdateMovie(langOption),
    CreateUpdateEpisode(langOption),
    CreateUpdateVideo(langOption),
};

var optimiseCommand = CreateOptimize();

var renameCommand = CreateRename();

var configuration = new CliConfiguration(new CliRootCommand
{
    searchCommand,
    readCommand,
    updateCommand,
    optimiseCommand,
    renameCommand,
});

await configuration
    .UseHost(
        Host.CreateDefaultBuilder,
        configure => configure.ConfigureServices((builder, services) =>
        {
            _ = services
                .AddTMDb()
                .AddTheTVDB(builder.Configuration)
                .AddMp4v2()
                .AddTagLib()
                .AddPlex();

            _ = services
                .Configure<InvocationLifetimeOptions>(options => options.SuppressStatusMessages = true);
        }))
    .InvokeAsync([.. args.Select(Environment.ExpandEnvironmentVariables)])
    .ConfigureAwait(true);

static CliCommand CreateSearchMovie()
{
    var nameArgument = new CliArgument<string>("name");
    var yearOption = new CliOption<int?>("--year", "-y") { Description = "The movie year" };
    var command = new CliCommand("movie")
    {
        nameArgument,
        yearOption,
    };

    command.SetAction(
        async (parseResult, cancellationToken) =>
        {
            var name = parseResult.GetValue(nameArgument).ThrowIfNull();
            var year = parseResult.GetValue(yearOption);
            foreach (var search in parseResult.GetHost().Services.GetServices<IMovieSearch>())
            {
                await foreach (var movie in search.SearchAsync(name, year ?? 0, cancellationToken: cancellationToken).ConfigureAwait(false))
                {
                    Console.WriteLine("{0} - {1:yyyy-MM-dd}", movie.Name, movie.Release);
                    Console.WriteLine(movie.ToString());

                    if (string.Equals(movie.Name, name, StringComparison.Ordinal)
                        && (year is 0 || (movie.Release.HasValue && movie.Release.Value.Year == year)))
                    {
                        break;
                    }
                }
            }
        });

    return command;
}

static CliCommand CreateReadMovie()
{
    var pathArgument = new CliArgument<FileInfo>("path").AcceptExistingOnly();
    var command = new CliCommand("movie")
    {
        pathArgument,
    };

    command.SetAction((parseResult, _) =>
    {
        var path = parseResult.GetValue(pathArgument).ThrowIfNull().ThrowIfNotExists();
        var movie = parseResult.GetHost().Services.GetRequiredService<IReader>().ReadMovie(path.FullName);
        Console.WriteLine(movie.Name);
        return Task.CompletedTask;
    });

    return command;
}

static CliCommand CreateReadEpisode()
{
    var pathArgument = new CliArgument<FileInfo>("path").AcceptExistingOnly();
    var command = new CliCommand("episode")
    {
        pathArgument,
    };

    command.SetAction((parseResult, _) =>
    {
        var path = parseResult.GetValue(pathArgument).ThrowIfNull().ThrowIfNotExists();
        var episode = parseResult.GetHost().Services.GetRequiredService<IReader>().ReadEpisode(path.FullName);
        Console.WriteLine(episode.Name);
        return Task.CompletedTask;
    });

    return command;
}

static CliCommand CreateSearchShow()
{
    var nameArgument = new CliArgument<string>("name");
    var yearOption = new CliOption<int>("--year", "-y") { Description = "The show year" };
    var command = new CliCommand("show")
    {
        nameArgument,
        yearOption,
    };

    command.SetAction(async (parseResult, cancellationToken) =>
    {
        var name = parseResult.GetValue(nameArgument).ThrowIfNull();
        var year = parseResult.GetValue(yearOption);

        foreach (var search in parseResult.GetHost().Services.GetServices<IShowSearch>())
        {
            await foreach (var show in search.SearchAsync(name, year, cancellationToken: cancellationToken).ConfigureAwait(false))
            {
                Console.WriteLine("{0}", show.Name);
                foreach (var season in show.Seasons.OrderBy(season => season.Number))
                {
                    Console.WriteLine("\tSeason {0}", season.Number);
                    if (season.Episodes is { } episodes)
                    {
                        foreach (var episode in episodes)
                        {
                            Console.WriteLine("\t\t{0}: {1}", episode.Name, episode.Description);
                        }
                    }
                }
            }
        }
    });

    return command;
}

static CliCommand CreateUpdateMovie(CliOption<string[]> langOption)
{
    var pathArgument = new CliArgument<FileInfo>("path").AcceptExistingOnly();
    var nameArgument = new CliArgument<string>("name");
    var yearOption = new CliOption<int>("--year", "-y") { Description = "The movie year" };
    var command = new CliCommand("movie")
    {
        pathArgument,
        nameArgument,
        yearOption,
        langOption,
    };

    command.SetAction(
        async (parseResult, cancellationToken) =>
        {
            var path = parseResult.GetValue(pathArgument).ThrowIfNull().ThrowIfNotExists();
            var host = parseResult.GetHost();
            var name = parseResult.GetValue(nameArgument).ThrowIfNull();
            var year = parseResult.GetValue(yearOption);
            var lang = parseResult.GetValue(langOption);

            foreach (var search in host.Services.GetServices<IMovieSearch>())
            {
                await foreach (var movie in search.SearchAsync(name, year, cancellationToken: cancellationToken).ConfigureAwait(false))
                {
                    if (string.Equals(movie.Name, name, StringComparison.OrdinalIgnoreCase) && (year is 0 || (movie.Release.HasValue && movie.Release.Value.Year == year)))
                    {
                        await parseResult.Configuration.Output.WriteLineAsync(string.Create(System.Globalization.CultureInfo.CurrentCulture, $"Found Movie {movie.Name} ({movie.Release?.Year})")).ConfigureAwait(false);
                        await parseResult.Configuration.Output.WriteLineAsync($"Saving {path.Name}").ConfigureAwait(false);
                        host.Services.GetRequiredService<IUpdater>().UpdateMovie(path.FullName, movie, GetLanguages(lang));
                        await parseResult.Configuration.Output.WriteLineAsync($"Saved {path.Name}").ConfigureAwait(false);
                        break;
                    }
                }
            }
        });

    return command;
}

static CliCommand CreateUpdateEpisode(CliOption<string[]> langOption)
{
    var pathArgument = new CliArgument<FileInfo[]>("path") { CustomParser = ParseFileInfo };
    var nameOption = new CliOption<string>("--name", "-n") { Description = "The series name", Required = true };
    var yearOption = new CliOption<int>("--year", "-y") { Description = "The series year", DefaultValueFactory = _ => -1 };
    var seasonOption = new CliOption<int>("--season", "-s") { Description = "The season number", DefaultValueFactory = _ => -1 };
    var episodeOption = new CliOption<int>("--episode", "-e") { Description = "The episode number", DefaultValueFactory = _ => -1 };
    var ignoreOption = new CliOption<bool>("--ignore", "-i") { Description = "Ignore files that already have a valid episode" };
    var episodeOffsetOption = new CliOption<int>("--offset", "-o") { Description = "Offset for episode numbers" };

    var command = new CliCommand("episode")
    {
        pathArgument,
        nameOption,
        yearOption,
        seasonOption,
        episodeOption,
        ignoreOption,
        episodeOffsetOption,
        langOption,
    };

    command.SetAction((parseResult, cancellationToken) =>
    {
        return Process(
            parseResult.Configuration,
            parseResult.GetHost(),
            parseResult.GetValue(pathArgument).ThrowIfNull(),
            parseResult.GetValue(nameOption).ThrowIfNull(),
            parseResult.GetValue(yearOption),
            parseResult.GetValue(seasonOption),
            parseResult.GetValue(episodeOption),
            parseResult.GetValue(ignoreOption),
            parseResult.GetValue(episodeOffsetOption),
            parseResult.GetValue(langOption),
            cancellationToken);

        static async Task Process(CliConfiguration console, IHost host, FileInfo[] paths, string name, int year, int season, int episode, bool ignore, int offset, string[]? lang, CancellationToken cancellationToken)
        {
            var regex = GetEpisodeRegexes();

            var reader = host.Services.GetRequiredService<IReader>();
            var pathList = paths
                .Where(path => ShouldProcess(path, reader, ignore))
                .ToList();
            var updater = host.Services.GetRequiredService<IUpdater>();

            foreach (var search in host.Services.GetServices<IShowSearch>())
            {
                await foreach (var series in search.SearchAsync(name, year, cancellationToken: cancellationToken).ConfigureAwait(false))
                {
                    var seasons = pathList
                        .Where(path => path.Exists)
                        .GroupBy(
                            path => season switch
                            {
                                -1 when GetMatch(path.Name) is { } match => int.Parse(match.Groups["season"].Value, System.Globalization.CultureInfo.CurrentCulture),
                                _ => season,
                            },
                            path => path)
                        .OrderBy(group => group.Key)
                        .ToList();

                    if (seasons.Count is 0)
                    {
                        return;
                    }

                    await console.Output.WriteLineAsync($"Found Series  {series.Name}").ConfigureAwait(false);
                    var seasonEnumerator = series.Seasons.GetEnumerator();
                    if (!seasonEnumerator.MoveNext())
                    {
                        continue;
                    }

                    foreach (var seasonGroup in seasons)
                    {
                        while (seasonEnumerator is { Current: { } current } && current.Number < seasonGroup.Key && seasonEnumerator.MoveNext())
                        {
                            // loop until we have the correct season.
                        }

                        if (seasonEnumerator.Current is { } s && s.Number == seasonGroup.Key)
                        {
                            await console.Output.WriteLineAsync(string.Create(System.Globalization.CultureInfo.CurrentCulture, $"Found Season  {series.Name}:{s.Number}")).ConfigureAwait(false);

                            var episodes = seasonGroup
                                .Select(path => episode switch
                                {
                                    -1 when GetMatch(path.Name) is { } match => (Episode: int.Parse(match.Groups["episode"].Value, System.Globalization.CultureInfo.CurrentCulture) + offset, Path: path),
                                    _ => (Episode: episode + offset, Path: path),
                                })
                                .OrderBy(ep => ep.Episode)
                                .ToList();

                            var episodeEnumerator = s.Episodes.GetEnumerator();

                            if (!episodeEnumerator.MoveNext())
                            {
                                continue;
                            }

                            foreach (var ep in episodes.Where(ep => ep.Path.Exists))
                            {
                                await console.Output.WriteLineAsync(string.Create(System.Globalization.CultureInfo.CurrentCulture, $"Processing {ep.Path.Name}")).ConfigureAwait(false);

                                while (episodeEnumerator is { Current: { } current } && current.Number < ep.Episode && episodeEnumerator.MoveNext())
                                {
                                    // loop until we have the correct episode.
                                }

                                if (episodeEnumerator.Current is { } e && e.Number == ep.Episode)
                                {
                                    await console.Output.WriteLineAsync(string.Create(System.Globalization.CultureInfo.CurrentCulture, $"Found Episode {series.Name}:{s.Number}:{e.Name}")).ConfigureAwait(false);
                                    if (e.Rating is null)
                                    {
                                        await console.Error.WriteLineAsync(string.Create(System.Globalization.CultureInfo.CurrentCulture, $"Failed to get rating for {e.Name}")).ConfigureAwait(false);
                                    }

                                    var (image, imageFormat) = (s, series, e) switch
                                    {
                                        (IHasImage { Image: not null } i, _, _) => (i.Image, i.ImageFormat),
                                        (_, IHasImage { Image: not null } i, _) => (i.Image, i.ImageFormat),
                                        (_, _, IHasImage { Image: not null } i) => (i.Image, i.ImageFormat),
                                        _ => (default, default),
                                    };

                                    updater.UpdateEpisode(ep.Path.FullName, e with { Image = image, ImageFormat = imageFormat }, GetLanguages(lang));

                                    // remove this from the list of paths
                                    _ = pathList.Remove(ep.Path);
                                }
                            }
                        }
                    }
                }
            }

            System.Text.RegularExpressions.Match? GetMatch(string input)
            {
                return regex
                    .Select(r => r.Match(input))
                    .FirstOrDefault(m => m is { Success: true });
            }

            static bool ShouldProcess(FileInfo path, IReader reader, bool ignore)
            {
                if (!path.Exists)
                {
                    return false;
                }

                if (!ignore)
                {
                    return true;
                }

                // read the file to see if it has an episode
                using var f = reader.ReadEpisode(path.FullName);
                return string.IsNullOrEmpty(f.Show) || f.Season < 0 || f.Number < 0 || string.IsNullOrEmpty(f.Name);
            }
        }
    });

    return command;
}

static CliCommand CreateUpdateVideo(CliOption<string[]> langOption)
{
    var pathArgument = new CliArgument<FileInfo[]>("path") { CustomParser = ParseFileInfo }.AcceptExistingOnly();
    var command = new CliCommand("video")
    {
        pathArgument,
        langOption,
    };

    command.SetAction(
        async (parseResult, _) =>
        {
            var host = parseResult.GetHost();
            var path = parseResult.GetValue(pathArgument).ThrowIfNull();
            var lang = parseResult.GetValue(langOption);
            var reader = host.Services.GetRequiredService<IReader>();
            var updater = host.Services.GetRequiredService<IUpdater>();
            var languages = GetLanguages(lang);
            foreach (var p in path.Where(p => p.Exists).Select(p => p.FullName))
            {
                var video = reader.ReadVideo(p);
                await parseResult.Configuration.Output.WriteLineAsync(string.Create(System.Globalization.CultureInfo.CurrentCulture, $"Updating {video.Name}")).ConfigureAwait(true);
                updater.UpdateVideo(p, video, languages);
            }
        });

    return command;
}

static CliCommand CreateOptimize()
{
    var pathArgument = new CliArgument<FileInfo>("path").AcceptExistingOnly();
    var command = new CliCommand("optimize") { pathArgument };

    command.SetAction(
        (parseResult, cancellationToken) => Task.Run(
            () =>
            {
                var path = parseResult.GetValue(pathArgument).ThrowIfNull().ThrowIfNotExists();
                _ = parseResult.GetHost().Services.GetRequiredService<IOptimizer>().Optimize(path.FullName);
            },
            cancellationToken));

    return command;
}

static CliCommand CreateRename()
{
    var sourceArgument = new CliArgument<DirectoryInfo>("source");
    var moviesOption = new CliOption<DirectoryInfo>("--movies") { Description = "The destination folder for movies. If unset, defaults to \"--tv\"" }.AcceptExistingOnly();
    var tvOption = new CliOption<DirectoryInfo>("--tv") { Description = "The destination folder for TV shows. If unset, defaults to \"--movies\"" }.AcceptExistingOnly();
    var moveOption = new CliOption<bool>("-m", "--move") { Description = "Moves the files" };
    var recursiveOption = new CliOption<bool>("-r", "--recursive") { Description = "Recursively searches <SOURCE>" };
    var dryRunOption = new CliOption<bool>("-n", "--dry-run") { Description = "Don't actually move/copy any file(s). Instead, just show if they exist and would otherwise be moved/copied by the command." };
    var inPlaceOption = new CliOption<bool>("-i", "--in-place") { Description = "Renames the files in place, rather than to <DESTINATION>." };

    var command = new CliCommand("rename")
    {
        sourceArgument,
        moviesOption,
        tvOption,
        moveOption,
        recursiveOption,
        dryRunOption,
        inPlaceOption,
    };

    command.SetAction(async (parseResult, cancellationToken) =>
    {
        var host = parseResult.GetHost();
        var reader = host.Services.GetRequiredService<IReader>();
        var renamer = host.Services.GetRequiredService<IRenamer>();
        var source = parseResult.GetValue(sourceArgument) ?? throw new InvalidOperationException();
        var recursive = parseResult.GetValue(recursiveOption);

        var move = parseResult.GetValue(moveOption);
        var dryRun = parseResult.GetValue(dryRunOption);

        var movies = parseResult.GetValue(moviesOption);
        var tv = parseResult.GetValue(tvOption);
        tv ??= movies;
        movies ??= tv;

        if (movies is null || tv is null)
        {
            throw new InvalidOperationException($"Either {moviesOption.Name} or {tvOption.Name} to be set.");
        }

        foreach (var file in source.EnumerateFiles("*.*", new EnumerationOptions { RecurseSubdirectories = recursive, IgnoreInaccessible = true, AttributesToSkip = FileAttributes.Hidden }))
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            if (file.Length is 0)
            {
                continue;
            }

            Video input;
            try
            {
                input = ReadFile(reader, file.FullName);
            }
            catch (Exception ex)
            {
                await parseResult.Configuration.Output.WriteLineAsync($"Unsupported file - {file.Name} - {ex.Message}").ConfigureAwait(true);
                continue;
            }

            if (renamer.GetFileName(file.FullName, input) is { } name)
            {
                var basePath = input switch
                {
                    Episode => tv.FullName,
                    Movie => movies.FullName,
                    _ => throw new InvalidOperationException(),
                };

                var path = new FileInfo(Path.Combine(basePath, name));
                if (path.Exists && path.Length == file.Length)
                {
                    await parseResult.Configuration.Output.WriteLineAsync($"{file.FullName} has the same length as {path.FullName}").ConfigureAwait(true);
                    continue;
                }

                if (!dryRun && path.Directory is { } directory)
                {
                    directory.Create();
                }

                if (move)
                {
                    if (!path.Exists)
                    {
                        await parseResult.Configuration.Output.WriteLineAsync($"Moving {file.FullName} to {path.FullName}").ConfigureAwait(true);
                        if (!dryRun)
                        {
                            file.MoveTo(path.FullName);
                        }
                    }
                    else
                    {
                        await parseResult.Configuration.Output.WriteLineAsync($"Replacing {path.FullName} with {file.FullName} with a move").ConfigureAwait(true);
                        if (!dryRun)
                        {
                            _ = file.CopyTo(path.FullName, overwrite: true);
                            if (file.Exists)
                            {
                                file.Delete();
                            }
                        }
                    }
                }
                else
                {
                    if (path.Exists)
                    {
                        await parseResult.Configuration.Output.WriteLineAsync($"Replacing {path.FullName} with {file.FullName} by a copy").ConfigureAwait(true);
                    }
                    else
                    {
                        await parseResult.Configuration.Output.WriteLineAsync($"Coping {file.FullName} to {path.FullName}").ConfigureAwait(true);
                    }

                    if (!dryRun)
                    {
                        _ = file.CopyTo(path.FullName, overwrite: true);
                    }
                }
            }

            static Video ReadFile(IReader reader, string path, int retries = 3)
            {
                for (var i = 0; i < retries - 1; i++)
                {
                    try
                    {
                        return reader.ReadVideo(path);
                    }
                    catch
                    {
                        // this in done on a retry basis
                    }
                }

                return reader.ReadVideo(path);
            }
        }
    });

    return command;
}

[System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S1168:Empty arrays and collections should be returned instead of null", Justification = "A null result is not the same as an empty result")]
static Dictionary<MediaTrackType, string>? GetLanguages(string[]? lang)
{
    return lang?.Select(val => val.Split('=')).ToDictionary(GetId, GetLang);

    static MediaTrackType GetId(string[] val)
    {
        return val.Length > 1
            ? val[0].ToLowerInvariant() switch
            {
                "a" => MediaTrackType.Audio,
                "v" => MediaTrackType.Video,
                { } value when int.TryParse(value, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.CurrentCulture, out var intValue) => (MediaTrackType)intValue,
                _ => MediaTrackType.Unknown,
            }
            : MediaTrackType.All;
    }

    static string GetLang(string[] val)
    {
        return val[val.Length > 1 ? 1 : 0].ToLowerInvariant();
    }
}

static FileInfo[] ParseFileInfo(ArgumentResult argumentResult)
{
    return [.. Process(argumentResult.Tokens.Select(token => token.Value)).SelectMany(results => results.Select(file => new FileInfo(file)))];

    static IEnumerable<IEnumerable<string>> Process(IEnumerable<string> tokens)
    {
        var normalisedTokens = tokens.Select(token => token.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)).ToArray();
        var rooted = normalisedTokens.Where(Path.IsPathRooted).ToArray();
        foreach (var root in rooted)
        {
            yield return GetRooted(root);
        }

        var matcher = new Matcher(StringComparison.CurrentCulture);
        matcher.AddIncludePatterns(normalisedTokens.Except(rooted, StringComparer.Ordinal));
        yield return matcher.GetResultsInFullPath(Directory.GetCurrentDirectory());

        static IEnumerable<string> GetRooted(string root)
        {
            var matcher = new Matcher(StringComparison.OrdinalIgnoreCase);

            // separate the root directory from the glob
            var glob = Path.GetFileName(root);
            var directory = Path.GetDirectoryName(root);
            while (directory?.Contains('*', StringComparison.OrdinalIgnoreCase) is true)
            {
                glob = $"{Path.GetFileName(directory)}{Path.AltDirectorySeparatorChar}{glob}";
                directory = Path.GetDirectoryName(directory);
            }

            _ = matcher.AddInclude(glob);
            return matcher.GetResultsInFullPath(directory ?? ".\\");
        }
    }
}

/// <content>
/// Compiled <see cref="System.Text.RegularExpressions.Regex"/>.
/// </content>
internal sealed partial class Program
{
    private const int MillisecondTimeout = 1000;

    private Program()
    {
    }

    /// <summary>
    /// Gets the episode <see cref="System.Text.RegularExpressions.Regex"/> expressions.
    /// </summary>
    /// <returns>The episode <see cref="System.Text.RegularExpressions.Regex"/> expressions.</returns>
    internal static IEnumerable<System.Text.RegularExpressions.Regex> GetEpisodeRegexes() =>
        [
            SbsRegex1(),
            SbsRegex2(),
            SbsRegex3(),
            IViewRegex1(),
            IViewRegex2(),
            Season(),
        ];

    [System.Text.RegularExpressions.GeneratedRegex(@"[Ss](?<season>\d{2})[Ee](?<episode>\d{2})", System.Text.RegularExpressions.RegexOptions.None, MillisecondTimeout)]
    private static partial System.Text.RegularExpressions.Regex SbsRegex1();

    [System.Text.RegularExpressions.GeneratedRegex(@"S(?<season>\d+) Ep(?<episode>\d+)", System.Text.RegularExpressions.RegexOptions.None, MillisecondTimeout)]
    private static partial System.Text.RegularExpressions.Regex SbsRegex2();

    [System.Text.RegularExpressions.GeneratedRegex(@"S(?<season>\d+) Ep. (?<episode>\d+)", System.Text.RegularExpressions.RegexOptions.None, MillisecondTimeout)]
    private static partial System.Text.RegularExpressions.Regex SbsRegex3();

    [System.Text.RegularExpressions.GeneratedRegex(@"Series (?<season>\d+) Ep (?<episode>\d+)", System.Text.RegularExpressions.RegexOptions.None, MillisecondTimeout)]
    private static partial System.Text.RegularExpressions.Regex IViewRegex1();

    [System.Text.RegularExpressions.GeneratedRegex(@"..(\d+).(?<episode>\d{3})S(\d{2})", System.Text.RegularExpressions.RegexOptions.ExplicitCapture | System.Text.RegularExpressions.RegexOptions.None, MillisecondTimeout)]
    private static partial System.Text.RegularExpressions.Regex IViewRegex2();

    [System.Text.RegularExpressions.GeneratedRegex(@"Season (?<season>\d{2})", System.Text.RegularExpressions.RegexOptions.ExplicitCapture | System.Text.RegularExpressions.RegexOptions.None, MillisecondTimeout)]
    private static partial System.Text.RegularExpressions.Regex Season();
}