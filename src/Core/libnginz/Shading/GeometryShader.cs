using System;
using OpenTK.Graphics.OpenGL4;

namespace nginz
{

	/// <summary>
	/// Geometry shader.
	/// </summary>
	public class GeometryShader : BasicShader
	{

		/// <summary>
		/// Initializes a new instance of the <see cref="nginz.GeometryShader"/> class.
		/// </summary>
		/// <param name="sources">Sources.</param>
		public GeometryShader (params string[] sources)
			: base (ShaderType.GeometryShader, sources) {
		}
	}
}

