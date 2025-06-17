// -----------------------------------------------------------------------
// <copyright file="EditableVideoTemplateSelector.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Media.Metadata.Controls;

/// <summary>
/// The <see cref="Models.VideoViewModel"/> <see cref="Microsoft.UI.Xaml.Controls.DataTemplateSelector"/>.
/// </summary>
internal sealed partial class EditableVideoTemplateSelector : Microsoft.UI.Xaml.Controls.DataTemplateSelector
{
    /// <summary>
    /// Gets or sets the video template.
    /// </summary>
    public Microsoft.UI.Xaml.DataTemplate? VideoTemplate { get; set; }

    /// <summary>
    /// Gets or sets the episode template.
    /// </summary>
    public Microsoft.UI.Xaml.DataTemplate? EpisodeTemplate { get; set; }

    /// <summary>
    /// Gets or sets the movie template.
    /// </summary>
    public Microsoft.UI.Xaml.DataTemplate? MovieTemplate { get; set; }

    /// <inheritdoc/>
    protected override Microsoft.UI.Xaml.DataTemplate? SelectTemplateCore(object item) => item switch
    {
        ViewModels.EpisodeViewModel or Episode => this.EpisodeTemplate,
        ViewModels.MovieViewModel or Movie => this.MovieTemplate,
        ViewModels.VideoViewModel or Video => this.VideoTemplate,
        _ => base.SelectTemplateCore(item),
    };

    /// <inheritdoc/>
    protected override Microsoft.UI.Xaml.DataTemplate? SelectTemplateCore(object item, Microsoft.UI.Xaml.DependencyObject container) => this.SelectTemplateCore(item);
}