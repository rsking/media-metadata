// -----------------------------------------------------------------------
// <copyright file="IntPtrExtensions.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Media.Metadata;

/// <summary>
/// <see cref="IntPtr"/> extensions.
/// </summary>
internal static class IntPtrExtensions
{
    /// <summary>
    /// Reads a structure beginning at the location pointed to in memory by the specified pointer value.
    /// </summary>
    /// <typeparam name="T">The type of the structure to read.</typeparam>
    /// <param name="value">The <see cref="IntPtr"/> value indicating the location
    /// in memory at which to begin reading data.</param>
    /// <returns>An instance of the specified structure type.</returns>
    /// <exception cref="ArgumentNullException">Thrown when this <see cref="IntPtr"/> is a null pointer (<see cref="IntPtr.Zero"/>).</exception>
    public static T ToStructure<T>(this IntPtr value) => value == IntPtr.Zero
        ? throw new ArgumentNullException(nameof(value), $"Structures cannot be read from a null pointer ({nameof(IntPtr)}.{nameof(IntPtr.Zero)})")
        : Marshal.PtrToStructure<T>(value);

    /// <summary>
    /// Reads a block of memory beginning at the location pointed to by the specified pointer value, and copies the contents into a byte array of the specified length.
    /// </summary>
    /// <param name="value">The <see cref="IntPtr"/> value indicating the location in memory at which to begin reading data.</param>
    /// <param name="bufferLength">The number of bytes to read into the byte array.</param>
    /// <returns>
    /// The byte array containing copies of the values pointed to by this <see cref="IntPtr"/>.
    /// Returns <see langword="null"/> if this pointer is a null pointer (<see cref="IntPtr.Zero"/>).
    /// </returns>
    public static byte[]? ToByteArray(this IntPtr value, int bufferLength)
    {
        if (value == IntPtr.Zero)
        {
            return default;
        }

        var buffer = new byte[bufferLength];
        Marshal.Copy(value, buffer, 0, bufferLength);
        return buffer;
    }

    /// <summary>
    /// Reads a 16-bit integer value beginning at the location pointed to in memory by the specified pointer value.
    /// </summary>
    /// <param name="value">The <see cref="IntPtr"/> value indicating the location in memory at which to begin reading data.</param>
    /// <returns>
    /// The 16-bit integer value pointed to by this <see cref="IntPtr"/>.
    /// Returns <see langword="null"/> if this pointer is a null pointer (<see cref="IntPtr.Zero"/>).
    /// </returns>
    public static short? ReadInt16(this IntPtr value) => value == IntPtr.Zero ? null : System.Runtime.InteropServices.Marshal.ReadInt16(value);

    /// <summary>
    /// Reads a 32-bit integer value beginning at the location pointed to in memory by the specified pointer value.
    /// </summary>
    /// <param name="value">The <see cref="IntPtr"/> value indicating the location in memory at which to begin reading data.</param>
    /// <returns>
    /// The 32-bit integer value pointed to by this <see cref="IntPtr"/>.
    /// Returns <see langword="null"/> if this pointer is a null pointer (<see cref="IntPtr.Zero"/>).
    /// </returns>
    public static int? ReadInt32(this IntPtr value) => value == IntPtr.Zero ? null : System.Runtime.InteropServices.Marshal.ReadInt32(value);

    /// <summary>
    /// Reads a 64-bit integer value beginning at the location pointed to in memory by the specified pointer value.
    /// </summary>
    /// <param name="value">The <see cref="IntPtr"/> value indicating the location in memory at which to begin reading data.</param>
    /// <returns>
    /// The 64-bit integer value pointed to by this <see cref="IntPtr"/>.
    /// Returns <see langword="null"/> if this pointer is a null pointer (<see cref="IntPtr.Zero"/>).
    /// </returns>
    public static long? ReadInt64(this IntPtr value) => value == IntPtr.Zero ? null : System.Runtime.InteropServices.Marshal.ReadInt64(value);

    /// <summary>
    /// Reads an 8-bit integer value beginning at the location pointed to in memory by the specified pointer value, and coerces that value into a boolean.
    /// </summary>
    /// <param name="value">The <see cref="IntPtr"/> value indicating the location in memory at which to begin reading data.</param>
    /// <returns>
    /// <see langword="true"/> if the value pointed to by this <see cref="IntPtr"/> is non-zero; <see langword="false"/> if the value pointed to is zero.
    /// Returns <see langword="null"/> if this pointer is a null pointer (<see cref="IntPtr.Zero"/>).
    /// </returns>
    public static bool? ReadBoolean(this IntPtr value) => value == IntPtr.Zero ? null : System.Runtime.InteropServices.Marshal.ReadByte(value) is not 0;

    /// <summary>
    /// Reads an 8-bit integer value beginning at the location pointed to in memory by the specified pointer value.
    /// </summary>
    /// <param name="value">The <see cref="IntPtr"/> value indicating the location in memory at which to begin reading data.</param>
    /// <returns>
    /// The 8-bit integer value pointed to by this <see cref="IntPtr"/>.
    /// Returns <see langword="null"/> if this pointer is a null pointer (<see cref="IntPtr.Zero"/>).
    /// </returns>
    public static byte? ReadByte(this IntPtr value) => value == IntPtr.Zero ? null : System.Runtime.InteropServices.Marshal.ReadByte(value);

    /// <summary>
    /// Reads an enumerated value beginning at the location pointed to in memory by the specified pointer value.
    /// </summary>
    /// <typeparam name="T">A value derived from <see cref="Enum"/>.</typeparam>
    /// <param name="value">The <see cref="IntPtr"/> value indicating the location in memory at which to begin reading data.</param>
    /// <param name="defaultValue">The default value of the enumerated value to return if the memory location pointed to by this <see cref="IntPtr"/> is a null pointer (<see cref="IntPtr.Zero"/>).</param>
    /// <returns>
    /// The enumerated value pointed to by this <see cref="IntPtr"/>.
    /// Returns the specified default value if this pointer is a null pointer (<see cref="IntPtr.Zero"/>).
    /// </returns>
    /// <exception cref="ArgumentException"><typeparamref name="T"/> must be an enum.</exception>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0046:Convert to conditional expression", Justification = "This would make the logic harder to read.")]
    public static T ReadEnumValue<T>(this IntPtr value, T defaultValue)
        where T : Enum
    {
        if (value == IntPtr.Zero)
        {
            return defaultValue;
        }

        var underlyingType = Enum.GetUnderlyingType(typeof(T));
        if (underlyingType == typeof(byte))
        {
            return GetEnum(Marshal.ReadByte(value));
        }

        if (underlyingType == typeof(long))
        {
            return GetEnum(Marshal.ReadInt64(value));
        }

        if (underlyingType == typeof(short))
        {
            return GetEnum(Marshal.ReadInt16(value));
        }

        return GetEnum(Marshal.ReadInt32(value));

        static T GetEnum(object rawValue)
        {
            return (T)Enum.ToObject(typeof(T), rawValue);
        }
    }

    /// <summary>
    /// Writes a 16-bit integer value to the block of memory pointed to by the specified and calls the specified API call with a pointer to the block of memory.
    /// </summary>
    /// <param name="tagsStructure">The <see cref="IntPtr"/> value pointing indicating the location of the MP4Tags structure.</param>
    /// <param name="value">The 16-bit integer value with which to call the MP4V2 API.</param>
    /// <param name="mp4ApiFunction">The MP4V2 API method to call with the pointer to the value.</param>
    public static void WriteInt16(this IntPtr tagsStructure, short? value, Func<IntPtr, IntPtr, bool> mp4ApiFunction)
    {
        if (value is { } stringValue)
        {
            var valuePtr = Marshal.AllocHGlobal(sizeof(short));
            Marshal.WriteInt16(valuePtr, stringValue);
            _ = mp4ApiFunction(tagsStructure, valuePtr);
            Marshal.FreeHGlobal(valuePtr);
        }
        else
        {
            _ = mp4ApiFunction(tagsStructure, IntPtr.Zero);
        }
    }

    /// <summary>
    /// Writes a 32-bit integer value to the block of memory pointed to by the specified and calls the specified API call with a pointer to the block of memory.
    /// </summary>
    /// <param name="tagsStructure">The <see cref="IntPtr"/> value pointing indicating the location of the MP4Tags structure.</param>
    /// <param name="value">The 32-bit integer value with which to call the MP4V2 API.</param>
    /// <param name="mp4ApiFunction">The MP4V2 API method to call with the pointer to the 32-bit integer value.</param>
    public static void WriteInt32(this IntPtr tagsStructure, int? value, Func<IntPtr, IntPtr, bool> mp4ApiFunction)
    {
        if (value is { } intValue)
        {
            var valuePtr = Marshal.AllocHGlobal(sizeof(int));
            Marshal.WriteInt32(valuePtr, intValue);
            _ = mp4ApiFunction(tagsStructure, valuePtr);
            Marshal.FreeHGlobal(valuePtr);
        }
        else
        {
            _ = mp4ApiFunction(tagsStructure, IntPtr.Zero);
        }
    }

    /// <summary>
    /// Writes a 64-bit integer value to the block of memory pointed to by the specified and calls the specified API call with a pointer to the block of memory.
    /// </summary>
    /// <param name="tagsStructure">The <see cref="IntPtr"/> value pointing indicating the location of the MP4Tags structure.</param>
    /// <param name="value">The 64-bit integer value with which to call the MP4V2 API.</param>
    /// <param name="mp4ApiFunction">The MP4V2 API method to call with the pointer to the 64-bit integer value.</param>
    public static void WriteInt64(this IntPtr tagsStructure, long? value, Func<IntPtr, IntPtr, bool> mp4ApiFunction)
    {
        if (value is { } longValue)
        {
            var valuePtr = Marshal.AllocHGlobal(sizeof(long));
            Marshal.WriteInt64(valuePtr, longValue);
            _ = mp4ApiFunction(tagsStructure, valuePtr);
            Marshal.FreeHGlobal(valuePtr);
        }
        else
        {
            _ = mp4ApiFunction(tagsStructure, IntPtr.Zero);
        }
    }

    /// <summary>
    /// Writes an 8-bit integer value to the block of memory pointed to by the specified and calls the specified API call with a pointer to the block of memory.
    /// </summary>
    /// <param name="tagsStructure">The <see cref="IntPtr"/> value pointing indicating the location of the MP4Tags structure.</param>
    /// <param name="value">The 8-bit integer value with which to call the MP4V2 API.</param>
    /// <param name="mp4ApiFunction">The MP4V2 API method to call with the pointer to the 8-bit integer value.</param>
    public static void WriteByte(this IntPtr tagsStructure, byte? value, Func<IntPtr, IntPtr, bool> mp4ApiFunction)
    {
        if (value is { } byteValue)
        {
            var valuePtr = Marshal.AllocHGlobal(sizeof(byte));
            Marshal.WriteByte(valuePtr, byteValue);
            _ = mp4ApiFunction(tagsStructure, valuePtr);
            Marshal.FreeHGlobal(valuePtr);
        }
        else
        {
            _ = mp4ApiFunction(tagsStructure, IntPtr.Zero);
        }
    }

    /// <summary>
    /// Writes an boolean value as an 8-bit integer to the block of memory pointed to by the specified and calls the specified API call with a pointer to the block of memory.
    /// </summary>
    /// <param name="tagsStructure">The <see cref="IntPtr"/> value pointing indicating the location of the MP4Tags structure.</param>
    /// <param name="value">The 8-bit integer value with which to call the MP4V2 API.</param>
    /// <param name="mp4ApiFunction">The MP4V2 API method to call with the pointer to the 8-bit integer value.</param>
    public static void WriteBoolean(this IntPtr tagsStructure, bool? value, Func<IntPtr, IntPtr, bool> mp4ApiFunction)
    {
        if (value is { } boolValue)
        {
            var valuePtr = Marshal.AllocHGlobal(sizeof(byte));
            Marshal.WriteByte(valuePtr, GetByte(boolValue));
            _ = mp4ApiFunction(tagsStructure, valuePtr);
            Marshal.FreeHGlobal(valuePtr);

            static byte GetByte(bool value)
            {
                return value ? (byte)1 : (byte)0;
            }
        }
        else
        {
            _ = mp4ApiFunction(tagsStructure, IntPtr.Zero);
        }
    }
}