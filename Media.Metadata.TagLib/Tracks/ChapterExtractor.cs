// -----------------------------------------------------------------------
// <copyright file="ChapterExtractor.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Media.Metadata.Tracks;

/// <summary>
/// The Chapter extractor.
/// </summary>
/// <remarks>
/// Initialises a new instance of the <see cref="ChapterExtractor"/> class.
/// </remarks>
/// <param name="stream">The abstract stream.</param>
internal sealed class ChapterExtractor(Stream stream)
{
    private static readonly byte[] Tref = "tref"u8.ToArray();

    private static readonly byte[] Chap = "chap"u8.ToArray();

    private static readonly byte[] Mdat = "mdat"u8.ToArray();

    private static readonly byte[] Moov = "moov"u8.ToArray();

    private static readonly byte[] Mvhd = "mvhd"u8.ToArray();

    private static readonly byte[] Trak = "trak"u8.ToArray();

    private static readonly byte[] Tkhd = "tkhd"u8.ToArray();

    private static readonly byte[] Mdia = "mdia"u8.ToArray();

    private static readonly byte[] Mdhd = "mdhd"u8.ToArray();

    private static readonly byte[] Hdlr = "hdlr"u8.ToArray();

    private static readonly byte[] Minf = "minf"u8.ToArray();

    private static readonly byte[] Stbl = "stbl"u8.ToArray();

    private static readonly byte[] Stco = "stco"u8.ToArray();

    private static readonly byte[] Stts = "stts"u8.ToArray();

    private static readonly byte[] Ftyp = "ftyp"u8.ToArray();

    private readonly Stream stream = stream;

    /// <summary>
    /// Gets the chapters.
    /// </summary>
    public ChapterInfo[]? Chapters { get; private set; }

    /// <summary>
    /// Gets the tracks.
    /// </summary>
    public TrakInfo[]? Tracks { get; private set; }

    /// <summary>
    /// Runs the chapter extractor.
    /// </summary>
    public void Run()
    {
        this.Chapters = null;
        _ = this.stream.Seek(0L, SeekOrigin.Begin);
        if (ReadMoovInfo() is { } moovInfo)
        {
            this.Tracks = moovInfo.Tracks;
            ReadChapters(moovInfo);
        }

        MoovInfo? ReadMoovInfo()
        {
            if (FindBox(Moov) is { } moovBox)
            {
                var moovData = default(MoovInfo);
                var tracks = new List<TrakInfo>();
                var maximumLength = moovBox.BoxOffset + moovBox.Offset;
                foreach (var box in this.EnumerateBoxes(maximumLength))
                {
                    if (box.Type == Mvhd)
                    {
                        moovData = ReadMvhd(moovData);
                    }

                    if (box.Type == Trak)
                    {
                        tracks.Add(ReadTrak(box));
                    }
                }

                return moovData with { Tracks = [.. tracks] };

                MoovInfo ReadMvhd(MoovInfo moovData)
                {
                    var v = new byte[1];
                    _ = this.stream.Read(v, 0, 1);
                    var isv8 = v[0] is 1;
                    _ = this.stream.Seek(3L + (isv8 ? 16L : 8L), SeekOrigin.Current);
                    return moovData with { TimeUnitPerSecond = this.ReadUInt32() };
                }

                TrakInfo ReadTrak(BoxInfo trakBox)
                {
                    var maximumTrackLength = trakBox.BoxOffset + trakBox.Offset;
                    var trakData = default(TrakInfo);
                    foreach (var box in this.EnumerateBoxes(maximumTrackLength))
                    {
                        if (box.Type == Tkhd)
                        {
                            trakData = trakData with { Id = ReadTkhd() };
                        }

                        if (box.Type == Mdia)
                        {
                            trakData = ReadMdia(trakData, box);
                        }

                        if (box.Type == Tref)
                        {
                            trakData = ReadTref(trakData, box);
                        }
                    }

                    return trakData;

                    uint ReadTkhd()
                    {
                        var v = new byte[1];
                        _ = this.stream.Read(v, 0, 1);
                        var isv8 = v[0] is 1;
                        _ = this.stream.Seek(3L + (isv8 ? 16L : 8L), SeekOrigin.Current);
                        return this.ReadUInt32();
                    }

                    TrakInfo ReadMdia(TrakInfo trackData, BoxInfo mdiaBox)
                    {
                        foreach (var box in this.EnumerateBoxes(mdiaBox.BoxOffset + mdiaBox.Offset))
                        {
                            if (box.Type == Mdhd)
                            {
                                trackData = ReadMdhd(trackData);
                            }

                            if (box.Type == Hdlr)
                            {
                                trackData = ReadHdlr(trackData);
                            }

                            if (box.Type == Minf)
                            {
                                trackData = ReadMinf(trackData, box);
                            }
                        }

                        return trackData;

                        TrakInfo ReadMdhd(TrakInfo trakData)
                        {
                            var v = new byte[1];
                            _ = this.stream.Read(v, 0, 1);
                            var isv8 = v[0] is 1;

                            _ = this.stream.Seek(3L + (isv8 ? 16L : 8L), SeekOrigin.Current);
                            var timeUnitPerSecond = this.ReadUInt32();

                            // get the duration
                            var duration = isv8
                                ? this.ReadUInt64()
                                : this.ReadUInt32();

                            var data = this.ReadUInt16();
                            char[] code =
                            [
                                (char)(((data & 0x7c00) >> 10) + 0x60),
                                (char)(((data & 0x03e0) >> 5) + 0x60),
                                (char)((data & 0x001f) + 0x60),
                            ];

                            var language = new string(code);

                            return trakData with { TimeUnitPerSecond = timeUnitPerSecond, Duration = duration, Language = language };
                        }

                        TrakInfo ReadHdlr(TrakInfo trakData)
                        {
                            _ = this.stream.Seek(8, SeekOrigin.Current);
                            var b = new byte[4];
                            _ = this.stream.Read(b, 0, 4);
                            var bc = new char[4];
                            _ = System.Text.Encoding.ASCII.GetDecoder().GetChars(b, 0, 4, bc, 0);
                            return trakData with { Type = new string(bc) };
                        }

                        TrakInfo ReadMinf(TrakInfo trakData, BoxInfo box2)
                        {
                            foreach (var box in this
                                .EnumerateBoxes(box2.BoxOffset + box2.Offset)
                                .Where(box => box.Type == Stbl))
                            {
                                trakData = ReadStbl(trakData, box);
                            }

                            return trakData;

                            TrakInfo ReadStbl(TrakInfo trakData, BoxInfo box2)
                            {
                                foreach (var boxType in this
                                    .EnumerateBoxes(box2.BoxOffset + box2.Offset)
                                    .Select(box => box.Type))
                                {
                                    if (boxType == Stco)
                                    {
                                        trakData = ReadStco(trakData);
                                    }

                                    if (boxType == Stts)
                                    {
                                        trakData = ReadStts(trakData);
                                    }
                                }

                                return trakData;

                                TrakInfo ReadStco(TrakInfo trakData)
                                {
                                    _ = this.stream.Seek(4L, SeekOrigin.Current);
                                    var len = this.ReadUInt32();
                                    if (len > 1024)
                                    {
                                        len = 0;
                                    }

                                    var samples = new long[len];
                                    for (uint i = 0; i < len; i++)
                                    {
                                        samples[i] = this.ReadUInt32();
                                    }

                                    return trakData with { Samples = samples };
                                }

                                TrakInfo ReadStts(TrakInfo trakData)
                                {
                                    _ = this.stream.Seek(4L, SeekOrigin.Current);
                                    var len = this.ReadUInt32();
                                    if (len > 1024)
                                    {
                                        len = 0;
                                    }

                                    var frameCount = new uint[len];
                                    var durations = new uint[len];
                                    for (uint i = 0; i < len; i++)
                                    {
                                        frameCount[i] = this.ReadUInt32();
                                        durations[i] = this.ReadUInt32();
                                    }

                                    return trakData with { FrameCount = frameCount, Durations = durations };
                                }
                            }
                        }
                    }

                    TrakInfo ReadTref(TrakInfo trakData, BoxInfo box2)
                    {
                        foreach (var box in this
                            .EnumerateBoxes(box2.BoxOffset + box2.Offset)
                            .Where(box => box.Type == Chap))
                        {
                            trakData = ReadChap(trakData, box);
                        }

                        return trakData;

                        TrakInfo ReadChap(TrakInfo trakData, BoxInfo box2)
                        {
                            var length = (box2.Offset - 8) / 4;
                            if (length is > 0 and < 1024)
                            {
                                var chaps = new uint[length];
                                for (var i = 0; i < length; i++)
                                {
                                    chaps[i] = this.ReadUInt32();
                                }

                                return trakData with { Chaps = chaps };
                            }

                            return trakData;
                        }
                    }
                }
            }

            return null;

            BoxInfo? FindBox(byte[] type)
            {
                return FirstOrDefaultNullable(this.EnumerateBoxes().Where(box => box.Type == type));

                static T? FirstOrDefaultNullable<T>(IEnumerable<T> source)
                    where T : struct
                {
                    var enumerator = source.GetEnumerator();
                    return enumerator.MoveNext()
                        ? enumerator.Current
                        : default(T?);
                }
            }
        }

        void ReadChapters(MoovInfo moovBox)
        {
            var soundBox = moovBox.Tracks.Where(b => string.Equals(b.Type, "soun", StringComparison.Ordinal)).ToArray();
            if (soundBox.Length is 0)
            {
                return;
            }

            if (soundBox[0].Chaps?.Count > 0)
            {
                var chapterIds = new HashSet<uint>(soundBox[0].Chaps);
                var textBox = moovBox.Tracks.Where(b => b.Id.HasValue && string.Equals(b.Type, "text", StringComparison.Ordinal) && chapterIds.Contains(b.Id.Value)).ToArray();
                if (textBox.Length is 0)
                {
                    return;
                }

                ReadChaptersText(textBox[0]);

                void ReadChaptersText(TrakInfo textBox)
                {
                    if (textBox.Durations is { } durations)
                    {
                        var length = Math.Min(durations.Count, textBox.Samples?.Count ?? int.MinValue);
                        if (length > 0)
                        {
                            this.Chapters = new ChapterInfo[length];
                            var position = TimeSpan.Zero;
                            var timeUnitPerSecond = textBox.TimeUnitPerSecond ?? 1D;
                            if (timeUnitPerSecond <= 0.1)
                            {
                                timeUnitPerSecond = 600;
                            }

                            for (var i = 0; i < length; i++)
                            {
                                var chapterInfo = new ChapterInfo { Time = position };
                                var duration = (double)textBox.Durations[i];
                                position += TimeSpan.FromSeconds(duration / timeUnitPerSecond);
                                if (textBox.Samples is { } samples)
                                {
                                    _ = this.stream.Seek(samples[i], SeekOrigin.Begin);
                                    chapterInfo = chapterInfo with { Name = ReadPascalString(System.Text.Encoding.UTF8) };
                                }

                                this.Chapters[i] = chapterInfo;
                            }

                            string ReadPascalString(System.Text.Encoding encoding)
                            {
                                var @ushort = this.ReadUInt16();
                                if (@ushort is 0)
                                {
                                    return string.Empty;
                                }

                                var b = new byte[@ushort];
                                _ = this.stream.Read(b, 0, @ushort);
                                return new string(encoding.GetChars(b));
                            }
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Gets a value indicating whether this stream is an MP4A file.
    /// </summary>
    /// <returns><see langword="true" /> if this instance is an MP4A file; otherwise <see langword="false" />.</returns>
    public bool IsMp4a()
    {
        if (this.stream.Length < 8)
        {
            return false;
        }

        _ = this.stream.Seek(4L, SeekOrigin.Begin);
        return this.ReadType() == Ftyp;
    }

    private IEnumerable<BoxInfo> EnumerateBoxes(long? maximumLength = null)
    {
        bool last;
        do
        {
            last = true;
            if (NextBox(maximumLength) is { } box)
            {
                yield return box;

                SeekNext(box);

                last = box.Last;
            }
        }
        while (!last);

        BoxInfo? NextBox(long? maxLen = null)
        {
            var currentPosition = this.stream.Position;
            var length = maxLen ?? this.stream.Length;
            if (length - this.stream.Position < 8)
            {
                return null;
            }

            long offset = this.ReadUInt32();
            var type = this.ReadType();
            if (type != Mdat)
            {
                return new BoxInfo
                {
                    BoxOffset = currentPosition,
                    Offset = offset,
                    Last = offset is 0,
                    Type = type,
                };
            }

            if (length - this.stream.Position < 8)
            {
                return null;
            }

            if (offset is 1)
            {
                offset = (long)this.ReadUInt64();
            }
            else
            {
                _ = this.stream.Seek(8L, SeekOrigin.Current);
            }

            return new BoxInfo
            {
                BoxOffset = currentPosition,
                Offset = offset,
                Last = offset is 0,
                Type = type,
            };
        }

        void SeekNext(BoxInfo box)
        {
            _ = this.stream.Seek(box.BoxOffset, SeekOrigin.Begin);
            _ = this.stream.Seek(box.Offset, SeekOrigin.Current);
        }
    }

    private AsciiType ReadType()
    {
        var b = new byte[4];
        _ = this.stream.Read(b, 0, 4);
        return new AsciiType { Type = b };
    }

    private ushort ReadUInt16()
    {
        var bytes = new byte[2];
        return this.stream.Read(bytes, 0, 2) is 2 ? System.Buffers.Binary.BinaryPrimitives.ReadUInt16BigEndian(bytes) : default;
    }

    private uint ReadUInt32()
    {
        var bytes = new byte[4];
        return this.stream.Read(bytes, 0, 4) is 4 ? System.Buffers.Binary.BinaryPrimitives.ReadUInt32BigEndian(bytes) : default;
    }

    private ulong ReadUInt64()
    {
        var bytes = new byte[8];
        return this.stream.Read(bytes, 0, 8) is 8 ? System.Buffers.Binary.BinaryPrimitives.ReadUInt64BigEndian(bytes) : default;
    }
}