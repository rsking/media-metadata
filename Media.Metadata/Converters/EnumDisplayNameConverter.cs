// -----------------------------------------------------------------------
// <copyright file="EnumDisplayNameConverter.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Media.Metadata.Converters;

using System.Reflection;

/// <summary>
/// A value converter that takes enum value and returns name of the enum.
/// </summary>
public partial class EnumDisplayNameConverter : Microsoft.UI.Xaml.Data.IValueConverter
{
    /// <inheritdoc/>
    public object? Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is { } v)
        {
            return FromFieldInfo(v) ?? Enum.GetName(value.GetType(), v);

            static string? FromFieldInfo(object v)
            {
                return v.ToString() is { } valueString && v.GetType().GetField(valueString) is { } fieldInfo
                    ? FromMyDisplayAttribute(fieldInfo) ?? FromDisplayAttribute(fieldInfo) ?? FromDisplayNameAttribute(fieldInfo)
                    : default;

                static string? FromDisplayNameAttribute(FieldInfo fieldInfo)
                {
                    return fieldInfo.GetCustomAttribute<System.ComponentModel.DisplayNameAttribute>() switch
                    {
                        System.ComponentModel.DisplayNameAttribute attribute => attribute.DisplayName,
                        _ => default,
                    };
                }

                static string? FromDisplayAttribute(FieldInfo fieldInfo)
                {
                    return fieldInfo.GetCustomAttribute<System.ComponentModel.DataAnnotations.DisplayAttribute>() switch
                    {
                        System.ComponentModel.DataAnnotations.DisplayAttribute attribute when !string.IsNullOrEmpty(attribute.Name) => attribute.Name,
                        System.ComponentModel.DataAnnotations.DisplayAttribute attribute when !string.IsNullOrEmpty(attribute.ShortName) => attribute.ShortName,
                        _ => default,
                    };
                }

                static string? FromMyDisplayAttribute(FieldInfo fieldInfo)
                {
                    if (fieldInfo.GetCustomAttribute<DisplayAttribute>() is DisplayAttribute attribute)
                    {
                        var name = attribute.GetName();
                        if (!string.IsNullOrEmpty(name))
                        {
                            return name;
                        }

                        name = attribute.GetShortName();
                        if (!string.IsNullOrEmpty(name))
                        {
                            return name;
                        }
                    }

                    return default;
                }
            }
        }

        return string.Empty;
    }

    /// <inheritdoc/>
    public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotSupportedException();
}