using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using nginz.Common;
using OpenTK.Graphics.OpenGL4;

namespace nginz {
	public class Geometry : IBind<int> {
		public Dictionary<string, IBind<int>> Buffers;
		public GLBuffer<uint> Indices;

		int abo = -1;

		public Geometry () {
			Buffers = new Dictionary<string, IBind<int>> ();
			abo = GL.GenVertexArray ();
		}

		public Geometry AddBuffer (string name, IBind<int> buffer) {
			this.Buffers[name] = buffer;
			return this;
		}

		public Geometry Attribute<T> (ShaderProgram program, string name, int size, VertexAttribPointerType type, bool normalized = false, int offset = 0) where T : struct{
			((GLBuffer<T>) this.Buffers[name]).PointTo (program.Attrib (name));
			return this;
		}

		public Geometry SetIndices (GLBuffer<uint> indices) {
			this.Indices = indices;
			return this;
		}

		public static void Bind (Geometry @this) {
			GL.BindVertexArray (@this.abo);
			@this.Indices.Bind ();
			@this.Buffers.ToList ().ForEach (kvp => {
				kvp.Value.Bind ();
			});
		}

		public void Bind () {
			Bind (this);
		}

		public static void Unbind (Geometry @this) {
			GL.BindVertexArray (0);
			@this.Indices.Unbind ();
			@this.Buffers.ToList ().ForEach (kvp => {
				kvp.Value.Unbind ();
			});
		}

		public void Unbind () {
			Unbind (this);
		}

		public Geometry Construct (ShaderProgram program) {
			Bind ();
			Buffers.ToList ().ForEach (kvp => {
				kvp.Value.PointTo (program.Attrib (kvp.Key));
			});
			Unbind ();
			return this;
		}

		public void PointTo (int where) {
			throw new NotImplementedException ();
		}

		public void Draw (BeginMode mode, int offset = 0) {
			Bind ();
			GL.DrawElements (mode, this.Indices.Buffer.Length, DrawElementsType.UnsignedInt, offset);
		}
	}
}
