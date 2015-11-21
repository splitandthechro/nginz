using System;
using OpenTK.Graphics.OpenGL4;

namespace nginz
{
	public class GeometryShader : BasicShader
	{
		public GeometryShader (params string[] sources)
			: base (ShaderType.GeometryShader, sources) {
		}
	}
}

