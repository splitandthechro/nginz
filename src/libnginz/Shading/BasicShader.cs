using System;
using nginz.Common;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;

namespace nginz
{
	/// <summary>
	/// Basic shader.
	/// </summary>
	public class BasicShader : Shader, ICanThrow
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="nginz.BasicShader"/> class.
		/// </summary>
		/// <param name="type">Type.</param>
		/// <param name="sources">Sources.</param>
		public BasicShader (ShaderType type, params string[] sources) : base (type, sources) {
		}

		/// <summary>
		/// Compile the shader.
		/// </summary>
		public override void Compile () {

			// Create the shader
			shaderId = GL.CreateShader (shaderType);

			var lengths = new int[shaderSources.Length];
			for (var i = 0; i < lengths.Length; i++)
				lengths [i] = -1;

			// Load the shader sources
			GL.ShaderSource (
				shader: shaderId,
				count: shaderSources.Length,
				@string: shaderSources,
				length: ref lengths [0]
			);

			// Compile the shader
			GL.CompileShader (shaderId);

			// Get the shader compile status
			int status;
			GL.GetShader (
				shader: shaderId,
				pname: ShaderParameter.CompileStatus,
				@params: out status
			);

			// Check if there was an error compiling the shader
			if (status == 0) {

				// Get the error message
				var error = GL.GetShaderInfoLog (shaderId);

				// Throw an exception
				this.Throw ("Could not compile {0}: {1}", shaderType, error);
			}
		}
	}
}

