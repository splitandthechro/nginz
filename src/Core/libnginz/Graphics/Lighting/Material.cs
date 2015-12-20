using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK.Graphics;

namespace nginz.Lighting {
	public struct Material {
		public Color4 Color;
		public Texture2D Texture;
		public float SpecularIntensity;
		public float SpecularPower;

		public static Material DefaultMaterial = new Material (null, null);

		public Material (Color4? color = null, Texture2D texture = null, float specularIntensity = 1f, float specularPower = 2f) {
			this.Color = color ?? Color4.Gray;
			this.Texture = texture ?? Texture2D.Dot;
			this.SpecularIntensity = specularIntensity;
			this.SpecularPower = specularPower;
		}
	}
}
