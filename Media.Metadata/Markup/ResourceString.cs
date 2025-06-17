// -----------------------------------------------------------------------
// <copyright file="ResourceString.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Media.Metadata.Markup;

/// <summary>
/// Gets the resource string.
/// </summary>
[Microsoft.UI.Xaml.Markup.MarkupExtensionReturnType(ReturnType = typeof(string))]
public sealed partial class ResourceString : Microsoft.UI.Xaml.Markup.MarkupExtension
{
    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    public string? Name { get; set; }

    /// <inheritdoc/>
    protected override object? ProvideValue() => this.Name is { } name
        ? UI.Properties.Resources.ResourceManager.GetString(name, UI.Properties.Resources.Culture)
        : default;
}