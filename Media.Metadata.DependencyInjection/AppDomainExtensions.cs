// -----------------------------------------------------------------------
// <copyright file="AppDomainExtensions.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

#pragma warning disable IDE0130
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130

using System.Runtime.InteropServices;

/// <summary>
/// Extension methods for <see cref="AppDomain"/>.
/// </summary>
public static class AppDomainExtensions
{
    /// <summary>
    /// Adds the native path.
    /// </summary>
    /// <param name="appDomain">The application domain.</param>
    /// <exception cref="InvalidOperationException">Failed to determine native path.</exception>
    public static void AddNativePath(this AppDomain appDomain)
    {
        var potentialPaths = GetPotentialPaths()
            .Select(path => path.Replace('/', Path.DirectorySeparatorChar));

        var probingDirectories = ((string?)appDomain.GetData("PROBING_DIRECTORIES"))?.Split(Path.PathSeparator) ?? [appDomain.BaseDirectory];
        var directories = probingDirectories
            .Select(RootedPath)
            .SelectMany(directory => potentialPaths.Select(path => Path.Combine(directory, path)))
            .Where(Directory.Exists);

        var path = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Process);
        var pathSeparator = new string(Path.PathSeparator, 1);
        path = path is null
            ? string.Join(pathSeparator, directories)
            : string.Concat(string.Join(pathSeparator, directories), Path.PathSeparator, path);

        Environment.SetEnvironmentVariable("PATH", path, EnvironmentVariableTarget.Process);

        string RootedPath(string path)
        {
            return Path.IsPathRooted(path) ? path : Path.Combine(appDomain.BaseDirectory, path);
        }

        // get the path to the runtimes
        static IEnumerable<string> GetPotentialPaths()
        {
            const string Arm = "arm";
            const string Arm64 = "arm64";
            const string X64 = "x64";
            const string X86 = "x86";

            var architecture = RuntimeInformation.OSArchitecture switch
            {
                Architecture.Arm => Arm,
                Architecture.Arm64 when nint.Size is 4 => Arm,
                Architecture.Arm64 => Arm64,
                Architecture.X86 => X86,
                Architecture.X64 when nint.Size is 4 => X86,
                Architecture.X64 => X64,
                _ => throw new InvalidOperationException(),
            };

            yield return $"runtimes/{GetRidFront()}-{architecture}/native";
            yield return architecture;

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0046:Convert to conditional expression", Justification = "This would make it harder to read")]
            static string GetRidFront()
            {
                if (OperatingSystem.IsWindows())
                {
                    return "win";
                }

                if (OperatingSystem.IsLinux())
                {
                    return "linux";
                }

                if (OperatingSystem.IsMacOS())
                {
                    return "osx";
                }

                throw new InvalidOperationException();
            }
        }
    }
}