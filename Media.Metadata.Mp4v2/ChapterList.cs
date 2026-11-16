// -----------------------------------------------------------------------
// <copyright file="ChapterList.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Media.Metadata;

/// <summary>
/// Represents the collection of chapters in a file, tracking whether there have
/// been changes made to the collection.
/// </summary>
internal sealed class ChapterList : IList<Chapter>
{
    private readonly IList<Chapter> chapters = [];
    private readonly ICollection<Guid> hashedIndex = [];

    private ChapterList(IEnumerable<Chapter> chapters)
    {
        foreach (var chapter in chapters)
        {
            this.AddInternal(chapter);
        }
    }

    private ChapterList()
    {
    }

    /// <summary>
    /// Gets the number of <see cref="Chapter">Chapters</see> contained in this <see cref="ChapterList"/>.
    /// </summary>
    public int Count => this.chapters.Count;

    /// <summary>
    /// Gets a value indicating whether this <see cref="ChapterList"/> is read-only.
    /// </summary>
    bool ICollection<Chapter>.IsReadOnly => false;

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="ChapterList"/> has been modified since being loaded.
    /// </summary>
    private bool IsDirty { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="Chapter"/> at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the <see cref="Chapter"/> to get or set.</param>
    /// <returns>The <see cref="Chapter"/> at the specified index.</returns>
    /// <exception cref="ArgumentNullException">value to be set is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">value to be set is already in the list.</exception>
    public Chapter this[int index]
    {
        get
        {
            return this.chapters[index];
        }

        set
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (this.hashedIndex.Contains(value.Id))
            {
                throw new ArgumentException("Chapter is already in the chapter list", nameof(value));
            }

            this.chapters[index] = value;
            value.PropertyChanged += this.OnChapterChanged;
            this.hashedIndex.Add(value.Id);
            this.IsDirty = true;
        }
    }

    /// <summary>
    /// Adds a <see cref="Chapter"/> to this <see cref="ChapterList"/>.
    /// </summary>
    /// <param name="item">The <see cref="Chapter"/> to add.</param>
    /// <exception cref="ArgumentNullException"><paramref name="item"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="item"/> is already in the list.</exception>
    public void Add(Chapter item)
    {
        if (item is null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        if (this.hashedIndex.Contains(item.Id))
        {
            throw new ArgumentException("Chapter is already in the chapter list", nameof(item));
        }

        this.chapters.Add(item);
        item.PropertyChanged += this.OnChapterChanged;
        this.hashedIndex.Add(item.Id);
        this.IsDirty = true;
    }

    /// <summary>
    /// Removes all items from this <see cref="ChapterList"/>.
    /// </summary>
    public void Clear()
    {
        if (this.Count > 0)
        {
            this.hashedIndex.Clear();
            this.chapters.Clear();
            this.IsDirty = true;
        }
    }

    /// <summary>
    /// Determines whether this <see cref="ChapterList"/> contains a specific <see cref="Chapter"/>.
    /// </summary>
    /// <param name="item">The <see cref="Chapter"/> to locate in the <see cref="ChapterList"/>.</param>
    /// <returns><see langword="true"/> if <paramref name="item"/> is found in the <see cref="ChapterList"/>; otherwise, <see langword="false"/>.</returns>
    public bool Contains(Chapter item) => this.hashedIndex.Contains(item.Id);

    /// <summary>
    /// Copies the <see cref="Chapter">Chapters</see> of this <see cref="ChapterList"/> to an <see cref="Array"/>, starting at a particular <see cref="Array"/> index.
    /// </summary>
    /// <param name="array">The one-dimensional <see cref="Array"/> that is the destination of the <see cref="Chapter">Chapters</see> copied from this <see cref="ChapterList"/>. The <see cref="Array"/> must have zero-based indexing.</param>
    /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
    /// <exception cref="ArgumentNullException"><paramref name="array"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="arrayIndex"/> is less than 0.</exception>
    /// <exception cref="ArgumentException">The number of elements in this <see cref="ChapterList"/> is greater than the available space from <paramref name="arrayIndex"/> to the end of the destination <paramref name="array"/>.</exception>
    public void CopyTo(Chapter[] array, int arrayIndex) => this.chapters.CopyTo(array, arrayIndex);

    /// <summary>
    /// Determines the index of a <see cref="Chapter"/> in this <see cref="ChapterList"/>.
    /// </summary>
    /// <param name="item">The <see cref="Chapter"/> to locate in the <see cref="ChapterList"/>.</param>
    /// <returns>The index of <paramref name="item"/> if found in the list; otherwise, -1.</returns>
    public int IndexOf(Chapter item) => this.hashedIndex.Contains(item.Id) ? this.chapters.IndexOf(item) : -1;

    /// <summary>
    /// Inserts an item to this <see cref="ChapterList"/> at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param>
    /// <param name="item">The <see cref="Chapter"/> to insert into the <see cref="ChapterList"/>.</param>
    /// <exception cref="ArgumentNullException"><paramref name="item"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than 0, or <paramref name="index"/> is greater than <see cref="Count"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="item"/> is already in the list.</exception>
    public void Insert(int index, Chapter item)
    {
        if (item is null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        if (this.hashedIndex.Contains(item.Id))
        {
            throw new ArgumentException("Chapter is already in the chapter list", nameof(item));
        }

        this.chapters.Insert(index, item);
        item.PropertyChanged += this.OnChapterChanged;
        this.hashedIndex.Add(item.Id);
        this.IsDirty = true;
    }

    /// <summary>
    /// Removes the first occurrence of a specific <see cref="Chapter"/> from the <see cref="ChapterList"/>.
    /// </summary>
    /// <param name="item">The <see cref="Chapter"/> to remove from the <see cref="ChapterList"/>.</param>
    /// <returns><see langword="true"/> if <paramref name="item"/> was successfully removed from the
    /// <see cref="ChapterList"/>; otherwise, <see langword="false"/> false. This method also returns
    /// <see langword="false"/> if <paramref name="item"/> is not found in the original<see cref="ChapterList"/>.</returns>
    public bool Remove(Chapter item)
    {
        var isRemoved = this.chapters.Remove(item);
        this.IsDirty = this.IsDirty || isRemoved;
        if (isRemoved)
        {
            item.PropertyChanged -= this.OnChapterChanged;
            _ = this.hashedIndex.Remove(item.Id);
        }

        return isRemoved;
    }

    /// <summary>
    /// Removes the <see cref="Chapter"/> at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the <see cref="Chapter"/> to remove.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than 0, or <paramref name="index"/> is greater than <see cref="Count"/>.</exception>
    public void RemoveAt(int index)
    {
        var toRemove = this[index];
        _ = this.hashedIndex.Remove(toRemove.Id);
        toRemove.PropertyChanged -= this.OnChapterChanged;
        this.chapters.RemoveAt(index);
        this.IsDirty = true;
    }

    /// <summary>
    /// Returns an enumerator that iterates through a <see cref="ChapterList"/>.
    /// </summary>
    /// <returns>An <see cref="IEnumerator{T}"/> object that can be used to iterate through the <see cref="ChapterList"/>.</returns>
    public IEnumerator<Chapter> GetEnumerator() => this.chapters.GetEnumerator();

    /// <summary>
    /// Returns an enumerator that iterates through a <see cref="ChapterList"/>.
    /// </summary>
    /// <returns>An <see cref="System.Collections.IEnumerator"/> object that can be used to iterate through the <see cref="ChapterList"/>.</returns>
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => this.chapters.GetEnumerator();

    /// <summary>
    /// Reads the chapter information from the chapters.
    /// </summary>
    /// <param name="chapters">The chapters.</param>
    /// <returns>A new instance of a <see cref="ChapterList"/> object containing the information about the chapters for the file.</returns>
    internal static ChapterList Create(IEnumerable<MediaChapter> chapters) => new(chapters.Select(c => new Chapter
    {
        Title = c.Title,
        Duration = c.Duration,
    }));

    /// <summary>
    /// Reads the chapter information from the specified file.
    /// </summary>
    /// <param name="fileHandle">The handle to the file from which to read the chapter information.</param>
    /// <returns>A new instance of a <see cref="ChapterList"/> object containing the information
    /// about the chapters for the file.</returns>
    internal static ChapterList ReadFromFile(IntPtr fileHandle)
    {
        var list = new ChapterList();
        var chapterListPointer = IntPtr.Zero;
        var chapterCount = 0;
        var chapterType = NativeMethods.MP4GetChapters(fileHandle, ref chapterListPointer, ref chapterCount, NativeMethods.MP4ChapterType.Qt);
        if (chapterType is not NativeMethods.MP4ChapterType.None && chapterCount is not 0)
        {
            var currentChapterPointer = chapterListPointer;
            for (var i = 0; i < chapterCount; i++)
            {
                var currentChapter = currentChapterPointer.ToStructure<NativeMethods.MP4Chapter>();
                var duration = TimeSpan.FromMilliseconds(currentChapter.duration);
                var title = System.Text.Encoding.UTF8.GetString(currentChapter.title);
                if (currentChapter.title is [0xFE, 0xFF, ..] or [0xFF, 0xFE, ..])
                {
                    title = System.Text.Encoding.Unicode.GetString(currentChapter.title);
                }

                title = title[..title.IndexOf('\0')];
                list.AddInternal(new Chapter() { Duration = duration, Title = title });
                currentChapterPointer = IntPtr.Add(currentChapterPointer, Marshal.SizeOf(currentChapter));
            }
        }
        else
        {
            var timeScale = NativeMethods.MP4GetTimeScale(fileHandle);
            var duration = NativeMethods.MP4GetDuration(fileHandle);
            list.AddInternal(new Chapter() { Duration = TimeSpan.FromSeconds((double)duration / timeScale), Title = "Chapter 1" });
        }

        if (chapterListPointer != IntPtr.Zero)
        {
            NativeMethods.MP4Free(chapterListPointer);
        }

        return list;
    }

    /// <summary>
    /// Writes the chapter information to the file.
    /// </summary>
    /// <param name="fileHandle">The handle to the file to which to write the chapter information.</param>
    internal void WriteToFile(IntPtr fileHandle)
    {
        // Only write to the file if there have been changes since the chapters were read.
        // Note that a happy side effect of this is that if there were no chapters specified
        // in the file at read time, and no manipulation was done before write, we will not
        // write chapters into the file, even though our internal representation will contain
        // a single chapter with the full duration of the file, and the title of "Chapter 1".
        if (this.IsDirty)
        {
            // Find the first video track, so that we make sure the total duration
            // of the chapters we add does not exceed the length of the file.
            var referenceTrackId = -1;
            for (short i = 0; i < NativeMethods.MP4GetNumberOfTracks(fileHandle, type: null, 0); i++)
            {
                var currentTrackId = NativeMethods.MP4FindTrackId(fileHandle, i, type: null, 0);
                var trackType = NativeMethods.MP4GetTrackType(fileHandle, currentTrackId);
                if (string.Equals(trackType, NativeMethods.MP4VideoTrackType, StringComparison.Ordinal))
                {
                    referenceTrackId = currentTrackId;
                    break;
                }
            }

            // If we don't have a video track, then we have an audio file, which has
            // only one track, and we can use it to find the duration.
            referenceTrackId = referenceTrackId <= 0 ? 1 : referenceTrackId;
            var referenceTrackDuration = NativeMethods.MP4ConvertFromTrackDuration(fileHandle, referenceTrackId, NativeMethods.MP4GetTrackDuration(fileHandle, referenceTrackId), NativeMethods.MP4TimeScale.Milliseconds);

            long runningTotal = 0;
            var nativeChapters = new List<NativeMethods.MP4Chapter>();
            foreach (var chapter in this.chapters)
            {
                var nativeChapter = new NativeMethods.MP4Chapter
                {
                    // Set the title
                    title = new byte[1024],
                };
                var titleByteArray = System.Text.Encoding.UTF8.GetBytes(chapter.Title);
                Array.Copy(titleByteArray, nativeChapter.title, titleByteArray.Length);

                // Set the duration, making sure that we only use durations up to
                // the length of the reference track.
                var chapterLength = (long)chapter.Duration.TotalMilliseconds;
                nativeChapter.duration = runningTotal + chapterLength > referenceTrackDuration
                    ? referenceTrackDuration - runningTotal
                    : chapterLength;

                runningTotal += chapterLength;
                nativeChapters.Add(nativeChapter);
                if (runningTotal > referenceTrackDuration)
                {
                    break;
                }
            }

            var chapterArray = nativeChapters.ToArray();
            _ = NativeMethods.MP4SetChapters(fileHandle, chapterArray, chapterArray.Length, NativeMethods.MP4ChapterType.Qt);
        }
    }

    /// <summary>
    /// Adds a <see cref="Chapter"/> to the list without dirtying the list.
    /// </summary>
    /// <param name="toAdd">The <see cref="Chapter"/> to add to the list.</param>
    private void AddInternal(Chapter toAdd)
    {
        this.chapters.Add(toAdd);
        toAdd.PropertyChanged += this.OnChapterChanged;
        this.hashedIndex.Add(toAdd.Id);
    }

    private void OnChapterChanged(object sender, EventArgs e) => this.IsDirty = true;
}