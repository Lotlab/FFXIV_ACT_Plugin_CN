using System;

// .NET Framework's BitConverter has no Span overloads - those only exist from
// .NET Core 2.1 / netstandard2.1 onwards.  The Unscrambler library derives its
// keys with `BitConverter.ToUInt32(span)`, so this forwards those calls.
//
// GlobalUsings.cs aliases BitConverter to this type, which means every
// BitConverter call in this assembly goes through here.
namespace FFXIV_ACT_Plugin.Unscrambler.Compat
{
    public static class BitConverter
    {
        public static ushort ToUInt16(ReadOnlySpan<byte> value)
        {
            if (value.Length < sizeof(ushort))
                throw new ArgumentOutOfRangeException(nameof(value));
            return (ushort)(value[0] | (value[1] << 8));
        }

        public static uint ToUInt32(ReadOnlySpan<byte> value)
        {
            if (value.Length < sizeof(uint))
                throw new ArgumentOutOfRangeException(nameof(value));
            return (uint)(value[0] | (value[1] << 8) | (value[2] << 16) | (value[3] << 24));
        }

        public static int ToInt32(ReadOnlySpan<byte> value) => (int)ToUInt32(value);

        public static short ToInt16(ReadOnlySpan<byte> value) => (short)ToUInt16(value);

        public static ushort ToUInt16(byte[] value, int startIndex) => System.BitConverter.ToUInt16(value, startIndex);

        public static short ToInt16(byte[] value, int startIndex) => System.BitConverter.ToInt16(value, startIndex);

        public static uint ToUInt32(byte[] value, int startIndex) => System.BitConverter.ToUInt32(value, startIndex);

        public static int ToInt32(byte[] value, int startIndex) => System.BitConverter.ToInt32(value, startIndex);

        public static byte[] GetBytes(int value) => System.BitConverter.GetBytes(value);

        public static byte[] GetBytes(uint value) => System.BitConverter.GetBytes(value);

        public static byte[] GetBytes(ushort value) => System.BitConverter.GetBytes(value);

        public static byte[] GetBytes(short value) => System.BitConverter.GetBytes(value);
    }
}
