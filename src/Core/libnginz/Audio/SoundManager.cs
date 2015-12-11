using System;
using OpenTK.Audio;
using OpenTK.Audio.OpenAL;

namespace nginz
{
	public class SoundManager
	{
		readonly AudioContext Context;

		public SoundManager () {
			Context = new AudioContext ();
			Context.MakeCurrent ();
		}
	}
}

