// -----------------------------------------------------------------------
// <copyright file="ListDetailsView.Properties.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Media.Metadata.Controls;

using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

/// <summary>
/// Panel that allows for a List/Details pattern.
/// </summary>
/// <seealso cref="ItemsControl" />
public partial class ListDetailsView
{ /// <summary>
  /// Identifies the SelectedIndex dependency property.
  /// </summary>
    public static readonly DependencyProperty SelectedIndexProperty =
        DependencyProperty.Register(
            nameof(SelectedIndex),
            typeof(int),
            typeof(ListDetailsView),
            new PropertyMetadata(-1, OnSelectedIndexChanged));

    /// <summary>
    /// Identifies the <see cref="SelectedItem"/> dependency property.
    /// </summary>
    /// <returns>The identifier for the <see cref="SelectedItem"/> dependency property.</returns>
    public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register(
        nameof(SelectedItem),
        typeof(object),
        typeof(ListDetailsView),
        new PropertyMetadata(defaultValue: null, OnSelectedItemChanged));

    /// <summary>
    /// Identifies the <see cref="DetailsTemplate"/> dependency property.
    /// </summary>
    /// <returns>The identifier for the <see cref="DetailsTemplate"/> dependency property.</returns>
    public static readonly DependencyProperty DetailsTemplateProperty = DependencyProperty.Register(
        nameof(DetailsTemplate),
        typeof(DataTemplate),
        typeof(ListDetailsView),
        new PropertyMetadata(defaultValue: null));

    /// <summary>
    /// Identifies the <see cref="DetailsContentTemplateSelector"/> dependency property.
    /// </summary>
    /// <returns>The identifier for the <see cref="DetailsContentTemplateSelector"/> dependency property.</returns>
    public static readonly DependencyProperty DetailsContentTemplateSelectorProperty = DependencyProperty.Register(
        nameof(DetailsContentTemplateSelector),
        typeof(DataTemplateSelector),
        typeof(ListDetailsView),
        new PropertyMetadata(defaultValue: null));

    /// <summary>
    /// Identifies the <see cref="ListPaneItemTemplateSelector"/> dependency property.
    /// </summary>
    /// <returns>The identifier for the <see cref="ListPaneItemTemplateSelector"/> dependency property.</returns>
    public static readonly DependencyProperty ListPaneItemTemplateSelectorProperty = DependencyProperty.Register(
        nameof(ListPaneItemTemplateSelector),
        typeof(DataTemplateSelector),
        typeof(ListDetailsView),
        new PropertyMetadata(defaultValue: null));

    /// <summary>
    /// Identifies the <see cref="DetailsPaneBackground"/> dependency property.
    /// </summary>
    /// <returns>The identifier for the <see cref="DetailsPaneBackground"/> dependency property.</returns>
    public static readonly DependencyProperty DetailsPaneBackgroundProperty = DependencyProperty.Register(
        nameof(DetailsPaneBackground),
        typeof(Brush),
        typeof(ListDetailsView),
        new PropertyMetadata(defaultValue: null));

    /// <summary>
    /// Identifies the <see cref="ListPaneBackground"/> dependency property.
    /// </summary>
    /// <returns>The identifier for the <see cref="ListPaneBackground"/> dependency property.</returns>
    public static readonly DependencyProperty ListPaneBackgroundProperty = DependencyProperty.Register(
        nameof(ListPaneBackground),
        typeof(Brush),
        typeof(ListDetailsView),
        new PropertyMetadata(defaultValue: null));

    /// <summary>
    /// Identifies the <see cref="ListHeader"/> dependency property.
    /// </summary>
    /// <returns>The identifier for the <see cref="ListHeader"/> dependency property.</returns>
    public static readonly DependencyProperty ListHeaderProperty = DependencyProperty.Register(
        nameof(ListHeader),
        typeof(object),
        typeof(ListDetailsView),
        new PropertyMetadata(defaultValue: null, OnListHeaderChanged));

    /// <summary>
    /// Identifies the <see cref="ListHeaderTemplate"/> dependency property.
    /// </summary>
    /// <returns>The identifier for the <see cref="ListHeaderTemplate"/> dependency property.</returns>
    public static readonly DependencyProperty ListHeaderTemplateProperty = DependencyProperty.Register(
        nameof(ListHeaderTemplate),
        typeof(DataTemplate),
        typeof(ListDetailsView),
        new PropertyMetadata(defaultValue: null));

    /// <summary>
    /// Identifies the <see cref="ListPaneEmptyContent"/> dependency property.
    /// </summary>
    /// <returns>The identifier for the <see cref="ListPaneEmptyContent"/> dependency property.</returns>
    public static readonly DependencyProperty ListPaneEmptyContentProperty = DependencyProperty.Register(
        nameof(ListPaneEmptyContent),
        typeof(object),
        typeof(ListDetailsView),
        new PropertyMetadata(defaultValue: null));

    /// <summary>
    /// Identifies the <see cref="ListPaneEmptyContentTemplate"/> dependency property.
    /// </summary>
    /// <returns>The identifier for the <see cref="ListPaneEmptyContentTemplate"/> dependency property.</returns>
    public static readonly DependencyProperty ListPaneEmptyContentTemplateProperty = DependencyProperty.Register(
        nameof(ListPaneEmptyContentTemplate),
        typeof(DataTemplate),
        typeof(ListDetailsView),
        new PropertyMetadata(defaultValue: null));

    /// <summary>
    /// Identifies the <see cref="DetailsHeader"/> dependency property.
    /// </summary>
    /// <returns>The identifier for the <see cref="DetailsHeader"/> dependency property.</returns>
    public static readonly DependencyProperty DetailsHeaderProperty = DependencyProperty.Register(
        nameof(DetailsHeader),
        typeof(object),
        typeof(ListDetailsView),
        new PropertyMetadata(defaultValue: null));

    /// <summary>
    /// Identifies the <see cref="DetailsHeaderTemplate"/> dependency property.
    /// </summary>
    /// <returns>The identifier for the <see cref="DetailsHeaderTemplate"/> dependency property.</returns>
    public static readonly DependencyProperty DetailsHeaderTemplateProperty = DependencyProperty.Register(
        nameof(DetailsHeaderTemplate),
        typeof(DataTemplate),
        typeof(ListDetailsView),
        new PropertyMetadata(defaultValue: null));

    /// <summary>
    /// Identifies the <see cref="ListPaneWidth"/> dependency property.
    /// </summary>
    /// <returns>The identifier for the <see cref="ListPaneWidth"/> dependency property.</returns>
    public static readonly DependencyProperty ListPaneWidthProperty = DependencyProperty.Register(
        nameof(ListPaneWidth),
        typeof(double),
        typeof(ListDetailsView),
        new PropertyMetadata(320d, OnListPaneWidthChanged));

    /// <summary>
    /// Identifies the <see cref="NoSelectionContent"/> dependency property.
    /// </summary>
    /// <returns>The identifier for the <see cref="NoSelectionContent"/> dependency property.</returns>
    public static readonly DependencyProperty NoSelectionContentProperty = DependencyProperty.Register(
        nameof(NoSelectionContent),
        typeof(object),
        typeof(ListDetailsView),
        new PropertyMetadata(defaultValue: null));

    /// <summary>
    /// Identifies the <see cref="NoSelectionContentTemplate"/> dependency property.
    /// </summary>
    /// <returns>The identifier for the <see cref="NoSelectionContentTemplate"/> dependency property.</returns>
    public static readonly DependencyProperty NoSelectionContentTemplateProperty = DependencyProperty.Register(
        nameof(NoSelectionContentTemplate),
        typeof(DataTemplate),
        typeof(ListDetailsView),
        new PropertyMetadata(defaultValue: null));

    /// <summary>
    /// Identifies the <see cref="ViewState"/> dependency property.
    /// </summary>
    /// <returns>The identifier for the <see cref="ViewState"/> dependency property.</returns>
    public static readonly DependencyProperty ViewStateProperty = DependencyProperty.Register(
        nameof(ViewState),
        typeof(ListDetailsViewState),
        typeof(ListDetailsView),
        new PropertyMetadata(default(ListDetailsViewState)));

    /// <summary>
    /// Identifies the <see cref="ListCommandBar"/> dependency property.
    /// </summary>
    /// <returns>The identifier for the <see cref="ListCommandBar"/> dependency property.</returns>
    public static readonly DependencyProperty ListCommandBarProperty = DependencyProperty.Register(
        nameof(ListCommandBar),
        typeof(CommandBar),
        typeof(ListDetailsView),
        new PropertyMetadata(defaultValue: null, OnListCommandBarChanged));

    /// <summary>
    /// Identifies the <see cref="DetailsCommandBar"/> dependency property.
    /// </summary>
    /// <returns>The identifier for the <see cref="DetailsCommandBar"/> dependency property.</returns>
    public static readonly DependencyProperty DetailsCommandBarProperty = DependencyProperty.Register(
        nameof(DetailsCommandBar),
        typeof(CommandBar),
        typeof(ListDetailsView),
        new PropertyMetadata(defaultValue: null, OnDetailsCommandBarChanged));

    /// <summary>
    /// Identifies the <see cref="CompactModeThresholdWidth"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty CompactModeThresholdWidthProperty = DependencyProperty.Register(
        nameof(CompactModeThresholdWidth),
        typeof(double),
        typeof(ListDetailsView),
        new PropertyMetadata(640d));

    /// <summary>
    /// Identifies the <see cref="BackButtonBehavior"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty BackButtonBehaviorProperty = DependencyProperty.Register(
        nameof(BackButtonBehavior),
        typeof(BackButtonBehavior),
        typeof(ListDetailsView),
        new PropertyMetadata(BackButtonBehavior.System, OnBackButtonBehaviorChanged));

    /// <summary>
    /// Gets or sets the index of the current selection.
    /// </summary>
    /// <returns>The index of the current selection, or -1 if the selection is empty.</returns>
    public int SelectedIndex
    {
        get => (int)this.GetValue(SelectedIndexProperty);
        set => this.SetValue(SelectedIndexProperty, value);
    }

    /// <summary>
    /// Gets or sets the selected item.
    /// </summary>
    /// <returns>The selected item. The default is null.</returns>
    public object? SelectedItem
    {
        get => this.GetValue(SelectedItemProperty);
        set => this.SetValue(SelectedItemProperty, value);
    }

    /// <summary>
    /// Gets or sets the DataTemplate used to display the details.
    /// </summary>
    public DataTemplate? DetailsTemplate
    {
        get => (DataTemplate)this.GetValue(DetailsTemplateProperty);
        set => this.SetValue(DetailsTemplateProperty, value);
    }

    /// <summary>
    /// Gets or sets the <see cref="DataTemplateSelector"/> for the details presenter.
    /// </summary>
    public DataTemplateSelector? DetailsContentTemplateSelector
    {
        get => (DataTemplateSelector)this.GetValue(DetailsContentTemplateSelectorProperty);
        set => this.SetValue(DetailsContentTemplateSelectorProperty, value);
    }

    /// <summary>
    /// Gets or sets the <see cref="DataTemplateSelector"/> for the list pane items.
    /// </summary>
    public DataTemplateSelector? ListPaneItemTemplateSelector
    {
        get => (DataTemplateSelector)this.GetValue(ListPaneItemTemplateSelectorProperty);
        set => this.SetValue(ListPaneItemTemplateSelectorProperty, value);
    }

    /// <summary>
    /// Gets or sets the content for the list pane's header
    /// Gets or sets the Brush to apply to the background of the details area of the control.
    /// </summary>
    /// <returns>The Brush to apply to the background of the details area of the control.</returns>
    public Brush? DetailsPaneBackground
    {
        get => (Brush)this.GetValue(DetailsPaneBackgroundProperty);
        set => this.SetValue(DetailsPaneBackgroundProperty, value);
    }

    /// <summary>
    /// Gets or sets the Brush to apply to the background of the list area of the control.
    /// </summary>
    /// <returns>The Brush to apply to the background of the list area of the control.</returns>
    public Brush? ListPaneBackground
    {
        get => (Brush)this.GetValue(ListPaneBackgroundProperty);
        set => this.SetValue(ListPaneBackgroundProperty, value);
    }

    /// <summary>
    /// Gets or sets the content for the list pane's header.
    /// </summary>
    /// <returns>
    /// The content of the list pane's header. The default is null.
    /// </returns>
    public object? ListHeader
    {
        get => this.GetValue(ListHeaderProperty);
        set => this.SetValue(ListHeaderProperty, value);
    }

    /// <summary>
    /// Gets or sets the DataTemplate used to display the content of the list pane's header.
    /// </summary>
    /// <returns>
    /// The template that specifies the visualization of the list pane header object. The default is null.
    /// </returns>
    public DataTemplate? ListHeaderTemplate
    {
        get => (DataTemplate)this.GetValue(ListHeaderTemplateProperty);
        set => this.SetValue(ListHeaderTemplateProperty, value);
    }

    /// <summary>
    /// Gets or sets the content for the list pane's no items presenter.
    /// </summary>
    /// <returns>
    /// The content of the list pane's header. The default is null.
    /// </returns>
    public object? ListPaneEmptyContent
    {
        get => this.GetValue(ListPaneEmptyContentProperty);
        set => this.SetValue(ListPaneEmptyContentProperty, value);
    }

    /// <summary>
    /// Gets or sets the DataTemplate used to display the list pane's no items presenter.
    /// </summary>
    /// <returns>
    /// The template that specifies the visualization of the list pane no items object. The default is null.
    /// </returns>
    public DataTemplate? ListPaneEmptyContentTemplate
    {
        get => (DataTemplate)this.GetValue(ListPaneEmptyContentTemplateProperty);
        set => this.SetValue(ListPaneEmptyContentTemplateProperty, value);
    }

    /// <summary>
    /// Gets or sets the content for the details pane's header.
    /// </summary>
    /// <returns>
    /// The content of the details pane's header. The default is null.
    /// </returns>
    public object? DetailsHeader
    {
        get => this.GetValue(DetailsHeaderProperty);
        set => this.SetValue(DetailsHeaderProperty, value);
    }

    /// <summary>
    /// Gets or sets the DataTemplate used to display the content of the details pane's header.
    /// </summary>
    /// <returns>
    /// The template that specifies the visualization of the details pane header object. The default is null.
    /// </returns>
    public DataTemplate? DetailsHeaderTemplate
    {
        get => (DataTemplate)this.GetValue(DetailsHeaderTemplateProperty);
        set => this.SetValue(DetailsHeaderTemplateProperty, value);
    }

    /// <summary>
    /// Gets or sets the width of the list pane when the view is expanded.
    /// </summary>
    /// <returns>
    /// The width of the SplitView pane when it's fully expanded. The default is 320
    /// device-independent pixel (DIP).
    /// </returns>
    public double ListPaneWidth
    {
        get => (double)this.GetValue(ListPaneWidthProperty);
        set => this.SetValue(ListPaneWidthProperty, value);
    }

    /// <summary>
    /// Gets or sets the content to dsiplay when there is no item selected in the list.
    /// </summary>
    public object? NoSelectionContent
    {
        get => this.GetValue(NoSelectionContentProperty);
        set => this.SetValue(NoSelectionContentProperty, value);
    }

    /// <summary>
    /// Gets or sets the DataTemplate used to display the content when there is no selection.
    /// </summary>
    /// <returns>
    /// The template that specifies the visualization of the content when there is no
    /// selection. The default is null.
    /// </returns>
    public DataTemplate NoSelectionContentTemplate
    {
        get => (DataTemplate)this.GetValue(NoSelectionContentTemplateProperty);
        set => this.SetValue(NoSelectionContentTemplateProperty, value);
    }

    /// <summary>
    /// Gets the current visual state of the control.
    /// </summary>
    public ListDetailsViewState ViewState
    {
        get => (ListDetailsViewState)this.GetValue(ViewStateProperty);
        private set => this.SetValue(ViewStateProperty, value);
    }

    /// <summary>
    /// Gets or sets the <see cref="CommandBar"/> for the list section.
    /// </summary>
    public CommandBar ListCommandBar
    {
        get => (CommandBar)this.GetValue(ListCommandBarProperty);
        set => this.SetValue(ListCommandBarProperty, value);
    }

    /// <summary>
    /// Gets or sets the <see cref="CommandBar"/> for the details section.
    /// </summary>
    public CommandBar DetailsCommandBar
    {
        get => (CommandBar)this.GetValue(DetailsCommandBarProperty);
        set => this.SetValue(DetailsCommandBarProperty, value);
    }

    /// <summary>
    /// Gets or sets the Threshold width that will trigger the control to go into compact mode.
    /// </summary>
    public double CompactModeThresholdWidth
    {
        get => (double)this.GetValue(CompactModeThresholdWidthProperty);
        set => this.SetValue(CompactModeThresholdWidthProperty, value);
    }

    /// <summary>
    /// Gets or sets the behavior to use for the back button.
    /// </summary>
    /// <returns>The current BackButtonBehavior. The default is System.</returns>
    public BackButtonBehavior BackButtonBehavior
    {
        get => (BackButtonBehavior)this.GetValue(BackButtonBehaviorProperty);
        set => this.SetValue(BackButtonBehaviorProperty, value);
    }

    /// <summary>
    /// Gets or sets a function for mapping the selected item to a different model.
    /// This new model will be the DataContext of the Details area.
    /// </summary>
    public Func<object?, object?>? MapDetails { get; set; }

    private static void OnDetailsCommandBarChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => As<ListDetailsView>(d)?.OnDetailsCommandBarChanged();

    private static void OnListCommandBarChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => As<ListDetailsView>(d)?.OnListCommandBarChanged();

    private static void OnSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => As<ListDetailsView>(d)?.OnSelectedItemChanged(e);

    private static void OnSelectedIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => As<ListDetailsView>(d)?.OnSelectedIndexChanged(e);

    private static void OnBackButtonBehaviorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => As<ListDetailsView>(d)?.SetBackButtonVisibility();

    private static void OnListHeaderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => As<ListDetailsView>(d)?.SetListHeaderVisibility();

    private static void OnListPaneWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => As<ListDetailsView>(d)?.OnListPaneWidthChanged();

    private static T? As<T>(DependencyObject d)
        where T : class => d as T;
}