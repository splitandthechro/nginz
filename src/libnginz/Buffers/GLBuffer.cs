using System;
using System.Runtime.InteropServices;
using nginz.Common;
using OpenTK.Graphics.OpenGL4;

namespace nginz {

	/// <summary>
	/// GL buffer.
	/// </summary>
	public class GLBuffer<T> : IBind<int>, IPointTo<int> where T : struct {

		/// <summary>
		/// The buffer identifier.
		/// </summary>
		readonly public int BufferId;

		public GLBufferSettings Settings;

		/// <summary>
		/// Gets or sets the size of the buffer.
		/// </summary>
		/// <value>The size of the buffer.</value>
		public int BufferSize { get; private set; }

		/// <summary>
		/// Gets the size of the element.
		/// </summary>
		/// <value>The size of the element.</value>
		public int ElementSize { get { return BufferSize / Buffer.Length; } }

		/// <summary>
		/// Gets or sets the buffer.
		/// </summary>
		/// <value>The buffer.</value>
		public T[] Buffer { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="nginz.GLBuffer{T}"/> class.
		/// </summary>
		/// <param name="target">Target.</param>
		/// <param name="buffer">Buffer.</param>
		/// <param name="hint">Hint.</param>
		public GLBuffer (GLBufferSettings settings, T[] buffer) {
			Settings = settings;

			// Set the buffer
			Buffer = buffer;

			// Calculate the buffer size
			BufferSize = Marshal.SizeOf (buffer [0]) * Buffer.Length;

			// Generate the buffer
			BufferId = GL.GenBuffer ();

			// Bind the buffer
			Bind ();

			// Set the buffer data
			GL.BufferData (Settings.Target, BufferSize, Buffer, Settings.Hint);

			// Unbind the data
			Unbind ();
		}

		/// <summary>
		/// Point the buffer to an index
		/// </summary>
		/// <param name="index">Index.</param>
		/// <param name="type">Type.</param>
		/// <param name="normalized">If set to <c>true</c> normalized.</param>
		/// <param name="offset">Offset.</param>
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

		/// <summary>
		/// Bind the specified buffer.
		/// </summary>
		/// <param name="buffer">Buffer.</param>
		/// <typeparam name="BuffType">The 1st type parameter.</typeparam>
		public static void Bind<BuffType> (GLBuffer<BuffType> buffer) where BuffType : struct {
			GL.BindBuffer (buffer.Settings.Target, buffer.BufferId);
		}

		/// <summary>
		/// Bind the buffer.
		/// </summary>
		public void Bind () {
			GLBuffer<T>.Bind (this);
		}

		/// <summary>
		/// Unbind the specified buffer.
		/// </summary>
		/// <param name="buffer">Buffer.</param>
		/// <typeparam name="BuffType">The 1st type parameter.</typeparam>
		public static void Unbind<BuffType>(GLBuffer<BuffType> buffer) where BuffType : struct {
			GL.BindBuffer (buffer.Settings.Target, 0);
		}

		/// <summary>
		/// Unbind the buffer.
		/// </summary>
		public void Unbind () {
			GLBuffer<T>.Unbind (this);
		}
	}

	public struct GLBufferSettings {
		public BufferTarget Target;
		public BufferUsageHint Hint;
		public int AttribSize;
		public VertexAttribPointerType Type;
		public bool Normalized;
		public int Offset;

		public static GLBufferSettings StaticDraw3FloatArray = new GLBufferSettings {
			Target = BufferTarget.ArrayBuffer,
			Hint = BufferUsageHint.StaticDraw,
			AttribSize = 3,
			Type = VertexAttribPointerType.Float
		};

		public static GLBufferSettings Indices = new GLBufferSettings {
			Target = BufferTarget.ElementArrayBuffer,
			Hint = BufferUsageHint.StaticDraw
		};
	}
}
