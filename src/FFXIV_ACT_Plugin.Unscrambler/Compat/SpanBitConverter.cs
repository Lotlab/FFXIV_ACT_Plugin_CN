using System;

// .NET Framework's BitConverter has no overloads that take a span.  The
// Unscrambler sources use those to read little-endian values out of packet and
// table buffers, so this type supplies just that surface.  GlobalUsings.cs
// aliases the name BitConverter to this class for the whole assembly.
namespace FFXIV_ACT_Plugin.Unscrambler.Compat
{
    public static class SpanBitConverter
    {
        public static ushort ToUInt16(ReadOnlySpan<byte> value)
        {
            if (value.Length < sizeof(ushort))
                throw new ArgumentOutOfRangeException(nameof(value), "A UInt16 needs two bytes.");
            return (ushort)(value[0] | (value[1] << 8));
        }

        public static short ToInt16(ReadOnlySpan<byte> value)
        {
            return (short)ToUInt16(value);
        }

        public static uint ToUInt32(ReadOnlySpan<byte> value)
        {
            if (value.Length < sizeof(uint))
                throw new ArgumentOutOfRangeException(nameof(value), "A UInt32 needs four bytes.");
            return (uint)(value[0] | (value[1] << 8) | (value[2] << 16) | (value[3] << 24));
        }

        public static int ToInt32(ReadOnlySpan<byte> value)
        {
            return (int)ToUInt32(value);
        }

        // The byte[] overloads below are the ones .NET Framework already has;
        // they are repeated here only because the alias hides the BCL type.

        public static ushort ToUInt16(byte[] value, int startIndex)
        {
            return System.BitConverter.ToUInt16(value, startIndex);
        }

        public static short ToInt16(byte[] value, int startIndex)
        {
            return System.BitConverter.ToInt16(value, startIndex);
        }

        public static uint ToUInt32(byte[] value, int startIndex)
        {
            return System.BitConverter.ToUInt32(value, startIndex);
        }

        public static int ToInt32(byte[] value, int startIndex)
        {
            return System.BitConverter.ToInt32(value, startIndex);
        }

        public static byte[] GetBytes(ushort value) => System.BitConverter.GetBytes(value);

        public static byte[] GetBytes(short value) => System.BitConverter.GetBytes(value);

        public static byte[] GetBytes(uint value) => System.BitConverter.GetBytes(value);

        public static byte[] GetBytes(int value) => System.BitConverter.GetBytes(value);
    }
}
