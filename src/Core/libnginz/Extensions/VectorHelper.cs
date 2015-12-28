using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;

namespace nginz {
	public static class VectorHelper {
		public static float Max (this Vector3 @this) {
			return Math.Max (@this.X, @this.Yz.Max ());
		}

		public static float Max (this Vector2 @this) {
			return Math.Max (@this.X, @this.Y);
		}
	}
}
