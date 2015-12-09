using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nginz {
	public struct TextureConfiguration {
		public bool Mipmap;
		public InterpolationMode Interpolation;

		public static TextureConfiguration Linear = new TextureConfiguration {
			Mipmap = false,
			Interpolation = InterpolationMode.Linear
		};
		public static TextureConfiguration Nearest = new TextureConfiguration {
			Mipmap = false,
			Interpolation = InterpolationMode.Nearest
		};
		public static TextureConfiguration LinearMipmap = new TextureConfiguration {
			Mipmap = true,
			Interpolation = InterpolationMode.Linear
		};
		public static TextureConfiguration NearestMipmap = new TextureConfiguration {
			Mipmap = true,
			Interpolation = InterpolationMode.Nearest
		};
	}
}
