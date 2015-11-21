using System;
using System.IO;
using System.Text;
using OpenTK.Graphics.OpenGL4;

namespace nginz
{
	/// <summary>
	/// Shader extensions.
	/// </summary>
	public static class ShaderExtensions
	{
		/// <summary>
		/// Loads a shader from a file.
		/// </summary>
		/// <returns>The file.</returns>
		/// <param name="shader">Shader.</param>
		/// <param name="type">Type.</param>
		/// <param name="path">Path.</param>
		/// <typeparam name="TShader">The 1st type parameter.</typeparam>
		public static TShader FromFile<TShader> (this TShader shader, ShaderType type, string path)
			where TShader : BasicShader {
			var source = File.ReadAllText (path, Encoding.UTF8);
			return (TShader)new BasicShader (type, source);
		}
	}
}

