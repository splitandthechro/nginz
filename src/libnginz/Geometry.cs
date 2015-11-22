using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using nginz.Common;
using OpenTK.Graphics.OpenGL4;

namespace nginz {

	/// <summary>
	/// Geometry.
	/// </summary>
	public class Geometry : IBind {

		/// <summary>
		/// The buffers.
		/// </summary>
		public Dictionary<string, IBuffer<int>> Buffers;

		/// <summary>
		/// The indices.
		/// </summary>
		public GLBuffer<uint> Indices;

		/// <summary>
		/// The array buffer.
		/// </summary>
		int abo = -1;

		/// <summary>
		/// Initializes a new instance of the <see cref="nginz.Geometry"/> class.
		/// </summary>
		public Geometry () {
			Buffers = new Dictionary<string, IBuffer<int>> ();
			abo = GL.GenVertexArray ();
		}

		/// <summary>
		/// Add a buffer to the geometry object.
		/// </summary>
		/// <returns>The buffer.</returns>
		/// <param name="name">Name.</param>
		/// <param name="buffer">Buffer.</param>
		public Geometry AddBuffer (string name, IBuffer<int> buffer) {
			Buffers[name] = buffer;
			return this;
		}

		/// <summary>
		/// Point the buffers to the specified attribute.
		/// </summary>
		/// <param name="program">Program.</param>
		/// <param name="name">Name.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public Geometry Attribute<T> (ShaderProgram program, string name) where T : struct{
			((GLBuffer<T>) Buffers[name]).PointTo (program.Attrib (name));
			return this;
		}

		/// <summary>
		/// Set the indices.
		/// </summary>
		/// <returns>The indices.</returns>
		/// <param name="indices">Indices.</param>
		public Geometry SetIndices (GLBuffer<uint> indices) {
			Indices = indices;
			return this;
		}

		/// <summary>
		/// Bind the vertices and indices.
		/// </summary>
		/// <param name="this">This.</param>
		public static void Bind (Geometry @this) {
			GL.BindVertexArray (@this.abo);
			@this.Indices.Bind ();
			@this.Buffers.ToList ().ForEach (kvp => kvp.Value.Bind ());
		}

		/// <summary>
		/// Bind the vertices and indices.
		/// </summary>
		public void Bind () {
			Bind (this);
		}

		/// <summary>
		/// Unbind the vertices and indices.
		/// </summary>
		/// <param name="this">This.</param>
		public static void Unbind (Geometry @this) {
			GL.BindVertexArray (0);
			@this.Indices.Unbind ();
			@this.Buffers.ToList ().ForEach (kvp => kvp.Value.Unbind ());
		}

		/// <summary>
		/// Unbind the vertices and indices.
		/// </summary>
		public void Unbind () {
			Unbind (this);
		}


		/// <summary>
		/// Construct the geometry object.
		/// </summary>
		/// <param name="program">Program.</param>
		public Geometry Construct (ShaderProgram program) {
			Bind ();
			Buffers.ToList ().ForEach (kvp => kvp.Value.PointTo (program.Attrib (kvp.Key)));
			Unbind ();
			return this;
		}

		/// <summary>
		/// Draw the geometry object.
		/// </summary>
		/// <param name="mode">Mode.</param>
		/// <param name="offset">Offset.</param>
		public void Draw (BeginMode mode, int offset = 0) {
			Bind ();
			GL.DrawElements (mode, Indices.Buffer.Length, DrawElementsType.UnsignedInt, offset);
			Unbind ();
		}
	}
}
