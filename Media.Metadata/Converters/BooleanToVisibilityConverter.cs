// -----------------------------------------------------------------------
// <copyright file="BooleanToVisibilityConverter.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Media.Metadata.Converters;

/// <summary>
/// The <see cref="bool"/> to <see cref="Microsoft.UI.Xaml.Visibility"/> <see cref="Microsoft.UI.Xaml.Data.IValueConverter"/>.
/// </summary>
internal sealed partial class BooleanToVisibilityConverter : Microsoft.UI.Xaml.Data.IValueConverter
{
    /// <inheritdoc/>
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        return value switch
        {
            bool boolValue => BoolToVisibility(boolValue),
            _ => Microsoft.UI.Xaml.Visibility.Visible,
        };

        static Microsoft.UI.Xaml.Visibility BoolToVisibility(bool value)
        {
            return value switch
            {
                true => Microsoft.UI.Xaml.Visibility.Visible,
                false => Microsoft.UI.Xaml.Visibility.Collapsed,
            };
        }
    }

    /// <inheritdoc/>
    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        return value switch
        {
            Microsoft.UI.Xaml.Visibility visibility => VisibilityToBool(visibility),
            _ => false,
        };

        static bool VisibilityToBool(Microsoft.UI.Xaml.Visibility value)
        {
            return value switch
            {
                Microsoft.UI.Xaml.Visibility.Visible => true,
                _ => false,
            };
        }
    }
}