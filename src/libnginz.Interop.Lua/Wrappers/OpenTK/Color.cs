using System;
using MoonSharp.Interpreter;
using OpenTK.Graphics;

namespace nginz.Interop.Lua
{
	[MoonSharpUserData]
	public class Color
	{
		public float R { get { return BaseColor.R; } }
		public float G { get { return BaseColor.G; } }
		public float B { get { return BaseColor.B; } }
		public float A { get { return BaseColor.A; } }

		Color4 BaseColor;

		public Color () {
			BaseColor = Color4.Black;
		}

		public Color (float r, float g, float b, float a = 1f) {
			BaseColor = new Color4 (r, g, b, a);
		}

		public Color @new (float r, float g, float b, float a) {
			return new Color (r, g, b, a);
		}

		public static implicit operator Color4 (Color color) {
			return color.BaseColor;
		}
	}
}

