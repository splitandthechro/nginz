using System;
using OpenTK.Graphics.OpenGL4;

namespace nginz
{
	/// <summary>
	/// Fragment shader.
	/// </summary>
	public class FragmentShader : BasicShader
	{

		/// <summary>
		/// Initializes a new instance of the <see cref="nginz.FragmentShader"/> class.
		/// </summary>
		/// <param name="sources">Sources.</param>
		public FragmentShader (params string[] sources)
			: base (ShaderType.FragmentShader, sources) {
		}
	}
}

