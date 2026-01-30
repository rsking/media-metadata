// -----------------------------------------------------------------------
// <copyright file="ListDetailsViewState.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Media.Metadata.Controls;

/// <summary>
/// The <see cref="ListDetailsView"/> state.
/// </summary>
public enum ListDetailsViewState
{
    /// <summary>
    /// Only the List view is shown.
    /// </summary>
    List,

    /// <summary>
    /// Only the Details view is shown.
    /// </summary>
    Details,

    /// <summary>
    /// Both the List and Details views are shown.
    /// </summary>
    Both,
}