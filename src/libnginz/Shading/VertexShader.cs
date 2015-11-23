using System;
using OpenTK.Graphics.OpenGL4;

namespace nginz
{
	/// <summary>
	/// Vertex shader.
	/// </summary>
	public class VertexShader : BasicShader
	{

		/// <summary>
		/// Initializes a new instance of the <see cref="nginz.VertexShader"/> class.
		/// </summary>
		/// <param name="sources">Sources.</param>
		public VertexShader (params string[] sources)
			: base (ShaderType.VertexShader, sources) {
		}
	}
}

