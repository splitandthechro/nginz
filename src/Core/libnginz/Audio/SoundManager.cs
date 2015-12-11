using System;
using System.IO;
using OpenTK.Audio;
using OpenTK.Audio.OpenAL;

namespace nginz
{
	/// <summary>
	/// Sound manager.
	/// </summary>
	public class SoundManager : IDisposable
	{

		/// <summary>
		/// The device.
		/// </summary>
		readonly AudioDevice Device;

		/// <summary>
		/// The music streamer.
		/// </summary>
		StreamingAudio MusicStreamer;

		/// <summary>
		/// The music decoder.
		/// </summary>
		IDecoder MusicDecoder;

		/// <summary>
		/// The file stream.
		/// </summary>
		FileStream Stream;

		/// <summary>
		/// Whether the music should be looped.
		/// </summary>
		bool loopMusic = false;

		/// <summary>
		/// The music volume.
		/// </summary>
		float musicVolume = 1f;

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="nginz.SoundManager"/> should loop music.
		/// </summary>
		/// <value><c>true</c> if music should be looped; otherwise, <c>false</c>.</value>
		public bool LoopMusic {
			get {
				return loopMusic;
			}
			set {
				loopMusic = value;
			}
		}

		/// <summary>
		/// Gets or sets the music volume.
		/// </summary>
		/// <value>The music volume.</value>
		public float MusicVolume {
			get {
				return musicVolume;
			}
			set {
				musicVolume = value;
				if (MusicStreamer != null)
					MusicStreamer.Volume = MusicVolume;
			}
		}

		/// <summary>
		/// Gets the state of the music.
		/// </summary>
		/// <value>The state of the music.</value>
		public AudioPlayState MusicState {
			get {
				var state = MusicStreamer == null
					? AudioPlayState.Stopped
					: MusicStreamer.GetState ();
				return state;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="nginz.SoundManager"/> class.
		/// </summary>
		public SoundManager () {
			Device = new AudioDevice ();
		}

		public void PlayMusic (Sound sound) {
			PlayMusic (sound.Filename);
		}

		public void PlayMusic (string filename) {
			StopMusic ();
			Stream = File.OpenRead (filename);
			MusicDecoder = DecoderFactory.GetDecoderFromStream (Stream);
			MusicStreamer = new StreamingAudio (Device, MusicDecoder.Format, MusicDecoder.SampleRate);
			MusicStreamer.BufferNeeded += (instance, buffer) => MusicDecoder.Read (buffer.Length, buffer);
			MusicStreamer.PlaybackFinished += (sender, e) => {
				if (loopMusic)
					PlayMusic (filename);
				else {
					MusicStreamer.Dispose ();
					MusicDecoder.Dispose ();
					MusicStreamer = null;
				}
			};
			MusicStreamer.Play ();
		}

		public void StopMusic () {
			if (MusicState == AudioPlayState.Playing) {
				MusicStreamer.Stop ();
				MusicStreamer.Dispose ();
				MusicDecoder.Dispose ();
				MusicStreamer = null;
				if (Stream != null) {
					try {
						Stream.Dispose ();
					// Analysis disable once EmptyGeneralCatchClause
					} catch { }
				}
			}
		}

		public void Dispose () {
			StopMusic ();
			Device.Dispose ();
		}
	}
}

