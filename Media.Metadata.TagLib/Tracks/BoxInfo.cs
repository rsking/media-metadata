// -----------------------------------------------------------------------
// <copyright file="BoxInfo.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Media.Metadata.Tracks;

/// <summary>
/// The BOX info.
/// </summary>
/// <param name="Offset">The offset.</param>
/// <param name="BoxOffset">The box offset.</param>
/// <param name="Last">Whether this instance is last.</param>
/// <param name="Type">The type.</param>
[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
internal readonly record struct BoxInfo(long Offset, long BoxOffset, bool Last, AsciiType Type);