using System;
using System.Linq;
using System.Collections.Concurrent;
using OpenTK.Audio;
using OpenTK.Audio.OpenAL;
namespace nginz
{
	class StreamingAudio
	{
		public delegate int BufferNeededHandler (StreamingAudio instance, byte[] buffer);
		public delegate void PlaybackFinishedHandler (object sender, bool finished);
		public event BufferNeededHandler BufferNeeded;
		public event PlaybackFinishedHandler PlaybackFinished;

		readonly int sourceId;
		readonly int[] bufferIds;

		int sampleRate;
		byte[] buffer = new byte[4096];
		float volume = 1f;

		ALFormat bufferFormat;
		AudioPlayState currentState = AudioPlayState.Stopped;
		AudioDevice device;

		public float Volume {
			get {
				return volume;
			} set {
				if (value != volume) {
					volume = value;
					AL.Source (sourceId, ALSourcef.Gain, volume);
				}
			}
		}
		internal StreamingAudio (AudioDevice device, ALFormat format, int sampleRate)
		{
			bufferFormat = format;
			while (!device.ready)
				;
			uint sid;
			AL.GenSource (out sid);
			sourceId = (int)sid;
			AudioDevice.CheckALError ();
			bufferIds = AL.GenBuffers (4);
			AudioDevice.CheckALError ();
			this.device = device;
			this.sampleRate = sampleRate;
		}
		bool finished = false;
		bool threadRunning = false;
		bool userStopped = false;
		internal void Update()
		{
			//manage state
			if (currentState == AudioPlayState.Stopped) {
				AL.SourceStop (sourceId);
				device.Remove (this);
				threadRunning = false;
				if (!userStopped) {
					if (PlaybackFinished != null)
						PlaybackFinished (this, true);
				}
				userStopped = false;
				return;
			}
			var state = AL.GetSourceState (sourceId);
			AudioDevice.CheckALError ();
			if (currentState == AudioPlayState.Paused) {
				if (state != ALSourceState.Paused)
					AL.SourcePause (sourceId);
				return;
			}

			//load buffers
			int processed_count;
			AL.GetSource (sourceId, ALGetSourcei.BuffersProcessed, out processed_count);
			while (processed_count > 0) {
				int bid = AL.SourceUnqueueBuffer (sourceId);
				if (bid != 0 && !finished) {
					int length = BufferNeeded (this, buffer);
					finished = length <= 0;
					if (!finished) {
						AL.BufferData (bid, bufferFormat, buffer, length, sampleRate);
						AL.SourceQueueBuffer (sourceId, bid);
					}
				}
				--processed_count;
			}
			//check buffer
			if (state == ALSourceState.Stopped && !finished)
				AL.SourcePlay (sourceId);
			//are we finished?
			if (finished && state == ALSourceState.Stopped) {
				device.Remove (this);
				currentState = AudioPlayState.Stopped;
				threadRunning = false;
				Empty ();
				if(PlaybackFinished != null)
					PlaybackFinished (this, false);
			}
		}

		public void Play ()
		{
			if (currentState == AudioPlayState.Playing)
				return;
			if (currentState == AudioPlayState.Stopped) {
				finished = false;
				currentState = AudioPlayState.Playing;
				for (int i = 0; i < bufferIds.Length; i++) {
					int length = BufferNeeded (this, buffer);
					AL.BufferData (bufferIds [i], bufferFormat, buffer, length, sampleRate);
					AudioDevice.CheckALError ();
					AL.SourceQueueBuffer (sourceId, bufferIds [i]);
					AudioDevice.CheckALError ();
					AL.SourcePlay (sourceId);
					AudioDevice.CheckALError ();
				}
				device.Add (this);
				threadRunning = true;
			}
			currentState = AudioPlayState.Playing;

		}

		public void Pause ()
		{
			currentState = AudioPlayState.Paused;
		}

		public void Stop ()
		{
			if (currentState == AudioPlayState.Stopped)
				return;
			userStopped = true;
			currentState = AudioPlayState.Stopped;
			while (threadRunning)
				;
			AL.SourceStop (sourceId);
			device.Remove (this);
			Empty ();
		}

		void Empty()
		{
			int queued;
			AL.GetSource(sourceId, ALGetSourcei.BuffersQueued, out queued);
			if (queued > 0)
			{
				try
				{
					AL.SourceUnqueueBuffers(sourceId, queued);
					AudioDevice.CheckALError ();
				}
				catch (InvalidOperationException)
				{
					//work around OpenAL bug
					int processed;
					AL.GetSource(sourceId, ALGetSourcei.BuffersProcessed, out processed);
					var salvaged = new int[processed];
					if (processed > 0)
					{
						AL.SourceUnqueueBuffers(sourceId, processed, salvaged);
						AudioDevice.CheckALError ();
					}
					AL.SourceStop(sourceId);
					AudioDevice.CheckALError ();
					Empty();
				}
			}
		}
		public AudioPlayState GetState ()
		{
			return currentState;
		}

		public void Dispose ()
		{
			Stop ();
			AL.DeleteBuffers (bufferIds);
			AL.DeleteSource (sourceId);
		}
	}
}