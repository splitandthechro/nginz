using System;
using nginz.Common;
using OpenTK.Graphics.OpenGL4;
using System.IO;
using System.Text;

namespace nginz
{
	/// <summary>
	/// Basic shader.
	/// </summary>
	public class BasicShader : Shader, ICanThrow, IAsset
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="nginz.BasicShader"/> class.
		/// </summary>
		/// <param name="type">Type.</param>
		/// <param name="sources">Sources.</param>
		public BasicShader (ShaderType type, params string[] sources) : base (type, sources) {
			// Analysis disable DoNotCallOverridableMethodsInConstructor
			Compile ();
			// Analysis restore DoNotCallOverridableMethodsInConstructor
		}

		/// <summary>
		/// Create a shader with the specified type.
		/// </summary>
		/// <param name="sources">Sources.</param>
		public static Shader Create<Shader> (params string[] sources) where Shader : BasicShader {
			// Return basic shader
			return (Shader) Activator.CreateInstance(typeof(Shader), sources);
		}

		/// <summary>
		/// Creates a shader from the specified file
		/// </summary>
		/// <returns>The shader.</returns>
		/// <param name="path">Path.</param>
		public static Shader FromFile<Shader>(string path) where Shader : BasicShader {

			// Get the full path
			var fullpath = Path.GetFullPath (path);

			// Throw if the file doesn't exist
			if (!File.Exists (fullpath))
				LogExtensions.ThrowStatic ("Could not load {0} from file. Reason: File not found: {1}", typeof(Shader).Name, path);

			// Read the source code from the file
			var source = File.ReadAllText (fullpath, Encoding.ASCII);

			// Create and return the shader
			return Create<Shader> (source);
		}

		/// <summary>
		/// Compile the shader.
		/// </summary>
		public override void Compile () {

			// Create the shader
			internalShaderId = GL.CreateShader (shaderType);

			var lengths = new int[shaderSources.Length];
			for (var i = 0; i < lengths.Length; i++)
				lengths [i] = -1;

			// Load the shader sources
			GL.ShaderSource (
				shader: internalShaderId,
				count: shaderSources.Length,
				@string: shaderSources,
				length: ref lengths [0]
			);

			// Compile the shader
			GL.CompileShader (internalShaderId);

			// Get the shader compile status
			int status;
			GL.GetShader (
				shader: internalShaderId,
				pname: ShaderParameter.CompileStatus,
				@params: out status
			);

			// Check if there was an error compiling the shader
			if (status == 0) {

				// Get the error message
				var error = GL.GetShaderInfoLog (internalShaderId);

				// Throw an exception
				this.Throw ("Could not compile {0}: {1}", shaderType, error);
			}
		}
	}
}

