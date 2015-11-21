using System;
using OpenTK.Graphics.OpenGL4;

namespace nginz
{
	public class FragmentShader : BasicShader
	{
		public FragmentShader (string[] sources)
			: base (ShaderType.FragmentShader, sources) {
		}
	}
}

