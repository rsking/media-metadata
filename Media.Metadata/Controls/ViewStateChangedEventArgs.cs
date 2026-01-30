// -----------------------------------------------------------------------
// <copyright file="ViewStateChangedEventArgs.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Media.Metadata.Controls;

/// <summary>
/// The <see cref="ListDetailsViewState"/> changed <see cref="EventArgs"/>.
/// </summary>
public class ViewStateChangedEventArgs : System.EventArgs
{
    /// <summary>
    /// Gets the view state.
    /// </summary>
    public required ListDetailsViewState ViewState { get; init; }
}