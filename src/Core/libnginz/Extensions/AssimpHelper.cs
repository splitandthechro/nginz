using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assimp;
using OpenTK;

namespace nginz {
	public static class AssimpHelper {
		public static Vector3 ToVector3 (this Vector3D @this) {
			return new Vector3 (@this.X, @this.Y, @this.Z);
		}

		public static Vector2 ToVector2 (this Vector2D @this) {
			return new Vector2 (@this.X, @this.Y);
		}

		public static Vector2 ToVector2 (this Vector3 @this) {
			return new Vector2 (@this.X, @this.Y);
		}

		public static GLBuffer<Vector3> ToGLBuffer (this IList<Vector3> @this, GLBufferSettings? settings = null) {
			return new GLBuffer<Vector3> (settings ?? GLBufferSettings.StaticDraw3FloatArray, @this);
		}

		public static GLBuffer<Vector2> ToGLBuffer (this IList<Vector2> @this, GLBufferSettings? settings = null) {
			return new GLBuffer<Vector2> (settings ?? GLBufferSettings.StaticDraw2FloatArray, @this);
		}

		[CLSCompliant(false)]
		public static GLBuffer<uint> ToGLBuffer (this IList<uint> @this, GLBufferSettings? settings = null) {
			return new GLBuffer<uint> (settings ?? GLBufferSettings.StaticIndices, @this);
		}
	}
}
