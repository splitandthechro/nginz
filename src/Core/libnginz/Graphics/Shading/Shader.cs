﻿using System;
using OpenTK.Graphics.OpenGL4;

namespace nginz
{
	/// <summary>
	/// Basic shader.
	/// </summary>
	public abstract class Shader : IDisposable
	{
		/// <summary>
		/// The type of the shader.
		/// </summary>
		protected ShaderType shaderType;

		/// <summary>
		/// The sources.
		/// </summary>
		protected string[] shaderSources;

		/// <summary>
		/// The shader identifier.
		/// </summary>
		protected int internalShaderId;

		/// <summary>
		/// Gets the shader identifier.
		/// </summary>
		/// <value>The shader identifier.</value>
		public int ShaderId { get { return internalShaderId; } }

		/// <summary>
		/// Initializes a new instance of the <see cref="nginz.Shader"/> class.
		/// </summary>
		/// <param name="type">Type.</param>
		/// <param name="sources">Sources.</param>
		protected Shader (ShaderType type, params string[] sources) {
			shaderType = type;
			shaderSources = sources;
		}

		public abstract void Compile ();

		#region IDisposable implementation

		/// <summary>
		/// Releases all resource used by the <see cref="nginz.BasicShader"/> object.
		/// </summary>
		/// <remarks>Call <see cref="Dispose"/> when you are finished using the <see cref="nginz.BasicShader"/>. The
		/// <see cref="Dispose"/> method leaves the <see cref="nginz.BasicShader"/> in an unusable state. After calling
		/// <see cref="Dispose"/>, you must release all references to the <see cref="nginz.BasicShader"/> so the garbage
		/// collector can reclaim the memory that the <see cref="nginz.BasicShader"/> was occupying.</remarks>
		public virtual void Dispose () {

			// Delete the shader if its id is not -1
			if (internalShaderId != -1)
				GL.DeleteShader (internalShaderId);

			// Set the shader id to -1
			internalShaderId = -1;
		}

		#endregion
	}
}

