using System;
using OpenTK.Audio.OpenAL;
namespace nginz
{
	interface IDecoder : IDisposable
	{
		TimeSpan Duration { get; }
		ALFormat Format { get; }
		int SampleRate { get; }
		int Read (int length, byte[] buffer);
		void Reset();
	}
}

