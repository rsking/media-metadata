// -----------------------------------------------------------------------
// <copyright file="ServiceCollectionExtensions.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

#pragma warning disable IDE0130
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RestSharp.Serializers.Json;

/// <summary>
/// Extension methods for <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the TVDB services.
    /// </summary>
    /// <param name="services">The services.</param>
    /// <param name="configuration">The configuration.</param>
    /// <returns>The input services.</returns>
    public static IServiceCollection AddTheTVDB(this IServiceCollection services, IConfiguration configuration) =>
        services
            .Configure<Media.Metadata.TheTVDB.TheTVDbOptions>(configuration.GetSection("TheTVDB"))
            .AddTransient<Media.Metadata.IShowSearch, Media.Metadata.TheTVDB.TheTVDbShowSearch>();

    /// <summary>
    /// Adds the TV Maze services.
    /// </summary>
    /// <param name="services">The services.</param>
    /// <param name="configuration">The configuration.</param>
    /// <returns>The input services.</returns>
    public static IServiceCollection AddTvMaze(this IServiceCollection services, IConfiguration configuration) =>
        services
            .Configure<Media.Metadata.TvMaze.TvMazeOptions>(configuration.GetSection("TvMaze"))
            .AddTransient<Media.Metadata.IShowSearch, Media.Metadata.TvMaze.TvMazeShowSearch>();

    /// <summary>
    /// Adds the TMDb services.
    /// </summary>
    /// <param name="services">The services.</param>
    /// <returns>The input services.</returns>
    public static IServiceCollection AddTMDb(this IServiceCollection services) =>
        services
            .AddRestSharp()
            .AddTransient<Media.Metadata.IMovieSearch, Media.Metadata.TMDb.TMDbMovieSearch>();

    /// <summary>
    /// Adds the <see cref="TagLib"/> services.
    /// </summary>
    /// <param name="services">The services.</param>
    /// <returns>The input services.</returns>
    public static IServiceCollection AddTagLib(this IServiceCollection services)
    {
        services.TryAddTransient<Media.Metadata.IReader, Media.Metadata.TagLibReader>();
        return services;
    }

    /// <summary>
    /// Adds the MP4 v2 services.
    /// </summary>
    /// <param name="services">The services.</param>
    /// <returns>The input services.</returns>
    public static IServiceCollection AddMp4v2(this IServiceCollection services)
    {
        if (OperatingSystem.IsWindows())
        {
            AddFileUpdaterCore(services);
        }

        return services;

        static void AddFileUpdaterCore(IServiceCollection services)
        {
            _ = services
                .AddTransient<Media.Metadata.IUpdater, Media.Metadata.Mp4Writer>()
                .AddTransient<Media.Metadata.IOptimizer, Media.Metadata.Mp4Optimizer>();

            AppDomain.CurrentDomain.AddNativePath();
        }
    }

    /// <summary>
    /// Adds the PLEX services.
    /// </summary>
    /// <param name="services">The services.</param>
    /// <returns>The input services.</returns>
    public static IServiceCollection AddPlex(this IServiceCollection services)
    {
        services.TryAddTransient<Media.Metadata.IRenamer, Media.Metadata.Plex.PlexRenamer>();
        return services;
    }

    /// <summary>
    /// Adds rest sharp to the services.
    /// </summary>
    /// <param name="services">The services.</param>
    /// <returns>The input services.</returns>
    public static IServiceCollection AddRestSharp(this IServiceCollection services)
    {
        services
            .TryAddTransient(_ =>
                new RestSharp.RestClient(configureSerialization: s => s.UseSystemTextJson(new System.Text.Json.JsonSerializerOptions
                {
                    Converters = { new Media.Metadata.JsonDateConverter() },
                    PropertyNameCaseInsensitive = true,
                })));

        return services;
    }
}