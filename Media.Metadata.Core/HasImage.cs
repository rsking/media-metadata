// -----------------------------------------------------------------------
// <copyright file="HasImage.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Media.Metadata;

/// <summary>
/// A resource that has an image.
/// </summary>
public abstract record class HasImage : IHasImage, IAsyncDisposable, IDisposable
{
    private bool imageRetrieved;

    private SixLabors.ImageSharp.Formats.IImageFormat? imageFormat;

    private Image? image;

    private bool disposedValue;

    /// <inheritdoc/>
    public Image? Image
    {
        get
        {
            this.EnsureImage();
            return this.image;
        }

        init
        {
            this.image = value;
            this.imageRetrieved = true;
        }
    }

    /// <inheritdoc/>
    public SixLabors.ImageSharp.Formats.IImageFormat? ImageFormat
    {
        get
        {
            this.EnsureImage();
            return this.imageFormat;
        }

        init
        {
            this.imageFormat = value;
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        // Perform async cleanup.
        await this.DisposeAsyncCore().ConfigureAwait(false);

        // Dispose of unmanaged resources.
        this.Dispose(disposing: false);

        // Suppress finalization.
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Gets the image.
    /// </summary>
    /// <returns>The image.</returns>
    protected virtual ValueTask<(Image Image, SixLabors.ImageSharp.Formats.IImageFormat ImageFormat)> GetImageAsync() => default;

    /// <summary>
    /// Disposes this instance.
    /// </summary>
    /// <param name="disposing">Set to <see langword="true"/> to dispose managed resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposedValue)
        {
            if (disposing)
            {
                if (this.imageRetrieved && this.image is { } disposable)
                {
                    disposable.Dispose();
                }

                this.image = default;
                this.imageRetrieved = false;
            }

            this.disposedValue = true;
        }
    }

    /// <summary>
    /// Disposes this instance asynchronously.
    /// </summary>
    /// <returns>The value task.</returns>
    protected virtual async ValueTask DisposeAsyncCore()
    {
        if (this.imageRetrieved)
        {
            if (this.image is IAsyncDisposable asyncDisposable)
            {
                await asyncDisposable.DisposeAsync().ConfigureAwait(false);
            }
            else if (this.image is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }

        this.image = default;
        this.imageRetrieved = false;
    }

    private void EnsureImage()
    {
        if (!this.imageRetrieved)
        {
            (this.image, this.imageFormat) = GetImage(this.GetImageAsync());
            this.imageRetrieved = true;

            static T GetImage<T>(ValueTask<T> task)
            {
                if (task.IsCompletedSuccessfully)
                {
                    return task.Result;
                }

                if (task.IsFaulted)
                {
                    // this should throw the exception.
                    return task.GetAwaiter().GetResult();
                }

                return task.AsTask().Result;
            }
        }
    }
}