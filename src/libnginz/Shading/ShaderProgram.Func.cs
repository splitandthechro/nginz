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
	public partial class ShaderProgram
	{

		/// <summary>
		/// Binds an attribute location.
		/// </summary>
		/// <param name="index">Index.</param>
		/// <param name="name">Name.</param>
		public void BindAttribLocation (int index, string name) {
			GL.BindAttribLocation (programId, index, name);
		}

		/// <summary>
		/// Binds an attribute location.
		/// </summary>
		/// <param name="attrs">Attrs.</param>
		public void BindAttribLocation (params KeyValuePair<int, string>[] attrs) {
			for (var i = 0; i < attrs.Length; i++)
				BindAttribLocation (attrs [i].Key, attrs [i].Value);
		}

		/// <summary>
		/// Binds an attribute location.
		/// </summary>
		/// <param name="attrs">Attrs.</param>
		public void BindAttribLocation (params Tuple<int, string>[] attrs) {
			for (var i = 0; i < attrs.Length; i++)
				BindAttribLocation (attrs [i].Item1, attrs [i].Item2);
		}

		/// <summary>
		/// Binds a fragment data location.
		/// </summary>
		/// <param name="color">Color.</param>
		/// <param name="name">Name.</param>
		public void BindFragDataLocation (int color, string name) {
			GL.BindFragDataLocation (programId, color, name);
		}

		/// <summary>
		/// Binds a fragment data location.
		/// </summary>
		/// <param name="data">Data.</param>
		public void BindFragDataLocation (params KeyValuePair<int, string>[] data) {
			for (var i = 0; i < data.Length; i++)
				BindFragDataLocation (data [i].Key, data [i].Value);
		}

		/// <summary>
		/// Binds a fragment data location.
		/// </summary>
		/// <param name="data">Data.</param>
		public void BindFragDataLocation (params Tuple<int, string>[] data) {
			for (var i = 0; i < data.Length; i++)
				BindFragDataLocation (data [i].Item1, data [i].Item2);
		}

		/// <summary>
		/// Transform feedback varyings.
		/// </summary>
		/// <param name="mode">Mode.</param>
		/// <param name="varyings">Varyings.</param>
		public void TransformFeedbackVaryings (TransformFeedbackMode mode, params string[] varyings) {
			GL.TransformFeedbackVaryings (programId, varyings.Length, varyings, mode);
		}

		/// <summary>
		/// Transform feedback varyings.
		/// </summary>
		/// <param name="data">Data.</param>
		public void TransformFeedbackVaryings (params KeyValuePair<TransformFeedbackMode, string[]>[] data) {
			for (var i = 0; i < data.Length; i++)
				TransformFeedbackVaryings (data [i].Key, data [i].Value);
		}

		/// <summary>
		/// Transform feedback varyings.
		/// </summary>
		/// <param name="data">Data.</param>
		public void TransformFeedbackVaryings (params Tuple<TransformFeedbackMode, string[]>[] data) {
			for (var i = 0; i < data.Length; i++)
				TransformFeedbackVaryings (data [i].Item1, data [i].Item2);
		}
	}
}

