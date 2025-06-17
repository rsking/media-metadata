// -----------------------------------------------------------------------
// <copyright file="ImageToImageSourceConverter.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Media.Metadata.Converters;

using Microsoft.UI.Xaml.Media.Imaging;
using SixLabors.ImageSharp;
using Windows.Graphics.Imaging;

/// <summary>
/// The converter from a <see cref="System.Drawing.Image"/> to a <see cref="ImageSource"/>.
/// </summary>
public sealed partial class ImageToImageSourceConverter : Microsoft.UI.Xaml.Data.IValueConverter
{
    /// <inheritdoc/>
    public object? Convert(object? value, Type targetType, object parameter, string language)
    {
        return value switch
        {
            ImageSource imageSource => imageSource,
            Image image => ImageToImageSource(image),
            _ => default,
        };

        static object ImageToImageSource(Image image)
        {
            using var stream = new MemoryStream();
            image.SaveAsBmp(stream);

            var decoder = BitmapDecoder.CreateAsync(stream.AsRandomAccessStream()).AsTask().Result;

            var source = new SoftwareBitmapSource();
            source.SetBitmapAsync(decoder.GetSoftwareBitmapAsync().AsTask().Result).AsTask().Wait();
            return source;
        }
    }

    /// <inheritdoc/>
    public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotSupportedException();
}