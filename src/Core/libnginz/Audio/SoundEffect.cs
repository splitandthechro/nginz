using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using OpenTK.Audio.OpenAL;
namespace nginz
{
	public class SoundEffect
	{
		int alBuffer;
		AudioDevice dev;

		public SoundEffect (AudioDevice device, string filename)
		{
			dev = device;
			List<byte> totalBytes = new List<byte> ();
			byte[] buffer = new byte[4096];
			var stream = File.OpenRead (filename);
			ALFormat fmt;
			int sampleRate;
			using (var decoder = DecoderFactory.GetDecoderFromStream (stream)) {
				int read = 0;
				fmt = decoder.Format;
				sampleRate = decoder.SampleRate;
				while ((read = decoder.Read (4096, buffer)) == 4096) {
					totalBytes.AddRange (buffer);
				}
				totalBytes.AddRange (buffer.Take (read));
			}
			dev.EnsureAudioThread(() => {
				alBuffer = AL.GenBuffer();
				AL.BufferData(
					alBuffer,
					fmt,
					totalBytes.ToArray(),
					totalBytes.Count,
					sampleRate
				);
			});
		}

		public SoundEffect (AudioDevice device, Sound sound) : this (device, sound.Filename) {
		}

		public void Play(float volume = 1f)
		{
			dev.PlayBuffer (alBuffer, volume);
		}
	}
}

