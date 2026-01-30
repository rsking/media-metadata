// -----------------------------------------------------------------------
// <copyright file="ListDetailsView.Events.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Media.Metadata.Controls;

using System;
using Microsoft.UI.Xaml.Controls;

/// <summary>
/// Panel that allows for a List/Details pattern.
/// </summary>
/// <seealso cref="ItemsControl" />
[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "MA0046:Use EventHandler<T> to declare events", Justification = "Checked")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Roslynator", "RCS1159:Use EventHandler<T>", Justification = "Checked")]
public partial class ListDetailsView
{
    /// <summary>
    /// Occurs when the currently selected item changes.
    /// </summary>
    public event SelectionChangedEventHandler? SelectionChanged;

    /// <summary>
    /// Occurs when the view state changes.
    /// </summary>
    public event EventHandler<ViewStateChangedEventArgs>? ViewStateChanged;

    private void OnSelectionChanged(SelectionChangedEventArgs e) => this.SelectionChanged?.Invoke(this, e);
}