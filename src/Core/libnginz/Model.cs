using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace nginz
{

	/// <summary>
	/// Model.
	/// </summary>
	public class Model {

		/// <summary>
		/// The geometry.
		/// </summary>
		public Geometry Geometry;

		/// <summary>
		/// The position.
		/// </summary>
		public Vector3 Position;

		/// <summary>
		/// The rotation.
		/// </summary>
		public Vector3 Rotation;

		/// <summary>
		/// The scale.
		/// </summary>
		public Vector3 Scale;

		/// <summary>
		/// Gets the matrix.
		/// </summary>
		/// <value>The matrix.</value>
		public Matrix4 Matrix {
			get {

				// Create scale matrix
				var scale = Matrix4.CreateScale (Scale);

				// Create rotation matrix for x axis
				var rotx = Matrix4.CreateRotationX (Rotation.X);

				// Create rotation matrix for y axis
				var roty = Matrix4.CreateRotationY (Rotation.Y);

				// Create rotation matrix for z axis
				var rotz = Matrix4.CreateRotationZ (Rotation.Z);

				// Create translation matrix
				var translation = Matrix4.CreateTranslation (Position);

				// Multiply all matrices together
				return scale * rotx * roty * rotz * translation;
            }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="nginz.Model"/> class.
		/// </summary>
		/// <param name="geometry">Geometry.</param>
		public Model (Geometry geometry) {
			Geometry = geometry;
			Position = Vector3.Zero;
			Scale = Vector3.One;
			Rotation = Vector3.Zero;
		}

		public Model (ObjFile objModel, int groupNum, ShaderProgram program) {
			var group = objModel.Groups[groupNum];
			var tempPos = new List<Vector3> ();
			var tempTex = new List<Vector2> ();
			foreach (ObjFace f in group.Faces) {
				foreach (ObjFaceVertex vert in f.Vertices) {
					tempPos.Add (objModel.Vertices[vert.VertexIndex - 1]);
					tempTex.Add (objModel.Textures[vert.TextureIndex - 1]);
                }
			}
			var v_pos = new GLBuffer<Vector3> (GLBufferSettings.StaticDraw3FloatArray, tempPos);
			var v_tex = new GLBuffer<Vector2> (GLBufferSettings.StaticDraw2FloatArray, tempTex);
			var m_ind = new GLBuffer<uint> (GLBufferSettings.StaticIndices, Array.ConvertAll<int, uint> (Enumerable.Range (0, tempPos.Count).ToArray (), x => (uint) x));
			Geometry = new Geometry (BeginMode.Quads)
				.AddBuffer ("v_pos", v_pos)
				.AddBuffer ("v_tex", v_tex)
				.Construct (program);

			Position = Vector3.Zero;
			Scale = Vector3.One;
			Rotation = Vector3.Zero;
		}

		/// <summary>
		/// Draw the specified program and camera.
		/// </summary>
		/// <param name="program">Program.</param>
		/// <param name="camera">Camera.</param>
		public void Draw (ShaderProgram program, Camera camera) {

			// Draw the geometry
			Geometry.Draw (program, Matrix, camera);
		}
	}
}
