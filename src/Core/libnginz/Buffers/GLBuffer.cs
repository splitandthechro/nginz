using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using nginz.Common;
using OpenTK.Graphics.OpenGL4;

namespace nginz {

	/// <summary>
	/// OpenGL Buffer.
	/// </summary>
	public class GLBuffer<T> : ICanThrow, IBuffer<int> where T : struct {

		/// <summary>
		/// The buffer identifier.
		/// </summary>
		readonly public int BufferId;

		/// <summary>
		/// The settings.
		/// </summary>
		public GLBufferSettings Settings;

		/// <summary>
		/// Gets or sets the size of the buffer.
		/// </summary>
		/// <value>The size of the buffer.</value>
		public int BufferSize { get; set; }

		/// <summary>
		/// Gets the size of the element.
		/// </summary>
		/// <value>The size of the element.</value>
		public int ElementSize { get { return BufferSize / Buffer.Count; } }

		/// <summary>
		/// Gets or sets the buffer.
		/// </summary>
		/// <value>The buffer.</value>
		public IList<T> Buffer { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="nginz.GLBuffer{T}"/> class.
		/// </summary>
		/// <param name="settings">The buffer settings.</param>
		/// <param name="buffer">The buffer initialization array.</param>
		public GLBuffer (GLBufferSettings settings, IList<T> buffer) {

			// Set the settings
			Settings = settings;

			// Set the buffer
			Buffer = buffer;

			// Calculate the buffer size
			BufferSize = Marshal.SizeOf (buffer [0]) * Buffer.Count;

			// Generate the buffer
			BufferId = GL.GenBuffer ();

			// Bind the buffer
			Bind ();

			// Set the buffer data
			GL.BufferData (Settings.Target, BufferSize, Buffer.ToArray (), Settings.Hint);

			// Unbind the data
			Unbind ();
		}

		/// <summary>
		/// Point the buffer to an index
		/// </summary>
		/// <param name="index">Index.</param>
		public void PointTo (int index) {

			// Bind buffer
			Bind ();

			// Enable vertex attribute array
			GL.EnableVertexAttribArray (index);

			// Set vertex attribute pointer
			GL.VertexAttribPointer (index, Settings.AttribSize, Settings.Type, Settings.Normalized, ElementSize, Settings.Offset);

			// Unbind buffer
			Unbind ();
		}

		public void PointTo (int where, int offset) {

			// Bind buffer
			Bind ();

			// Enable vertex attribute array
			GL.EnableVertexAttribArray (where);

			// Set vertex attribute pointer
			GL.VertexAttribPointer (where, Settings.AttribSize, Settings.Type, Settings.Normalized, ElementSize, offset);

			// Unbind buffer
			Unbind ();
		}

		public void PointTo (int where, params int[] other) {

			// Check if the other parameter is null
			if (other == null)
				this.Throw ("Cannot set attribute pointer: other is null");

			// Check if other contains at least two elements
			if (other.Length < 2)
				this.Throw ("Cannot set attribute pointer: other contains less than two elements");

			// Bind buffer
			Bind ();

			// Enable vertex attribute array
			GL.EnableVertexAttribArray (where);

			// Set vertex attribute pointer
			GL.VertexAttribPointer (where, other [0], Settings.Type, Settings.Normalized, ElementSize, other [1]);

			// Unbind buffer
			Unbind ();
		}

		/// <summary>
		/// Bind the specified buffer.
		/// </summary>
		/// <param name="buffer">Buffer.</param>
		/// <typeparam name="BuffType">The 1st type parameter.</typeparam>
		public static void Bind<BuffType> (GLBuffer<BuffType> buffer) where BuffType : struct {

			// Check if the buffer is null
			if (buffer == null)
				LogExtensions.ThrowStatic ("Cannot bind buffer: buffer is null");
			
			// Bind the buffer
			GL.BindBuffer (buffer.Settings.Target, buffer.BufferId);
		}

		/// <summary>
		/// Bind the buffer.
		/// </summary>
		public void Bind () {

			// Bind the buffer
			GLBuffer<T>.Bind (this);
		}

		/// <summary>
		/// Unbind the specified buffer.
		/// </summary>
		/// <param name="buffer">Buffer.</param>
		/// <typeparam name="BuffType">The 1st type parameter.</typeparam>
		public static void Unbind<BuffType>(GLBuffer<BuffType> buffer) where BuffType : struct {

			// Check if the buffer is null
			if (buffer == null)
				LogExtensions.ThrowStatic ("Cannot unbind buffer: buffer is null");

			// Unbind the buffer
			GL.BindBuffer (buffer.Settings.Target, 0);
		}

		/// <summary>
		/// Unbind the buffer.
		/// </summary>
		public void Unbind () {

			// Unbind the buffer
			GLBuffer<T>.Unbind (this);
		}
	}
}
