// -----------------------------------------------------------------------
// <copyright file="DisplayAttribute.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Media.Metadata;

using System.Reflection;

/// <summary>
/// <see cref="DisplayAttribute"/> is a general-purpose attribute to specify user-visible globalizable strings for types and members.
/// The string properties of this class can be used either as literals or as resource identifiers into a specified <see cref="ResourceType" />.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
public sealed class DisplayAttribute : Attribute
{
    private readonly LocalizableString description = new(nameof(Description));
    private readonly LocalizableString groupName = new(nameof(GroupName));
    private readonly LocalizableString name = new(nameof(Name));
    private readonly LocalizableString prompt = new(nameof(Prompt));
    private readonly LocalizableString shortName = new(nameof(ShortName));
    private bool? autoGenerateField;
    private bool? autoGenerateFilter;
    private int? order;

    /// <summary>
    ///     Gets or sets the ShortName attribute property, which may be a resource key string.
    ///     <para>
    ///         Consumers must use the <see cref="GetShortName" /> method to retrieve the UI display string.
    ///     </para>
    /// </summary>
    /// <remarks>
    ///     The property contains either the literal, non-localized string or the resource key
    ///     to be used in conjunction with <see cref="ResourceType" /> to configure a localized
    ///     short name for display.
    ///     <para>
    ///         The <see cref="GetShortName" /> method will return either the literal, non-localized
    ///         string or the localized string when <see cref="ResourceType" /> has been specified.
    ///     </para>
    /// </remarks>
    /// <value>
    ///     The short name is generally used as the grid column label for a UI element bound to the member
    ///     bearing this attribute.  A <see langword="null"/> or empty string is legal, and consumers must allow for that.
    /// </value>
    public string? ShortName
    {
        get => this.shortName.Value;
        set => this.shortName.Value = value;
    }

    /// <summary>
    ///     Gets or sets the Name attribute property, which may be a resource key string.
    ///     <para>
    ///         Consumers must use the <see cref="GetName" /> method to retrieve the UI display string.
    ///     </para>
    /// </summary>
    /// <remarks>
    ///     The property contains either the literal, non-localized string or the resource key
    ///     to be used in conjunction with <see cref="ResourceType" /> to configure a localized
    ///     name for display.
    ///     <para>
    ///         The <see cref="GetName" /> method will return either the literal, non-localized
    ///         string or the localized string when <see cref="ResourceType" /> has been specified.
    ///     </para>
    /// </remarks>
    /// <value>
    ///     The name is generally used as the field label for a UI element bound to the member
    ///     bearing this attribute.  A <see langword="null"/> or empty string is legal, and consumers must allow for that.
    /// </value>
    public string? Name
    {
        get => this.name.Value;
        set => this.name.Value = value;
    }

    /// <summary>
    ///     Gets or sets the Description attribute property, which may be a resource key string.
    ///     <para>
    ///         Consumers must use the <see cref="GetDescription" /> method to retrieve the UI display string.
    ///     </para>
    /// </summary>
    /// <remarks>
    ///     The property contains either the literal, non-localized string or the resource key
    ///     to be used in conjunction with <see cref="ResourceType" /> to configure a localized
    ///     description for display.
    ///     <para>
    ///         The <see cref="GetDescription" /> method will return either the literal, non-localized
    ///         string or the localized string when <see cref="ResourceType" /> has been specified.
    ///     </para>
    /// </remarks>
    /// <value>
    ///     Description is generally used as a tool tip or description a UI element bound to the member
    ///     bearing this attribute.  A <see langword="null"/> or empty string is legal, and consumers must allow for that.
    /// </value>
    public string? Description
    {
        get => this.description.Value;
        set => this.description.Value = value;
    }

    /// <summary>
    ///     Gets or sets the Prompt attribute property, which may be a resource key string.
    ///     <para>
    ///         Consumers must use the <see cref="GetPrompt" /> method to retrieve the UI display string.
    ///     </para>
    /// </summary>
    /// <remarks>
    ///     The property contains either the literal, non-localized string or the resource key
    ///     to be used in conjunction with <see cref="ResourceType" /> to configure a localized
    ///     prompt for display.
    ///     <para>
    ///         The <see cref="GetPrompt" /> method will return either the literal, non-localized
    ///         string or the localized string when <see cref="ResourceType" /> has been specified.
    ///     </para>
    /// </remarks>
    /// <value>
    ///     A prompt is generally used as a prompt or watermark for a UI element bound to the member
    ///     bearing this attribute.  A <see langword="null"/> or empty string is legal, and consumers must allow for that.
    /// </value>
    public string? Prompt
    {
        get => this.prompt.Value;
        set => this.prompt.Value = value;
    }

    /// <summary>
    ///     Gets or sets the GroupName attribute property, which may be a resource key string.
    ///     <para>
    ///         Consumers must use the <see cref="GetGroupName" /> method to retrieve the UI display string.
    ///     </para>
    /// </summary>
    /// <remarks>
    ///     The property contains either the literal, non-localized string or the resource key
    ///     to be used in conjunction with <see cref="ResourceType" /> to configure a localized
    ///     group name for display.
    ///     <para>
    ///         The <see cref="GetGroupName" /> method will return either the literal, non-localized
    ///         string or the localized string when <see cref="ResourceType" /> has been specified.
    ///     </para>
    /// </remarks>
    /// <value>
    ///     A group name is used for grouping fields into the UI.  A <see langword="null"/> or empty string is legal,
    ///     and consumers must allow for that.
    /// </value>
    public string? GroupName
    {
        get => this.groupName.Value;
        set => this.groupName.Value = value;
    }

    /// <summary>
    ///     Gets or sets the <see cref="Type" /> that contains the resources for <see cref="ShortName" />,
    ///     <see cref="Name" />, <see cref="Description" />, <see cref="Prompt" />, and <see cref="GroupName" />.
    ///     Using <see cref="ResourceType" /> along with these Key properties, allows the <see cref="GetShortName" />,
    ///     <see cref="GetName" />, <see cref="GetDescription" />, <see cref="GetPrompt" />, and <see cref="GetGroupName" />
    ///     methods to return localized values.
    /// </summary>
    [field: System.Diagnostics.CodeAnalysis.DynamicallyAccessedMembers(System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.PublicProperties)]
    [System.Diagnostics.CodeAnalysis.DynamicallyAccessedMembers(System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.PublicProperties)]
    public Type? ResourceType
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;

                this.shortName.ResourceType = value;
                this.name.ResourceType = value;
                this.description.ResourceType = value;
                this.prompt.ResourceType = value;
                this.groupName.ResourceType = value;
            }
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether UI should be generated automatically to display this field.
    /// If this property is not set then the presentation layer will automatically determine whether UI should be generated.
    /// Setting this property allows an override of the default behavior of the presentation layer.
    /// <para>Consumers must use the <see cref="GetAutoGenerateField" /> method to retrieve the value, as this property getter will throw an exception if the value has not been set.</para>
    /// </summary>
    /// <exception cref="InvalidOperationException">If the getter of this property is invoked when the value has not been explicitly set using the setter.</exception>
    public bool AutoGenerateField
    {
        get => this.autoGenerateField.HasValue ? this.autoGenerateField.GetValueOrDefault() : throw new InvalidOperationException("DisplayAttribute_PropertyNotSet");
        set => this.autoGenerateField = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether UI should be generated automatically to display filtering for this field.
    /// If this property is not set then the presentation layer will automatically determine whether filtering UI should be generated.
    /// Setting this property allows an override of the default behavior of the presentation layer.
    /// <para>Consumers must use the <see cref="GetAutoGenerateFilter" /> method to retrieve the value, as this property getter will throw an exception if the value has not been set.</para>
    /// </summary>
    /// <exception cref="InvalidOperationException">If the getter of this property is invoked when the value has not been explicitly set using the setter.</exception>
    public bool AutoGenerateFilter
    {
        get => this.autoGenerateFilter.HasValue ? this.autoGenerateFilter.GetValueOrDefault() : throw new InvalidOperationException("DisplayAttribute_PropertyNotSet");
        set => this.autoGenerateFilter = value;
    }

    /// <summary>
    ///     Gets or sets the order in which this field should be displayed.  If this property is not set then
    ///     the presentation layer will automatically determine the order.  Setting this property explicitly
    ///     allows an override of the default behavior of the presentation layer.
    ///     <para>
    ///         Consumers must use the <see cref="GetOrder" /> method to retrieve the value, as this property getter will throw
    ///         an exception if the value has not been set.
    ///     </para>
    /// </summary>
    /// <exception cref="InvalidOperationException">
    ///     If the getter of this property is invoked when the value has not been explicitly set using the setter.
    /// </exception>
    public int Order
    {
        get => this.order.HasValue ? this.order.GetValueOrDefault() : throw new InvalidOperationException("DisplayAttribute_PropertyNotSet");
        set => this.order = value;
    }

    /// <summary>
    /// Gets the UI display string for ShortName.
    /// <para>This can be either a literal, non-localized string provided to <see cref="ShortName" /> or the localized string found when <see cref="ResourceType" /> has been specified and <see cref="ShortName" /> represents a resource key within that resource type.</para>
    /// </summary>
    /// <returns>
    /// When <see cref="ResourceType" /> has not been specified, the value of <see cref="ShortName" /> will be returned.
    /// <para>When <see cref="ResourceType" /> has been specified and <see cref="ShortName" /> represents a resource key within that resource type, then the localized value will be returned.</para>
    /// <para>If <see cref="ShortName" /> is <see langword="null"/>, the value from <see cref="GetName" /> will be returned.</para>
    /// </returns>
    /// <exception cref="InvalidOperationException">After setting both the <see cref="ResourceType" /> property and the <see cref="ShortName" /> property, but a public static property with a name matching the <see cref="ShortName" /> value couldn't be found on the <see cref="ResourceType" />.</exception>
    public string? GetShortName() => this.shortName.GetLocalizableValue() ?? this.GetName();

    /// <summary>
    /// Gets the UI display string for Name.
    /// <para>This can be either a literal, non-localized string provided to <see cref="Name" /> or the localized string found when <see cref="ResourceType" /> has been specified and <see cref="Name" /> represents a resource key within that resource type.</para>
    /// </summary>
    /// <returns>
    /// When <see cref="ResourceType" /> has not been specified, the value of <see cref="Name" /> will be returned.
    /// <para>When <see cref="ResourceType" /> has been specified and <see cref="Name" /> represents a resource key within that resource type, then the localized value will be returned.</para>
    /// <para>Can return <see langword="null"/> and will not fall back onto other values, as it's more likely for the consumer to want to fall back onto the property name.</para>
    /// </returns>
    /// <exception cref="InvalidOperationException">After setting both the <see cref="ResourceType" /> property and the <see cref="Name" /> property, but a public static property with a name matching the <see cref="Name" /> value couldn't be found on the <see cref="ResourceType" />.</exception>
    public string? GetName() => this.name.GetLocalizableValue();

    /// <summary>
    /// Gets the UI display string for Description.
    /// <para>This can be either a literal, non-localized string provided to <see cref="Description" /> or the localized string found when <see cref="ResourceType" /> has been specified and <see cref="Description" /> represents a resource key within that resource type.</para>
    /// </summary>
    /// <returns>
    /// When <see cref="ResourceType" /> has not been specified, the value of <see cref="Description" /> will be returned.
    /// <para>When <see cref="ResourceType" /> has been specified and <see cref="Description" /> represents a resource key within that resource type, then the localized value will be returned.</para>
    /// </returns>
    /// <exception cref="InvalidOperationException">After setting both the <see cref="ResourceType" /> property and the <see cref="Description" /> property, but a public static property with a name matching the <see cref="Description" /> value couldn't be found on the <see cref="ResourceType" />.</exception>
    public string? GetDescription() => this.description.GetLocalizableValue();

    /// <summary>
    /// Gets the UI display string for Prompt.
    /// <para>This can be either a literal, non-localized string provided to <see cref="Prompt" /> or the localized string found when <see cref="ResourceType" /> has been specified and <see cref="Prompt" /> represents a resource key within that resource type.</para>
    /// </summary>
    /// <returns>
    /// When <see cref="ResourceType" /> has not been specified, the value of <see cref="Prompt" /> will be returned.
    /// <para>When <see cref="ResourceType" /> has been specified and <see cref="Prompt" /> represents a resource key within that resource type, then the localized value will be returned.</para>
    /// </returns>
    /// <exception cref="InvalidOperationException">After setting both the <see cref="ResourceType" /> property and the <see cref="Prompt" /> property, but a public static property with a name matching the <see cref="Prompt" /> value couldn't be found on the <see cref="ResourceType" />.</exception>
    public string? GetPrompt() => this.prompt.GetLocalizableValue();

    /// <summary>
    /// Gets the UI display string for GroupName.
    /// <para>This can be either a literal, non-localized string provided to <see cref="GroupName" /> or the localized string found when <see cref="ResourceType" /> has been specified and <see cref="GroupName" /> represents a resource key within that resource type.</para>
    /// </summary>
    /// <returns>
    /// When <see cref="ResourceType" /> has not been specified, the value of <see cref="GroupName" /> will be returned.
    /// <para>When <see cref="ResourceType" /> has been specified and <see cref="GroupName" /> represents a resource key within that resource type, then the localized value will be returned.</para>
    /// </returns>
    /// <exception cref="InvalidOperationException">After setting both the <see cref="ResourceType" /> property and the <see cref="GroupName" /> property, but a public static property with a name matching the <see cref="GroupName" /> value couldn't be found on the <see cref="ResourceType" />.</exception>
    public string? GetGroupName() => this.groupName.GetLocalizableValue();

    /// <summary>
    /// Gets the value of <see cref="AutoGenerateField" /> if it has been set, or <see langword="null"/>.
    /// </summary>
    /// <returns>
    /// When <see cref="AutoGenerateField" /> has been set returns the value of that property.
    /// <para>When <see cref="AutoGenerateField" /> has not been set returns <see langword="null"/>.</para>
    /// </returns>
    public bool? GetAutoGenerateField() => this.autoGenerateField;

    /// <summary>
    /// Gets the value of <see cref="AutoGenerateFilter" /> if it has been set, or <see langword="null"/>.
    /// </summary>
    /// <returns>
    /// When <see cref="AutoGenerateFilter" /> has been set returns the value of that property.
    /// <para>When <see cref="AutoGenerateFilter" /> has not been set returns <see langword="null"/>.</para>
    /// </returns>
    public bool? GetAutoGenerateFilter() => this.autoGenerateFilter;

    /// <summary>
    /// Gets the value of <see cref="Order" /> if it has been set, or <see langword="null"/>.
    /// </summary>
    /// <returns>
    /// When <see cref="Order" /> has been set returns the value of that property.
    /// <para>When <see cref="Order" /> has not been set returns <see langword="null"/>.</para>
    /// </returns>
    /// <remarks>
    /// When an order is not specified, presentation layers should consider using the value of 10000.
    /// This value allows for explicitly-ordered fields to be displayed before and after the fields that don't specify an order.
    /// </remarks>
    public int? GetOrder() => this.order;

    private sealed class LocalizableString(string propertyName)
    {
        private Func<string?>? cachedResult;

        public string? Value
        {
            get;
            set
            {
                if (!string.Equals(field, value, StringComparison.Ordinal))
                {
                    this.ClearCache();
                    field = value;
                }
            }
        }

        [field: System.Diagnostics.CodeAnalysis.DynamicallyAccessedMembers(System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.PublicProperties)]
        [System.Diagnostics.CodeAnalysis.DynamicallyAccessedMembers(System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.PublicProperties)]
        public Type? ResourceType
        {
            get;
            set
            {
                if (field != value)
                {
                    this.ClearCache();
                    field = value;
                }
            }
        }

        public string? GetLocalizableValue()
        {
            if (this.cachedResult is null)
            {
                // If the property value is null, then just cache that value
                // If the resource type is null, then property value is literal, so cache it
                if (this.Value is null || this.ResourceType is null)
                {
                    this.cachedResult = () => this.Value;
                }
                else
                {
                    // Get the property from the resource type for this resource key
                    var property = this.ResourceType.GetRuntimeProperties().FirstOrDefault(property => string.Equals(property.Name, this.Value, StringComparison.Ordinal));

                    // We need to detect bad configurations so that we can throw exceptions accordingly
                    var badlyConfigured = false;

                    // Make sure we found the property and it's the correct type, and that the type itself is public
                    if (property is null || property.PropertyType != typeof(string))
                    {
                        badlyConfigured = true;
                    }
                    else
                    {
                        var getter = property.GetMethod;
                        if (getter?.IsStatic is not true)
                        {
                            badlyConfigured = true;
                        }
                    }

                    // If the property is not configured properly, then throw a missing member exception
                    if (badlyConfigured)
                    {
                        this.cachedResult = () => throw new InvalidOperationException($"Failed to localize {propertyName}");
                    }
                    else
                    {
                        // We have a valid property, so cache the resource
                        this.cachedResult = () => (string?)property!.GetValue(null, index: null);
                    }
                }
            }

            // Return the cached result
            return this.cachedResult();
        }

        private void ClearCache() => this.cachedResult = null;
    }
}