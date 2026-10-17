using System;
using System.IO;

namespace FFXIV_ACT_Plugin.Network;

/// <summary>
/// Just enough PE parsing to turn an RVA into a file offset, which is what the
/// Unscrambler constants are expressed in.
/// </summary>
internal sealed class PortableExecutable
{
	private readonly byte[] _image;

	public PortableExecutable(byte[] image)
	{
		_image = image ?? throw new ArgumentNullException(nameof(image));
		if (image.Length < 0x40 || ReadUInt16(0) != 0x5A4D)
		{
			throw new InvalidDataException("Not a PE image (missing MZ signature).");
		}
		int pe = ReadInt32(0x3C);
		if (pe <= 0 || pe + 24 > image.Length || ReadUInt32(pe) != 0x00004550)
		{
			throw new InvalidDataException("Not a PE image (missing PE signature).");
		}
		NumberOfSections = ReadUInt16(pe + 6);
		int optionalHeaderSize = ReadUInt16(pe + 20);
		SectionTableOffset = pe + 24 + optionalHeaderSize;
	}

	public int NumberOfSections { get; }

	private int SectionTableOffset { get; }

	/// <summary>
	/// Converts an RVA to an offset into the file's raw bytes.
	/// </summary>
	public int RvaToFileOffset(int rva)
	{
		for (int i = 0; i < NumberOfSections; i++)
		{
			int header = SectionTableOffset + i * 40;
			if (header + 40 > _image.Length)
			{
				break;
			}
			uint virtualSize = ReadUInt32(header + 8);
			uint virtualAddress = ReadUInt32(header + 12);
			uint rawSize = ReadUInt32(header + 16);
			uint rawPointer = ReadUInt32(header + 20);
			uint span = Math.Max(virtualSize, rawSize);
			if (rva >= virtualAddress && rva < virtualAddress + span)
			{
				int offset = (int)(rva - virtualAddress + rawPointer);
				if (offset < 0 || offset > _image.Length)
				{
					break;
				}
				return offset;
			}
		}
		throw new InvalidDataException($"RVA 0x{rva:X} does not belong to any section.");
	}

	public byte[] ReadBytes(int rva, int length)
	{
		int offset = RvaToFileOffset(rva);
		if (length < 0 || offset + length > _image.Length)
		{
			throw new InvalidDataException($"Cannot read {length} bytes at RVA 0x{rva:X}.");
		}
		byte[] result = new byte[length];
		Buffer.BlockCopy(_image, offset, result, 0, length);
		return result;
	}

	private ushort ReadUInt16(int offset) => (ushort)(_image[offset] | (_image[offset + 1] << 8));

	private uint ReadUInt32(int offset)
	{
		return (uint)(_image[offset] | (_image[offset + 1] << 8) | (_image[offset + 2] << 16) | (_image[offset + 3] << 24));
	}

	private int ReadInt32(int offset) => (int)ReadUInt32(offset);
}
