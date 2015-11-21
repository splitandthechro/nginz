using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using OpenTK.Graphics.OpenGL4;

namespace nginz {
	public class GLBuffer<Type> where Type : struct {
		public int BufferId { get; }
		public int BufferSize { get; set; }
		public int ElementSize {
			get {
				return this.BufferSize / this.Buffer.Length;
			}
		}

		public Type[] Buffer { get; set; }
		public BufferUsageHint Hint { get; }

		readonly BufferTarget Target;

		public GLBuffer (BufferTarget target, Type[] buffer, BufferUsageHint hint) {
			this.Target = target;
			this.Buffer = buffer;
			this.BufferSize = Marshal.SizeOf (buffer[0]) * this.Buffer.Length;
			this.Hint = hint;

			this.BufferId = GL.GenBuffer ();
			this.Bind ();
			GL.BufferData (this.Target, this.BufferSize, this.Buffer, this.Hint);
			this.UnBind ();
		}

		public void PointTo (int index, VertexAttribPointerType type, bool normalized = false, int offset = 0) {
			this.Bind ();
			GL.EnableVertexAttribArray (index);
			GL.VertexAttribPointer (index, this.Buffer.Length, type, normalized, this.ElementSize, offset);
			this.UnBind ();
		}

		public static void Bind<BuffType> (GLBuffer<BuffType> buffer) where BuffType : struct {
			GL.BindBuffer (buffer.Target, buffer.BufferId);
		}

		public void Bind () {
			GLBuffer<Type>.Bind (this);
		}

		public static void UnBind<BuffType>(GLBuffer<BuffType> buffer) where BuffType : struct {
			GL.BindBuffer (buffer.Target, 0);
		}

		public void UnBind () {
			GLBuffer<Type>.UnBind (this);
		}
	}
}
