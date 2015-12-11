using System;
using System.IO;
namespace nginz
{
	static class DecoderFactory
	{
		const uint RIFF_MAGIC = 0x46464952;
		const uint OGG_MAGIC = 0x5367674f;
		public static IDecoder GetDecoderFromStream(Stream stream)
		{
			var reader = new BinaryReader (stream);
			var magic = reader.ReadUInt32 ();
			if (magic == RIFF_MAGIC) {
				return new WaveDecoder (reader);
			} else if (magic == OGG_MAGIC) {
				return new NVorbisDecoder (stream);
			}
			throw new NotSupportedException ("Unsupported file format");
		}
	}
}

