using System;
using System.Collections.Generic;
using System.Linq;
using nginz;

namespace FlameThrowah
{
	public class WrappedTexture {
		public readonly string texname;
		public readonly Texture2D tex;
		public readonly int texindex;
		public WrappedTexture (string name, Texture2D tex, int index) {
			texname = name;
			texindex = index;
			this.tex = tex;
		}
	}

	public class TextureLookup
	{
		public List<WrappedTexture> Textures;

		public int this [Texture2D tex] {
			get { 
				var texture = Textures.FirstOrDefault (t => t.tex == tex);
				return texture != default (WrappedTexture) ? texture.texindex : -1;
			}
		}

		public int this [string tex] {
			get { 
				var texture = Textures.FirstOrDefault (t => t.texname == tex);
				return texture != default (WrappedTexture) ? texture.texindex : -1;
			}
		}

		int index;

		public TextureLookup () {
			Textures = new List<WrappedTexture> ();
			index = 0;
		}

		public void Add (string name, Texture2D tex) {
			if (!Textures.Any () || Textures.All (t => t.texname != name && t.tex != tex))
				Textures.Add (new WrappedTexture (name, tex, index++));
		}
	}
}

