// -----------------------------------------------------------------------
// <copyright file="DateTimeToDateTimeOffsetConverter.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Media.Metadata.Converters;

/// <summary>
/// <see cref="Microsoft.UI.Xaml.Data.IValueConverter"/> to convert <see cref="DateTime"/> to <see cref="DateTimeOffset"/> instances.
/// </summary>
internal sealed partial class DateTimeToDateTimeOffsetConverter : Microsoft.UI.Xaml.Data.IValueConverter
{
    /// <inheritdoc/>
    public object? Convert(object value, Type targetType, object parameter, string language) => value switch
    {
        DateTime dateTime => new DateTimeOffset(dateTime),
        DateTimeOffset dateTimeOffset => dateTimeOffset,
        _ => default,
    };

    /// <inheritdoc/>
    public object? ConvertBack(object value, Type targetType, object parameter, string language) => value switch
    {
        DateTime dateTime => dateTime,
        DateTimeOffset dateTimeOffset => dateTimeOffset.DateTime,
        _ => default,
    };
}