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
		public int ElementSize { get; private set; }

		public GLBufferDynamic (GLBufferSettings settings, int elementSize, int startCapacity = 8192) {
			Settings = settings;
			BufferSize = startCapacity;
			ElementSize = elementSize;

			BufferId = GL.GenBuffer ();
			Bind ();
			GL.BufferData (Settings.Target, BufferSize, IntPtr.Zero, Settings.Hint);
			Unbind ();
		}

		public void UploadData (T[] dataArray) {

			// Check if data is null
			if (dataArray == null)
				this.Throw ("Cannot upload data: data is null");

			// Calculate the buffer size
			var bufferSize = Marshal.SizeOf (dataArray[0]) * dataArray.Length;

			Bind ();
			GL.BufferData (Settings.Target, bufferSize, dataArray, Settings.Hint);
			Unbind ();
		}

		public void UploadData (IList<T> dataList) {

			// Check if data is null
			if (dataList == null)
				this.Throw ("Cannot upload data: data is null");

			// Calculate the buffer size
			var bufferSize = Marshal.SizeOf (dataList[0]) * dataList.Count;

			Bind ();
			GL.BufferData (Settings.Target, bufferSize, dataList.ToArray (), Settings.Hint);
			Unbind ();
		}

		public static void Bind<BuffType> (GLBufferDynamic<BuffType> buffer) where BuffType : struct {

			// Check if buffer is null
			if (buffer == null)
				LogExtensions.ThrowStatic ("Cannot bind buffer: buffer is null");
			
			GL.BindBuffer (buffer.Settings.Target, buffer.BufferId);
		}

		public static void Unbind<BuffType>(GLBufferDynamic<BuffType> buffer) where BuffType : struct {
			
			// Check if buffer is null
			if (buffer == null)
				LogExtensions.ThrowStatic ("Cannot unbind buffer: buffer is null");

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
			GL.VertexAttribPointer (where, other[0], Settings.Type, Settings.Normalized, ElementSize, other[1]);

			// Unbind buffer
			Unbind ();
		}
	}
}
