using System;
using MoonSharp.Interpreter;
using OpenTK.Graphics.OpenGL4;
using gl = OpenTK.Graphics.OpenGL4.GL;

namespace nginz.Interop.Lua
{
	[MoonSharpUserData]
	public class GL
	{
		public void clearColor (float r, float g, float b) {
			gl.ClearColor (r, g, b, 1f);
		}

		public void clearColor (float r, float g, float b, float a) {
			gl.ClearColor (r, g, b, a);
		}

		public void clear (string bit) {
			ClearBufferMask mask;
			if (Enum.TryParse<ClearBufferMask> (bit, out mask))
				gl.Clear (mask);
		}

		public void clearColorBufferBit () {
			gl.Clear (ClearBufferMask.ColorBufferBit);
		}

		public void clearDepthBufferBit () {
			gl.Clear (ClearBufferMask.DepthBufferBit);
		}

		public void clearStencilBufferBit () {
			gl.Clear (ClearBufferMask.StencilBufferBit);
		}
	}
}

