using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;

namespace nginz.Lighting {
	public struct DirectionalLight {
		public BaseLight @base;
		public Vector3 direction;
	}
}
