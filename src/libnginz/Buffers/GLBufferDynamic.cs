using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using nginz.Common;
using OpenTK.Graphics.OpenGL4;

namespace nginz {
	public class GLBufferDynamic<T> : IBuffer<int>, ICanThrow where T : struct {

		/// <summary>
		/// The buffer identifier.
		/// </summary>
		readonly public int BufferId;

		/// <summary>
		/// The settings.
		/// </summary>
		public GLBufferSettings Settings;

		public int BufferSize { get; set; }

		/// <summary>
		/// Gets the size of the element.
		/// </summary>
		/// <value>The size of the element.</value>
		public int ElementSize { get; }

		public GLBufferDynamic (GLBufferSettings settings, int elementSize, int startCapacity = 8192) {
			Settings = settings;
			BufferSize = startCapacity;
			ElementSize = elementSize;

			BufferId = GL.GenBuffer ();
			Bind ();
			GL.BufferData (Settings.Target, BufferSize, IntPtr.Zero, Settings.Hint);
			Unbind ();
		}

		public void UploadData (IList<T> data) {

			// Calculate the buffer size
			var bufferSize = Marshal.SizeOf (data[0]) * data.Count;

			Bind ();
			GL.BufferData (Settings.Target, bufferSize, data.ToArray (), Settings.Hint);
			Unbind ();
		}

		public static void Bind<BuffType> (GLBufferDynamic<BuffType> buffer) where BuffType : struct {
			GL.BindBuffer (buffer.Settings.Target, buffer.BufferId);
		}

		public static void Unbind<BuffType>(GLBufferDynamic<BuffType> buffer) where BuffType : struct {
			GL.BindBuffer (buffer.Settings.Target, 0);
		}

		public void Bind () {
			GLBufferDynamic<T>.Bind (this);
		}

		public void Unbind () {
			GLBufferDynamic<T>.Unbind (this);
		}

		public void PointTo (int where) {

			// Bind buffer
			Bind ();

			// Enable vertex attribute array
			GL.EnableVertexAttribArray (where);

			// Set vertex attribute pointer
			GL.VertexAttribPointer (where, Settings.AttribSize, Settings.Type, Settings.Normalized, ElementSize, Settings.Offset);

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
	}
}
