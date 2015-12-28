using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;

namespace nginz.Lighting {
	public struct PointLight {
		private const int Color_Depth = 256;

		public BaseLight @base;
		public Attenuation atten;
		public Vector3 position;
		public float range {
			get {
				float a = atten.exponent;
				float b = atten.linear;
				float c = atten.constant - Color_Depth * @base.Intensity * @base.Color.Max ();

				return (float) ((-b + Math.Sqrt (b * b - 4 * a * c)) / (2 * a));
			}
		}
	}
}
