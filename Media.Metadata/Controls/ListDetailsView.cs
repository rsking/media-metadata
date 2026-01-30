// -----------------------------------------------------------------------
// <copyright file="ListDetailsView.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Media.Metadata.Controls;

using CommunityToolkit.WinUI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Windows.ApplicationModel;
using Windows.UI.Core;

/// <summary>
/// Panel that allows for a List/Details pattern.
/// </summary>
[TemplatePart(Name = PartDetailsPresenter, Type = typeof(ContentPresenter))]
[TemplatePart(Name = PartDetailsPanel, Type = typeof(FrameworkElement))]
[TemplateVisualState(Name = NoSelectionNarrowState, GroupName = SelectionStates)]
[TemplateVisualState(Name = NoSelectionWideState, GroupName = SelectionStates)]
[TemplateVisualState(Name = HasSelectionWideState, GroupName = SelectionStates)]
[TemplateVisualState(Name = HasSelectionNarrowState, GroupName = SelectionStates)]
public partial class ListDetailsView : ItemsControl
{
    // All view states:
    private const string SelectionStates = "SelectionStates";
    private const string NoSelectionWideState = "NoSelectionWide";
    private const string HasSelectionWideState = "HasSelectionWide";
    private const string NoSelectionNarrowState = "NoSelectionNarrow";
    private const string HasSelectionNarrowState = "HasSelectionNarrow";

    private const string HasItemsState = "HasItemsState";
    private const string HasNoItemsState = "HasNoItemsState";

    // Control names:
    private const string PartRootPanel = "RootPanel";
    private const string PartDetailsPresenter = "DetailsPresenter";
    private const string PartDetailsPanel = "DetailsPanel";
    private const string PartMainList = "MainList";
    private const string PartBackButton = "ListDetailsBackButton";
    private const string PartHeaderContentPresenter = "HeaderContentPresenter";

    private ContentPresenter? detailsPresenter;
    private TwoPaneView? twoPaneView;
    private VisualStateGroup? selectionStateGroup;

    /// <summary>
    /// Initialises a new instance of the <see cref="ListDetailsView"/> class.
    /// </summary>
    public ListDetailsView()
    {
        this.DefaultStyleKey = typeof(ListDetailsView);

        this.Loaded += this.OnLoaded;
        this.Unloaded += this.OnUnloaded;
    }

    /// <summary>
    /// Clears the <see cref="SelectedItem"/> and prevent flickering of the UI if only the order of the items changed.
    /// </summary>
    public void ClearSelectedItem() => this.SelectedItem = null;

    /// <inheritdoc />
    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        if (this.inlineBackButton is not null)
        {
            this.inlineBackButton.Click -= this.OnInlineBackButtonClicked;
        }

        this.inlineBackButton = (Button)this.GetTemplateChild(PartBackButton);
        if (this.inlineBackButton is not null)
        {
            this.inlineBackButton.Click += this.OnInlineBackButtonClicked;
        }

        this.selectionStateGroup = (VisualStateGroup)this.GetTemplateChild(SelectionStates);
        if (this.selectionStateGroup is not null)
        {
            this.selectionStateGroup.CurrentStateChanged += this.OnSelectionStateChanged;
        }

        this.twoPaneView = (TwoPaneView)this.GetTemplateChild(PartRootPanel);
        if (this.twoPaneView is not null)
        {
            this.twoPaneView.ModeChanged += this.OnModeChanged;
        }

        this.detailsPresenter = (ContentPresenter)this.GetTemplateChild(PartDetailsPresenter);
        this.SetDetailsContent();

        this.SetListHeaderVisibility();
        this.OnDetailsCommandBarChanged();
        this.OnListCommandBarChanged();
        this.OnListPaneWidthChanged();

        this.UpdateView(animate: true);
    }

    /// <inheritdoc />
    protected override void OnItemsChanged(object e)
    {
        base.OnItemsChanged(e);
        this.UpdateView(animate: true);

        if (this.SelectedIndex < 0)
        {
            return;
        }

        // Ensure we still have the correct index and selected item for the new collection.
        // This prevents flickering when the order of the collection changes.
        int index = -1;
        if (this.Items is not null)
        {
            index = this.Items.IndexOf(this.SelectedItem);
        }

        if (index < 0)
        {
            this.ClearSelectedItem();
        }
        else if (this.SelectedIndex != index)
        {
            this.SetValue(SelectedIndexProperty, index);
        }
    }

    private void OnSelectedIndexChanged(DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue is int newIndex)
        {
            var newItem = newIndex >= 0 && this.Items.Count > newIndex
                ? this.Items[newIndex]
                : null;
            var oldItem = e.OldValue is int oldIndex && oldIndex >= 0 && this.Items.Count > oldIndex
                ? this.Items[oldIndex]
                : null;
            if (this.SelectedItem != newItem)
            {
                if (newItem is null)
                {
                    this.ClearSelectedItem();
                }
                else
                {
                    this.SetValue(SelectedItemProperty, newItem);
                    this.UpdateSelection(oldItem, newItem);
                }
            }
        }
    }

    private void OnSelectedItemChanged(DependencyPropertyChangedEventArgs e)
    {
        int index = this.SelectedItem is null ? -1 : this.Items.IndexOf(this.SelectedItem);

        // If there is no selection, do not remove the DetailsPresenter content but let it animate out.
        if (index >= 0)
        {
            this.SetDetailsContent();
        }

        if (this.SelectedIndex != index)
        {
            this.SetValue(SelectedIndexProperty, index);
            this.UpdateSelection(e.OldValue, e.NewValue);
        }

        this.OnSelectionChanged(new SelectionChangedEventArgs([e.OldValue], [e.NewValue]));
        this.UpdateView(animate: true);
        this.SetFocus(this.ViewState);
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (!DesignMode.DesignModeEnabled)
        {
            if (Window.Current is not null)
            {
                SystemNavigationManager.GetForCurrentView().BackRequested += this.OnBackRequested;
            }

            if (this.frame is not null)
            {
                this.frame.Navigating -= this.OnFrameNavigating;
            }

            this.navigationView = this.FindAscendant<NavigationView>();
            this.frame = this.FindAscendant<Frame>();
            if (this.frame is not null)
            {
                this.frame.Navigating += this.OnFrameNavigating;
            }
        }
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        if (!DesignMode.DesignModeEnabled)
        {
            if (Window.Current is not null)
            {
                SystemNavigationManager.GetForCurrentView().BackRequested -= this.OnBackRequested;
            }

            if (this.frame is not null)
            {
                this.frame.Navigating -= this.OnFrameNavigating;
            }

            this.selectionStateGroup = (VisualStateGroup)this.GetTemplateChild(SelectionStates);
            if (this.selectionStateGroup is not null)
            {
                this.selectionStateGroup.CurrentStateChanged -= this.OnSelectionStateChanged;
                this.selectionStateGroup = null;
            }
        }
    }

    private void UpdateSelection(object? oldSelection, object? newSelection)
    {
        this.OnSelectionChanged(new SelectionChangedEventArgs([oldSelection], [newSelection]));

        this.UpdateView(animate: true);

        // If there is no selection, do not remove the DetailsPresenter content but let it animate out.
        if (this.SelectedItem is not null)
        {
            this.SetDetailsContent();
        }
    }

    private void SetListHeaderVisibility()
    {
        if (this.GetTemplateChild(PartHeaderContentPresenter) is FrameworkElement headerPresenter)
        {
            headerPresenter.Visibility = this.ListHeader is not null
                ? Visibility.Visible
                : Visibility.Collapsed;
        }
    }

    private void UpdateView(bool animate)
    {
        this.UpdateViewState();
        this.SetVisualState(animate);
    }

    private void UpdateViewState()
    {
        var previousState = this.ViewState;

        if (this.twoPaneView is null)
        {
            this.ViewState = ListDetailsViewState.Both;
        }

        // Single pane:
        else if (this.twoPaneView.Mode is TwoPaneViewMode.SinglePane)
        {
            this.ViewState = this.SelectedItem is null ? ListDetailsViewState.List : ListDetailsViewState.Details;
            this.twoPaneView.PanePriority = this.SelectedItem is null ? TwoPaneViewPriority.Pane1 : TwoPaneViewPriority.Pane2;
        }

        // Dual pane:
        else
        {
            this.ViewState = ListDetailsViewState.Both;
        }

        if (previousState != this.ViewState)
        {
            this.ViewStateChanged?.Invoke(this, new ViewStateChangedEventArgs { ViewState = this.ViewState });
            this.SetBackButtonVisibility(previousState);
        }
    }

    private void SetVisualState(bool animate)
    {
        var (noSelectionState, hasSelectionState) = this.ViewState switch
        {
            ListDetailsViewState.Both => (NoSelectionWideState, HasSelectionWideState),
            _ => (NoSelectionNarrowState, HasSelectionNarrowState),
        };

        VisualStateManager.GoToState(this, this.SelectedItem is null ? noSelectionState : hasSelectionState, animate);
        VisualStateManager.GoToState(this, this.Items.Count > 0 ? HasItemsState : HasNoItemsState, animate);
    }

    /// <summary>
    /// Sets the content of the <see cref="SelectedItem"/> based on current <see cref="MapDetails"/> function.
    /// </summary>
    private void SetDetailsContent()
    {
        if (this.detailsPresenter is not null)
        {
            // Update the content template:
            if (this.detailsPresenter.ContentTemplateSelector is not null)
            {
                this.detailsPresenter.ContentTemplate = this.detailsPresenter.ContentTemplateSelector.SelectTemplate(this.SelectedItem, this.detailsPresenter);
            }

            this.detailsPresenter.Content = (this.MapDetails, this.SelectedItem) switch
            {
                ({ } mapDetails, { } selectedItem) => mapDetails(selectedItem),
                (null, var selectedItem) => selectedItem,
                _ => null,
            };
        }
    }

    private void OnListCommandBarChanged() => this.OnCommandBarChanged("ListCommandBar", this.ListCommandBar);

    private void OnDetailsCommandBarChanged() => this.OnCommandBarChanged("DetailsCommandBar", this.DetailsCommandBar);

    private void OnCommandBarChanged(string panelName, CommandBar? commandbar)
    {
        if (this.GetTemplateChild(panelName) is not Panel panel)
        {
            return;
        }

        panel.Children.Clear();
        if (commandbar is not null)
        {
            panel.Children.Add(commandbar);
        }
    }

    private void SetListSelectionWithKeyboardFocusOnVisualStateChanged(ListDetailsViewState viewState) =>
        this.SetListSelectionWithKeyboardFocus(singleSelectionFollowsFocus: viewState is ListDetailsViewState.Both);

    private void SetListSelectionWithKeyboardFocus(bool singleSelectionFollowsFocus)
    {
        if (this.GetTemplateChild("List") is ListViewBase list)
        {
            list.SingleSelectionFollowsFocus = singleSelectionFollowsFocus;
        }
    }

    private void OnSelectionStateChanged(object sender, VisualStateChangedEventArgs e)
    {
        this.SetFocus(this.ViewState);
        this.SetListSelectionWithKeyboardFocusOnVisualStateChanged(this.ViewState);
    }

    private void SetFocus(ListDetailsViewState viewState)
    {
        if (viewState is not ListDetailsViewState.Details)
        {
            this.FocusItemList();
        }
        else
        {
            this.FocusFirstFocusableElementInDetails();
        }
    }

    private void FocusFirstFocusableElementInDetails()
    {
        if (this.GetTemplateChild(PartDetailsPanel) is { } details
            && FocusManager.FindFirstFocusableElement(details) is Control focusableElement)
        {
            focusableElement.Focus(FocusState.Programmatic);
        }
    }

    private void FocusItemList()
    {
        if (this.GetTemplateChild(PartMainList) is Control list)
        {
            list.Focus(FocusState.Programmatic);
        }
    }

    private void OnModeChanged(TwoPaneView sender, object args)
    {
        this.UpdateView(animate: true);
        this.SetListSelectionWithKeyboardFocusOnVisualStateChanged(this.ViewState);
    }

    private void OnListPaneWidthChanged() => this.twoPaneView?.Pane1Length = new GridLength(this.ListPaneWidth);
}