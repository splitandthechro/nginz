using System;
using System.Drawing;
using nginz;
using OpenTK;

namespace FlappyMascot
{
	class Tube {
		public Texture2D Texture;
		public Vector2 Position;
		public RectangleF Bounds {
			get {
				return new RectangleF (
					x: Position.X,
					y: Position.Y,
					width: Texture.Width,
					height: Texture.Height
				);
			}
		}
		public bool IsInView () {
			if (Position.X < (-Texture.Width) || Position.X > 640)
				return false;
			return true;
		}
	}
}

