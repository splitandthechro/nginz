﻿using System;
using OpenTK.Graphics.OpenGL4;

namespace nginz
{
	/// <summary>
	/// OpenGL Buffer settings.
	/// </summary>
	public struct GLBufferSettings {

		/// <summary>
		/// The buffer target.
		/// </summary>
		public BufferTarget Target;

		/// <summary>
		/// The buffer usage hint.
		/// </summary>
		public BufferUsageHint Hint;

		/// <summary>
		/// The size of the attributes.
		/// </summary>
		public int AttribSize;

		/// <summary>
		/// The vertex attribute pointer type.
		/// </summary>
		public VertexAttribPointerType Type;

		/// <summary>
		/// Whether the buffer is normalized.
		/// </summary>
		public bool Normalized;

		/// <summary>
		/// The buffer offset.
		/// </summary>
		public int Offset;

		/// <summary>
		/// Standard configuration for statically drawn three-float objects.
		/// </summary>
		public static GLBufferSettings StaticDraw3FloatArray = new GLBufferSettings {
			Target = BufferTarget.ArrayBuffer,
			Hint = BufferUsageHint.StaticDraw,
			AttribSize = 3,
			Type = VertexAttribPointerType.Float
		};

		/// <summary>
		/// Standard configuration for statically drawn two-float objects.
		/// </summary>
		public static GLBufferSettings StaticDraw2FloatArray = new GLBufferSettings {
			Target = BufferTarget.ArrayBuffer,
			Hint = BufferUsageHint.StaticDraw,
			AttribSize = 2,
			Type = VertexAttribPointerType.Float
		};

		/// <summary>
		/// Standard configuration for indices.
		/// </summary>
		public static GLBufferSettings Indices = new GLBufferSettings {
			Target = BufferTarget.ElementArrayBuffer,
			Hint = BufferUsageHint.StaticDraw
		};
	}
}
