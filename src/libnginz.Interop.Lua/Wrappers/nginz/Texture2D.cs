using System;
using MoonSharp.Interpreter;

namespace nginz.Interop.Lua
{
	[MoonSharpUserData]
	public class Texture2D
	{
		global::nginz.Texture2D Texture;

		public Texture2D () {
		}

		public Texture2D (global::nginz.Texture2D texture) {
			Texture = texture;
		}

		public int width () {
			return Texture.Width;
		}

		public int height () {
			return Texture.Height;
		}

		public static implicit operator global::nginz.Texture2D (Texture2D tex) {
			return tex.Texture;
		}
	}
}

