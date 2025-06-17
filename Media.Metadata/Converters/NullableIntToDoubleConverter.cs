// -----------------------------------------------------------------------
// <copyright file="NullableIntToDoubleConverter.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Media.Metadata.Converters;

/// <summary>
/// The converter from a <see cref="int?"/> to a <see cref="double"/>.
/// </summary>
internal sealed partial class NullableIntToDoubleConverter : Microsoft.UI.Xaml.Data.IValueConverter
{
    /// <inheritdoc/>
    public object? Convert(object value, Type targetType, object parameter, string language) => value switch
    {
        int intValue => intValue,
        _ => double.NaN,
    };

    /// <inheritdoc/>
    public object? ConvertBack(object value, Type targetType, object parameter, string language) => value switch
    {
        double doubleValue when !double.IsNaN(doubleValue) => (int)doubleValue,
        _ => default(int?),
    };
}