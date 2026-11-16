// -----------------------------------------------------------------------
// <copyright file="MoovInfo.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Media.Metadata.Tracks;

/// <summary>
/// The MOOV info.
/// </summary>
/// <param name="TimeUnitPerSecond">The time unit per second.</param>
/// <param name="Tracks">The tracks.</param>
[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
internal readonly record struct MoovInfo(uint? TimeUnitPerSecond, TrakInfo[]? Tracks);