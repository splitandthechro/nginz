using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK.Graphics;

namespace nginz.Lighting {
	public struct Material {
		public Color4 Color;
		public Texture2D Diffuse;
		public Texture2D Normal;
		public float SpecularIntensity;
		public float SpecularPower;

		public static Material DefaultMaterial = new Material (null, null);

		public Material (Color4? color = null, Texture2D diffuse = null, Texture2D normal = null, float specularIntensity = 1f, float specularPower = 2f) {
			this.Color = color ?? Color4.Gray;
			this.Diffuse = diffuse ?? Texture2D.Dot;
			this.Normal = normal ?? Texture2D.Dot;
			this.SpecularIntensity = specularIntensity;
			this.SpecularPower = specularPower;
		}
	}
}
