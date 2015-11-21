using System;
using System.Collections.Generic;
using nginz.Common;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;

namespace nginz
{
	/// <summary>
	/// Shader program.
	/// </summary>
	public partial class ShaderProgram
	{

		/// <summary>
		/// The uniforms.
		/// </summary>
		Dictionary<string, int> uniforms;

		/// <summary>
		/// Gets or sets the specified uniform.
		/// </summary>
		/// <param name="uniform">Uniform.</param>
		public object this [string uniform] {
			get { return GetUniform (uniform); }
			set { SetUniform (uniform, value); }
		}

		/// <summary>
		/// Gets or sets the specfied uniform by its identifier.
		/// </summary>
		/// <param name="uniformId">Uniform identifier.</param>
		public object this [int uniformId] {
			set { SetUniformValue (uniformId, value); }
		}

		/// <summary>
		/// Gets the uniform.
		/// </summary>
		/// <returns>The uniform.</returns>
		/// <param name="uniform">Uniform.</param>
		int GetUniform (string uniform) {

			// Check if the uniform exists
			if (!uniforms.ContainsKey (uniform)) {

				// Set the uniform
				uniforms [uniform] = GL.GetUniformLocation (programId, uniform);
			}

			// Return the uniform
			return uniforms [uniform];
		}

		/// <summary>
		/// Sets the uniform.
		/// </summary>
		/// <param name="uniform">Uniform.</param>
		/// <param name="value">Value.</param>
		void SetUniform (string uniform, object value) {

			// Check if another program is loaded
			if (CurrentProgramId != 0 && CurrentProgramId != programId) {

				// Throw an exception
				const string message = "Cannot set uniform {0} on program {1} because the current program is {2}.";
				this.Throw (message, uniform, programId, CurrentProgramId);
			}

			// Set the uniform value
			this [(int) this [uniform]] = value;
		}

		/// <summary>
		/// Sets the uniform value.
		/// </summary>
		/// <param name="uniformId">Uniform identifier.</param>
		/// <param name="value">Value.</param>
		void SetUniformValue (int uniformId, object value) {

			// Check if another program is loaded
			if (CurrentProgramId != 0 && CurrentProgramId != programId) {

				// Throw an exception
				const string message = "Cannot set uniform {0} on program {1} because the current program is {2}.";
				this.Throw (message, uniformId, programId, CurrentProgramId);
			}

			// Set the uniform
			using (UseProgram ()) {
				TypeSwitch.On (value)
				.Case ((int x) => GL.Uniform1 (uniformId, x))
				.Case ((uint x) => GL.Uniform1 (uniformId, x))
				.Case ((float x) => GL.Uniform1 (uniformId, x))
				.Case ((Vector2 x) => GL.Uniform2 (uniformId, x))
				.Case ((Vector3 x) => GL.Uniform3 (uniformId, x))
				.Case ((Vector4 x) => GL.Uniform4 (uniformId, x))
				.Case ((Quaternion x) => GL.Uniform4 (uniformId, x))
				.Case ((Color4 x) => GL.Uniform4 (uniformId, x))
				.Case ((int[] x) => GL.Uniform1 (uniformId, x.Length, x))
				.Case ((uint[] x) => GL.Uniform1 (uniformId, x.Length, x))
				.Case ((float[] x) => GL.Uniform1 (uniformId, x.Length, x))
				.Case ((Matrix4 x) => GL.UniformMatrix4 (uniformId, false, ref x))
				.Default (x => this.Throw ("GlUniform type {0} is not (yet?) implemented.", value.GetType ().FullName));
			}
		}
	}
}

