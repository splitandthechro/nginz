using System;
using MoonSharp.Interpreter;

namespace nginz.Interop.Lua
{
	[MoonSharpUserData]
	public class Vector2
	{
		public float X;
		public float Y;

		public Vector2 () {
			X = 0f;
			Y = 0f;
		}

		public Vector2 (float x, float y) {
			X = x;
			Y = y;
		}

		public Vector2 @new (float x, float y) {
			return new Vector2 (x, y);
		}

		public static implicit operator global::OpenTK.Vector2 (Vector2 vect) {
			return new OpenTK.Vector2 (vect.X, vect.Y);
		}
	}
}

