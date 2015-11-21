using System;
using OpenTK.Graphics.OpenGL4;

namespace nginz
{
	public class VertexShader : BasicShader
	{
		public VertexShader (params string[] sources)
			: base (ShaderType.VertexShader, sources) {
		}
	}
}

