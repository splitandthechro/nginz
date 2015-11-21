using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using System.Collections.Generic;

namespace nginz
{
	/// <summary>
	/// Shader program.
	/// </summary>
	public class ShaderProgram : IDisposable
	{
		/// <summary>
		/// The shader objects.
		/// </summary>
		List<Shader> shaderObjects;

		/// <summary>
		/// The program identifier.
		/// </summary>
		int programId;

		/// <summary>
		/// Gets the program identifier.
		/// </summary>
		/// <value>The program identifier.</value>
		public int ProgramId { get { return programId; } }

		/// <summary>
		/// Initializes a new instance of the <see cref="nginz.ShaderProgram"/> class.
		/// </summary>
		/// <param name="shaders">Shaders.</param>
		public ShaderProgram (params Shader[] shaders) {

			// Create the shader program
			programId = GL.CreateProgram ();

			// Initialize the shaderObjects list with the shaders 
			shaderObjects = new List<Shader> (shaders);

			// Attach all shaders to the program
			AttachAll ();
		}

		/// <summary>
		/// Attach the specified shader.
		/// </summary>
		/// <param name="shader">Shader.</param>
		public void Attach (Shader shader) {

			// Attach the shader to the program
			GL.AttachShader (programId, shader.ShaderId);

			// Add the shader to the shaderObjects list
			shaderObjects.Add (shader);
		}

		/// <summary>
		/// Attach all shaders.
		/// </summary>
		void AttachAll () {

			// Attach all shaders to the program
			for (var i = 0; i < shaderObjects.Count; i++)
				GL.AttachShader (programId, shaderObjects [i].ShaderId);
		}

		/// <summary>
		/// Detach all shaders
		/// </summary>
		void DetachAll () {

			// Detach all shaders from the program
			// and clear the shaderObjects list
			foreach (var shader in shaderObjects) {

				// Detach the shader
				GL.DetachShader (programId, shader.ShaderId);

				// Remove the shader
				shaderObjects.Remove (shader);
			}
		}

		#region IDisposable implementation

		/// <summary>
		/// Release all resource used by the <see cref="nginz.ShaderProgram"/> object.
		/// </summary>
		/// <remarks>Call <see cref="Dispose"/> when you are finished using the <see cref="nginz.ShaderProgram"/>. The
		/// <see cref="Dispose"/> method leaves the <see cref="nginz.ShaderProgram"/> in an unusable state. After calling
		/// <see cref="Dispose"/>, you must release all references to the <see cref="nginz.ShaderProgram"/> so the garbage
		/// collector can reclaim the memory that the <see cref="nginz.ShaderProgram"/> was occupying.</remarks>
		public void Dispose () {

			// Delete the program if its id is not -1
			if (programId != -1)
				GL.DeleteProgram (programId);

			// Set the program id to -1
			programId = -1;
		}

		#endregion
	}
}

