using System;
using MoonSharp.Interpreter;

namespace nginz.Interop.Lua
{
	[MoonSharpUserData]
	public class SpriteBatch
	{
		readonly global::nginz.SpriteBatch spriteBatch;

		public SpriteBatch (global::nginz.SpriteBatch batch) {
			spriteBatch = batch;
		}

		public void begin () {
			spriteBatch.Begin ();
		}

		public void draw (Texture2D tex, Vector2 vector, Color color) {
			spriteBatch.Draw (tex, vector, color);
		}

		// Needs to be flush because end is a reserved lua keyword
		public void flush () {
			spriteBatch.End ();
		}
	}
}

