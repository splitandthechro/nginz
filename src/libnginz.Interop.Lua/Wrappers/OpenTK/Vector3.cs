using System;
using MoonSharp.Interpreter;

namespace nginz.Interop.Lua
{
	[MoonSharpUserData]
	public class Vector3
	{
		public float X;
		public float Y;
		public float Z;

		public Vector3 () {
			X = 0f;
			Y = 0f;
			Z = 0f;
		}

		public Vector3 (float x, float y, float z) {
			X = x;
			Y = y;
			Z = z;
		}

		public Vector3 @new (float x, float y, float z) {
			return new Vector3 (x, y, z);
		}

		public static implicit operator global::OpenTK.Vector3 (Vector3 vect) {
			return new OpenTK.Vector3 (vect.X, vect.Y, vect.Z);
		}
	}
}

