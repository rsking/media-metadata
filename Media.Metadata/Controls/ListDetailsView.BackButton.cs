// -----------------------------------------------------------------------
// <copyright file="ListDetailsView.BackButton.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Media.Metadata.Controls;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Windows.ApplicationModel;
using Windows.UI.Core;

/// <summary>
/// Panel that allows for a List/Details pattern.
/// </summary>
/// <seealso cref="ItemsControl" />
[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1010: Collections should implement generic interface", Justification = "This is a control which is not strongly typed")]
public partial class ListDetailsView
{
    private AppViewBackButtonVisibility? previousSystemBackButtonVisibility;
    private bool previousNavigationViewBackEnabled;

    // Int used because the underlying type is an enum, but we don't have access to the enum
    private int previousNavigationViewBackVisibilty;
    private Button? inlineBackButton;
    private object? navigationView;
    private Frame? frame;

    private void SetBackButtonVisibility(ListDetailsViewState? previousState = null)
    {
        const int BackButtonVisible = 1;

        if (DesignMode.DesignModeEnabled)
        {
            return;
        }

        var navigationManager = Window.Current is null ? null : SystemNavigationManager.GetForCurrentView();

        if (this.ViewState is ListDetailsViewState.Details)
        {
            if (this.BackButtonBehavior is BackButtonBehavior.Inline && this.inlineBackButton is not null)
            {
                this.inlineBackButton.Visibility = Visibility.Visible;
            }
            else if (this.BackButtonBehavior is BackButtonBehavior.Automatic)
            {
                // Continue to support the system back button if it is being used
                if (navigationManager?.AppViewBackButtonVisibility is AppViewBackButtonVisibility.Visible)
                {
                    // Setting this indicates that the system back button is being used
                    this.previousSystemBackButtonVisibility = navigationManager.AppViewBackButtonVisibility;
                }
                else if (this.inlineBackButton is not null && (this.navigationView is null || this.frame is null))
                {
                    // We can only use the new NavigationView if we also have a Frame
                    // If there is no frame we have to use the inline button
                    this.inlineBackButton.Visibility = Visibility.Visible;
                }
                else
                {
                    this.SetNavigationViewBackButtonState(BackButtonVisible, enabled: true);
                }
            }
            else if (this.BackButtonBehavior is not BackButtonBehavior.Manual && navigationManager is not null)
            {
                this.previousSystemBackButtonVisibility = navigationManager.AppViewBackButtonVisibility;

                navigationManager.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            }
        }
        else if (previousState is ListDetailsViewState.Details)
        {
            if (this.BackButtonBehavior is BackButtonBehavior.Inline && this.inlineBackButton is not null)
            {
                this.inlineBackButton.Visibility = Visibility.Collapsed;
            }
            else if (this.BackButtonBehavior is BackButtonBehavior.Automatic && !this.previousSystemBackButtonVisibility.HasValue)
            {
                if (this.inlineBackButton is not null && (this.navigationView is null || this.frame is null))
                {
                    this.inlineBackButton.Visibility = Visibility.Collapsed;
                }
                else
                {
                    this.SetNavigationViewBackButtonState(this.previousNavigationViewBackVisibilty, this.previousNavigationViewBackEnabled);
                }
            }

            if (this.previousSystemBackButtonVisibility.HasValue)
            {
                // Make sure we show the back button if the stack can navigate back
                navigationManager?.AppViewBackButtonVisibility = this.previousSystemBackButtonVisibility.Value;

                this.previousSystemBackButtonVisibility = null;
            }
        }
    }

    private void SetNavigationViewBackButtonState(int visible, bool enabled)
    {
        if (this.navigationView is null)
        {
            return;
        }

        var navType = this.navigationView.GetType();
        if (navType.GetProperty("IsBackButtonVisible") is { } visibleProperty && visibleProperty.GetValue(this.navigationView) is int intValue)
        {
            this.previousNavigationViewBackVisibilty = intValue;
            visibleProperty.SetValue(this.navigationView, visible);
        }

        if (navType.GetProperty("IsBackEnabled") is { } enabledProperty && enabledProperty.GetValue(this.navigationView) is bool boolValue)
        {
            this.previousNavigationViewBackEnabled = boolValue;
            enabledProperty.SetValue(this.navigationView, enabled);
        }
    }

    private void OnFrameNavigating(object? sender, NavigatingCancelEventArgs args)
    {
        if (args.NavigationMode is NavigationMode.Back && this.ViewState is ListDetailsViewState.Details)
        {
            this.ClearSelectedItem();
            args.Cancel = true;
        }
    }

    private void OnBackRequested(object? sender, BackRequestedEventArgs args)
    {
        if (this.ViewState is ListDetailsViewState.Details)
        {
            // let the OnFrameNavigating method handle it if
            if (this.frame?.CanGoBack != true)
            {
                this.ClearSelectedItem();
            }

            args.Handled = true;
        }
    }

    private void OnInlineBackButtonClicked(object? sender, RoutedEventArgs e) => this.ClearSelectedItem();
}