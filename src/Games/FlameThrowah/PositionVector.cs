using System;
using Newtonsoft.Json;
using OpenTK;

namespace FlameThrowah
{
	public class PositionVector
	{
		[JsonIgnore]
		public Vector2 Vector;

		public float X {
			get { return Vector.X; }
			set { Vector.X = value; }
		}

		public float Y {
			get { return Vector.Y; }
			set { Vector.Y = value; }
		}

		public PositionVector () {
			Vector = new Vector2 ();
		}

		public PositionVector (float x, float y) {
			Vector = new Vector2 (x, y);
		}

		public PositionVector (Vector2 vec) {
			Vector = vec;
		}
	}
}

