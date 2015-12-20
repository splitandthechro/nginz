using System;
using OpenTK;

namespace nginz
{
	struct RGBA
	{
		public byte R;
		public byte G;
		public byte B;
		public byte A;

		public RGBA (byte r, byte g, byte b, byte a) {
			R = r;
			G = g;
			B = b;
			A = a;
		}

		public RGBA (float r, float g, float b, float a) {
			R = (byte) MathHelper.Clamp (r * 255f, 0, 255);
			G = (byte) MathHelper.Clamp (g * 255f, 0, 255);
			B = (byte) MathHelper.Clamp (b * 255f, 0, 255);
			A = (byte) MathHelper.Clamp (a * 255f, 0, 255);
		}
	}
}

