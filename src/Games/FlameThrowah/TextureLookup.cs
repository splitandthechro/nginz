using System;
using System.Collections.Generic;
using nginz;

namespace FlameThrowah
{
	[Serializable]
	public class TextureLookup
	{
		public List<Texture2D> Textures;

		public int this [Texture2D tex] {
			get { return Textures.IndexOf (tex); }
		}

		public TextureLookup () {
			Textures = new List<Texture2D> ();
		}

		public void Add (Texture2D tex) {
			Textures.Add (tex);
		}
	}
}

