using System;
using System.Collections.Generic;
using System.Linq;
using nginz.Common;
using nginz.Lighting;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace nginz {

	/// <summary>
	/// Geometry.
	/// </summary>
	public class Geometry : IBind, IAsset {

		/// <summary>
		/// The buffers.
		/// </summary>
		[CLSCompliant (false)]
		public Dictionary<string, IBuffer<int>> Buffers;

		/// <summary>
		/// The indices.
		/// </summary>
		[CLSCompliant (false)]
		public GLBuffer<uint> Indices = null;

		public Material Material = Material.DefaultMaterial;

		/// <summary>
		/// The array buffer.
		/// </summary>
		int abo = -1;

		BeginMode mode;

		/// <summary>
		/// Initializes a new instance of the <see cref="nginz.Geometry"/> class.
		/// </summary>
		public Geometry (BeginMode mode) {
			Buffers = new Dictionary<string, IBuffer<int>> ();
			abo = GL.GenVertexArray ();
			this.mode = mode;
		}

		/// <summary>
		/// Add a buffer to the geometry object.
		/// </summary>
		/// <returns>The buffer.</returns>
		/// <param name="name">Name.</param>
		/// <param name="buffer">Buffer.</param>
		[CLSCompliant (false)]
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
		public Geometry Attribute<T> (ShaderProgram program, string name) where T : struct {
			((GLBuffer<T>) Buffers[name]).PointTo (program.Attrib (name));
			return this;
		}

		/// <summary>
		/// Set the indices.
		/// </summary>
		/// <returns>The indices.</returns>
		/// <param name="indices">Indices.</param>
		[CLSCompliant (false)]
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
			if (@this.Indices != null)
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
			if (@this.Indices != null)
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
		/// Draw the geometry object.
		/// </summary>
		/// <param name="program">Shader Program.</param>
		/// <param name="Model">Model Matrix.</param>
		/// <param name="camera">Camera.</param>
		/// <param name="offset">Offset.</param>
		public void Draw (ShaderProgram program, Matrix4 Model, Camera camera, int offset = 0) {
			this.Draw (program, Model, camera.ViewProjectionMatrix, offset);
		}

		public void Draw (ShaderProgram program, Matrix4 Model, Matrix4 VP, int offset = 0) {
			Bind ();
			Buffers.ToList ().ForEach (kvp => kvp.Value.PointTo (program.Attrib (kvp.Key)));
			program["MVP"] = Model * VP;
			var NRM = Model.Inverted ();
			NRM.Transpose ();
			program["NRM"] = NRM;

			program.SetMaterial ("material", this.Material);

			Material.Diffuse.Bind (TextureUnit.Texture0);
			Material.Normal.Bind (TextureUnit.Texture1);

			if (Indices != null)
				GL.DrawElements (mode, Indices.Buffer.Count, DrawElementsType.UnsignedInt, offset);
			else
				GL.DrawArrays (mode == BeginMode.Triangles ? PrimitiveType.Triangles : PrimitiveType.Quads, 0, Buffers["v_pos"].BufferSize);
			Unbind ();
		}

		public void DrawRaw (ShaderProgram program, Matrix4 MVP, int offset = 0) {
			Bind ();
			Buffers.ToList ().ForEach (kvp => kvp.Value.PointTo (program.Attrib (kvp.Key)));

			program["MVP"] = MVP;

			if (Indices != null)
				GL.DrawElements (mode, Indices.Buffer.Count, DrawElementsType.UnsignedInt, offset);
			else
				GL.DrawArrays (mode == BeginMode.Triangles ? PrimitiveType.Triangles : PrimitiveType.Quads, 0, Buffers["v_pos"].BufferSize);

			Unbind ();
		}
	}
}
