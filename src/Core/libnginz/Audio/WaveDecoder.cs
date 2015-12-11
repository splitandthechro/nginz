using System;
using System.IO;
using System.Text;
using OpenTK.Audio.OpenAL;

namespace nginz
{
	class WaveDecoder : IDecoder
	{
		//RIFF magic numbers
		const ushort CODEC_PCM = 0x1;
		//members
		int sampleRate;
		ALFormat format;
		int dataSize;
		long dataStart;
		BinaryReader reader;
		TimeSpan duration;
		public WaveDecoder (BinaryReader reader)
		{
			this.reader = reader;
			byte[] buffer = new byte[4];
			//further checks
			int riffChunkSize = reader.ReadInt32 ();
			reader.Read (buffer, 0, 4);
			string riffFormat = Encoding.ASCII.GetString (buffer);
			if (riffFormat != "WAVE") {
				throw new NotSupportedException ("RIFF file is not wave");
			}
			//get codec information
			reader.Read (buffer, 0, 4);
			string fmtChunk = Encoding.ASCII.GetString (buffer);
			if (fmtChunk != "fmt ") {
				throw new NotSupportedException ("This wave file is not supported");
			}
			int fmtSize = reader.ReadInt32 ();
			ushort codecId = reader.ReadUInt16 ();
			switch (codecId) {
			case CODEC_PCM:
				break;
			default:
				throw new NotSupportedException ("Unsupported wave codec");
			}
			ushort channels = reader.ReadUInt16 ();
			sampleRate = reader.ReadInt32 ();
			int byteRate = reader.ReadInt32 ();
			ushort blockAlign = reader.ReadUInt16 ();
			ushort bits = reader.ReadUInt16 ();
			if (channels == 2) {
				if (bits == 8) {
					format = ALFormat.Stereo8;
				} else if (bits == 16) {
					format = ALFormat.Stereo16;
				} else {
					throw new NotSupportedException ("Unsupported bits " + bits);
				}
			} else if (channels == 1) {
				if (bits == 8) {
					format = ALFormat.Mono8;
				} else if (bits == 16) {
					format = ALFormat.Mono16;
				} else {
					throw new NotSupportedException ("Unsupported bits " + bits);
				}
			} else {
				throw new NotSupportedException ("Unsupported channels " + channels);
			}
			//get data block
			while (true) {
				//read the chunk specifier
				reader.Read (buffer, 0, 4);
				string dataChunk = Encoding.ASCII.GetString (buffer);
				if (dataChunk != "data") {
					//not the data chunk. skip
					var size = reader.ReadInt32 ();
					reader.BaseStream.Seek (size, SeekOrigin.Current);
				} else {
					//found the data chunk!
					break;
				}
			}
			dataSize = reader.ReadInt32 ();
			dataStart = reader.BaseStream.Position;
			duration = TimeSpan.FromSeconds ((double)dataSize / ((double)sampleRate * (double)channels * (double)bits / 8.0));
		}
		public TimeSpan Duration {
			get {
				return duration;
			}
		}
		public ALFormat Format {
			get {
				return format;
			}
		}
		public int SampleRate {
			get {
				return sampleRate;
			}
		}
		public void Reset()
		{
			reader.BaseStream.Seek (dataStart, SeekOrigin.Begin);
		}
		public int Read(int length, byte[] buffer)
		{
			var position = reader.BaseStream.Position;
			if (length > (dataStart + dataSize) - position) {
				length = (int)((dataStart + dataSize) - position);
			}
			reader.Read (buffer, 0, length);
			return length;
		}
		public void Dispose()
		{
			reader.Dispose();
		}
	}
}

