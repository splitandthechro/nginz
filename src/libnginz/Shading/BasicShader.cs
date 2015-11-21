using System;
using nginz.Common;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using System.IO;

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
		/// Create a shader with the specified type.
		/// </summary>
		/// <param name="type">Type.</param>
		/// <param name="sources">Sources.</param>
		public static BasicShader Create (ShaderType type, params string[] sources) {

			// Check if another shader class should be returned
			switch (type) {

			// Return vertex shader
			case ShaderType.VertexShader:
				return new VertexShader (sources);

			// Return geometry shader
			case ShaderType.GeometryShader:
				return new GeometryShader (sources);

			// Return fragment shader
			case ShaderType.FragmentShader:
				return new FragmentShader (sources);
			}

			// Return basic shader
			return new BasicShader (type, sources);
		}

		/// <summary>
		/// Creates a shader from the specified file
		/// </summary>
		/// <returns>The shader.</returns>
		/// <param name="type">Type.</param>
		/// <param name="path">Path.</param>
		public static BasicShader FromFile (ShaderType type, string path) {

			// Get the full path
			var fullpath = Path.GetFullPath (path);

			// Throw if the file doesn't exist
			if (!File.Exists (fullpath))
				LogExtensions.Throw ("Could not load {0} from file. Reason: File not found: {1}", type, path);

			// Read the source code from the file
			var source = File.ReadAllText (fullpath);

			// Create and return the shader
			return Create (type, path);
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

