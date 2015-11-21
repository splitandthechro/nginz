using System;
using OpenTK.Graphics.OpenGL4;

namespace nginz
{
	public class GeometryShader : BasicShader
	{
		public GeometryShader (string[] sources)
			: base (ShaderType.GeometryShader, sources) {
		}
	}
}

