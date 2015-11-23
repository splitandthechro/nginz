using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;

namespace nginz {
	public class Model {
		public Geometry Geometry;
		public Vector3 Position;
		public Vector3 Rotation;
		public Vector3 Scale;

		public Matrix4 Matrix {
			get {
				return Matrix4.CreateScale (Scale)
					 * Matrix4.CreateRotationX (Rotation.X)
					 * Matrix4.CreateRotationX (Rotation.Y)
					 * Matrix4.CreateRotationX (Rotation.Z)
					 * Matrix4.CreateTranslation (Position);
            }
		}

		public Model (Geometry geometry) {
			Geometry = geometry;
			Position = Vector3.Zero;
			Scale = Vector3.One;
			Rotation = Vector3.Zero;
		}

		public void Draw (ShaderProgram program, Camera camera) {
			Geometry.Draw (program, this.Matrix, camera);
		}
	}
}
