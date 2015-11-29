using System;
using System.Collections.Generic;
using nginz.Common;
using OpenTK.Graphics.OpenGL4;

namespace nginz
{
	/// <summary>
	/// Shader program.
	/// </summary>
	public partial class ShaderProgram : ICanThrow, IDisposable, Asset
	{
		/// <summary>
		/// The current shader program identifier.
		/// </summary>
		public static int CurrentProgramId;

		/// <summary>
		/// Initializes the <see cref="nginz.ShaderProgram"/> class.
		/// </summary>
		static ShaderProgram () {

			// Initialize the current program id to 0
			CurrentProgramId = 0;
		}

		/// <summary>
		/// The shader objects.
		/// </summary>
		readonly List<Shader> shaderObjects;

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

			// Initialize uniforms
			uniforms = new Dictionary<string, int> ();

			attributes = new Dictionary<string, int> ();

			// Create the shader program
			programId = GL.CreateProgram ();

			// Initialize the shaderObjects list with the shaders 
			shaderObjects = new List<Shader> (shaders.Length);

			// Attach all shaders to the program
			foreach (var shader in shaders)
				Attach (shader);
		}

		public void Use (Action act) {
			using (UseProgram ())
				act ();
		}

		/// <summary>
		/// Use the shader program.
		/// </summary>
		/// <returns>The handle of the previous shader program.</returns>
		ShaderProgramHandle UseProgram () {

			// Get the handle of the current shader program
			var handle = new ShaderProgramHandle (CurrentProgramId);

			// Check if the shader program doesn't equal the current one
			if (CurrentProgramId != programId) {

				// Use this shader program
				GL.UseProgram (programId);

				// Make the shader program the current one
				CurrentProgramId = programId;
			}

			// Return the handle of the previous shader program
			return handle;
		}

		/// <summary>
		/// Link the program.
		/// </summary>
		public ShaderProgram Link () {

			// Link the shader program
			GL.LinkProgram (programId);

			// Ge the shader link status
			int status;
			GL.GetProgram (
				program: programId,
				pname: GetProgramParameterName.LinkStatus,
				@params: out status
			);

			// Check if there was an error linking the shader
			if (status == 0) {

				// Get the error message
				var error = GL.GetProgramInfoLog (programId);

				// Throw an exception
				this.Throw ("Could not link program: {0}", error);
			}

			return this;
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

		public void Detach (Shader shader) {

			// Check if the shader is loaded
			if (shaderObjects.Contains (shader)) {
			
				// Detach the shader from the program
				GL.DetachShader (programId, shader.ShaderId);

				// Remove the shader from the shaderObjects list
				shaderObjects.Remove (shader);
			}
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

			// Clear shaders
			// TODO: Check if detaching the shaders is needed
			shaderObjects.Clear ();

			// Set the program id to -1
			programId = -1;
		}

		#endregion
	}
}

