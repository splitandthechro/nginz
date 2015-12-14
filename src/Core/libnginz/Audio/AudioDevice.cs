using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using OpenTK.Audio;
using OpenTK.Audio.OpenAL;

namespace nginz
{
	public class AudioDevice
	{
		//Maximum amount of concurrent sfx playbacks
		const int MAX_SOURCES = 30;

		internal AudioContext context;
		internal bool ready = false;
		bool createContext;
		bool running = true;
		//ConcurrentQueues to avoid threading errors
		ConcurrentQueue<StreamingAudio> toRemove = new ConcurrentQueue<StreamingAudio> ();
		ConcurrentQueue<StreamingAudio> toAdd = new ConcurrentQueue<StreamingAudio> ();
		ConcurrentQueue<Action> actions = new ConcurrentQueue<Action>();
		List<StreamingAudio> instances = new List<StreamingAudio> ();
		Queue<int> sources = new Queue<int>();
		List<int> playingSources = new List<int>();

		Thread updateThread;

		public AudioDevice(bool createContext = true)
		{
			this.createContext = createContext;
			updateThread = new Thread (new ThreadStart (UpdateThread));
			updateThread.Start ();
		}

		public void EnsureAudioThread(Action act)
		{
			bool done = false;
			actions.Enqueue (() => {
				act();
				done = true;
			});
			while (!done)
				Thread.Sleep (2);
		}
		void UpdateThread()
		{
			if(createContext)
				context = new AudioContext ();
			//Generate Sources
			int[] generatedSources = new int[MAX_SOURCES];
			AL.GenSources (generatedSources);
			foreach (int src in generatedSources)
				sources.Enqueue (src);
			generatedSources = null;
			//Let's get going
			ready = true;
			while (running) {
				//Actions
				Action action;
				if (actions.TryDequeue (out action))
					action ();
				//Check sfx
				for (int i = playingSources.Count - 1; i >= 0; i--) {
					var src = playingSources [i];
					if (AL.GetSourceState (src) != ALSourceState.Playing) {
						sources.Enqueue (src);
						playingSources.RemoveAt (i--);
					}
				}
				//remove from items to update
				while (toRemove.Count > 0) {
					StreamingAudio item;
					if (toRemove.TryDequeue (out item))
						instances.Remove (item);
				}
				//insert into items to update
				while (toAdd.Count > 0) {
					StreamingAudio item;
					if (toAdd.TryDequeue (out item))
						instances.Add(item);
				}
				//update
				for (int i = 0; i < instances.Count; i++) {
					instances [i].Update ();
				}
				Thread.Sleep (5);
				//CheckALError ();
			}
			//We have stopped now
			foreach(var src in playingSources) {
				AL.SourceStop(src);
				AL.DeleteSource(src);
			}
			foreach (var src in sources) {
				AL.DeleteSource(src);
			}
		}
		internal static void CheckALError()
		{
			ALError error;
			if ((error = AL.GetError()) != ALError.NoError)
				throw new InvalidOperationException(AL.GetErrorString(error));
		}
		internal void Add (StreamingAudio audio)
		{
			toAdd.Enqueue (audio);
		}
		internal void Remove(StreamingAudio audio)
		{
			toRemove.Enqueue (audio);
		}
		internal void PlayBuffer(int buf, float volume)
		{
			actions.Enqueue (() => {
				var src = sources.Dequeue();
				AL.BindBufferToSource(src, buf);
				AL.Source(src, ALSourcef.Gain, volume);
				AL.SourcePlay(src);
				playingSources.Add(src);
			});
		}
		public void Dispose()
		{
			running = false;
			updateThread.Join ();
		}
	}
}

