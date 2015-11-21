using System;
using OpenTK.Graphics.OpenGL4;

namespace nginz
{
	/// <summary>
	/// Shader program handle.
	/// </summary>
	public class ShaderProgramHandle : IDisposable
	{
		/// <summary>
		/// The previous shader program.
		/// </summary>
		readonly int previous;

		/// <summary>
		/// Initializes a new instance of the <see cref="nginz.ShaderProgramHandle"/> class.
		/// </summary>
		/// <param name="prev">Previous shader program.</param>
		public ShaderProgramHandle (int prev) {
			previous = prev;
		}

		#region IDisposable implementation

		/// <summary>
		/// Release all resources used by the <see cref="nginz.ShaderProgramHandle"/> object.
		/// </summary>
		/// <remarks>Call <see cref="Dispose"/> when you are finished using the <see cref="nginz.ShaderProgramHandle"/>. The
		/// <see cref="Dispose"/> method leaves the <see cref="nginz.ShaderProgramHandle"/> in an unusable state. After
		/// calling <see cref="Dispose"/>, you must release all references to the <see cref="nginz.ShaderProgramHandle"/> so
		/// the garbage collector can reclaim the memory that the <see cref="nginz.ShaderProgramHandle"/> was occupying.</remarks>
		public void Dispose () {

			// Make sure that the current shader program doesn't equal the previous one
			if (ShaderProgram.CurrentProgramId != previous) {

				// Set the current shader program id to the previous one
				ShaderProgram.CurrentProgramId = previous;

				// Use the previous shader program
				GL.UseProgram (previous);
			}
		}

		#endregion
	}
}

