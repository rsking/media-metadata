// -----------------------------------------------------------------------
// <copyright file="ChapterInfo.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Media.Metadata.Tracks;

/// <summary>
/// The chapter info.
/// </summary>
/// <param name="Time">The time.</param>
/// <param name="Name">The name.</param>
[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
internal readonly record struct ChapterInfo(TimeSpan Time, string Name);