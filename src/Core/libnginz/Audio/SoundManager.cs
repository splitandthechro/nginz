using System;
using System.IO;
using OpenTK.Audio;
using OpenTK.Audio.OpenAL;

namespace nginz
{
	public class SoundManager : IDisposable
	{
		AudioDevice device;
		StreamingAudio musicStreamer;
		IDecoder musicDecoder;
		float musicVolume = 1f;
		bool loopMusic = false;

		public bool LoopMusic {
			get {
				return loopMusic;
			} set {
				loopMusic = value;
			}
		}

		public float MusicVolume {
			get {
				return musicVolume;
			} set {
				musicVolume = value;
				if (musicStreamer != null)
					musicStreamer.Volume = MusicVolume;
			}
		}

		public SoundManager () 
		{
			device = new AudioDevice ();
		}

		public void PlayMusic(string filename)
		{
			StopMusic ();
			var stream = File.OpenRead (filename);
			musicDecoder = DecoderFactory.GetDecoderFromStream (stream);
			musicStreamer = new StreamingAudio (device, musicDecoder.Format, musicDecoder.SampleRate);
			musicStreamer.BufferNeeded += (instance, buffer) => musicDecoder.Read(buffer.Length, buffer);
			musicStreamer.PlaybackFinished += (sender, e) => {
				if(loopMusic)
					PlayMusic(filename);
				else {
					musicStreamer.Dispose();
					musicDecoder.Dispose();
					musicStreamer = null;
				}
			};
			musicStreamer.Play ();
		}

		public void StopMusic()
		{
			if(MusicState == AudioPlayState.Playing) {
				musicStreamer.Stop ();
				musicStreamer.Dispose();
				musicDecoder.Dispose();
				musicStreamer = null;
			}
		}

		public AudioPlayState MusicState {
			get {
				return musicStreamer == null ? AudioPlayState.Stopped : musicStreamer.GetState ();
			}
		}

		public void Dispose()
		{
			StopMusic ();
			device.Dispose ();
		}
	}
}

