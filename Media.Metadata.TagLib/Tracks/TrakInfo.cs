// -----------------------------------------------------------------------
// <copyright file="TrakInfo.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Media.Metadata.Tracks;

/// <summary>
/// The TRAK info.
/// </summary>
/// <param name="Id">The ID.</param>
/// <param name="Type">The type.</param>
/// <param name="Samples">The samples.</param>
/// <param name="Durations">The durations.</param>
/// <param name="FrameCount">The frame count.</param>
/// <param name="Chaps">The chapters.</param>
/// <param name="TimeUnitPerSecond">The time unit per second.</param>
/// <param name="Duration">The duration.</param>
/// <param name="Language">The language.</param>
[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
internal readonly record struct TrakInfo(uint? Id, string? Type, IReadOnlyList<long>? Samples, IReadOnlyList<uint>? Durations, IReadOnlyList<uint>? FrameCount, IReadOnlyList<uint>? Chaps, uint? TimeUnitPerSecond, ulong? Duration, string? Language);