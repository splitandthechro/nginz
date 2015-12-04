using System;
using System.Collections.Generic;
using OpenTK;
using System.Drawing;

namespace nginz
{
	public class Control : IUpdatable, IDrawable2D
	{
		public List<Control> Controls;

		public int X {
			get { return (int) Position.X; }
			set { Position = new Vector2 ((float) value, Position.Y); }
		}

		public int Y {
			get { return (int) Position.Y; }
			set { Position = new Vector2 (Position.X, (float) value); }
		}

		public int Width { get; set; }
		public int Height { get; set; }

		public float WidthF { get { return (float) Width; } }
		public float HeightF { get { return (float) Height; } }

		public Vector2 Position { get; set; }
		public Rectangle Bounds {
			get {
				return new Rectangle (
					x: (int) Position.X,
					y: (int) Position.Y,
					width: Width,
					height: Height
				);
			}
		}

		public Control (int width, int height) {
			Controls = new List<Control> ();
			Width = width;
			Height = height;
			Position = new Vector2 (0, 0);
		}

		public virtual void SetPosition (float x, float y) {
			Position = new Vector2 (x, y);
		}

		public virtual void SetPosition (int x, int y) {
			SetPosition ((float) x, (float) y);
		}

		#region IUpdatable implementation

		public virtual void Update (GameTime time) {
			for (var i = 0; i < Controls.Count; i++)
				Controls [i].Update (time);
		}

		#endregion

		#region IDrawable2D implementation

		public virtual void Draw (GameTime time, SpriteBatch batch) {
			for (var i = 0; i < Controls.Count; i++)
				Controls [i].Draw (time, batch);
		}

		#endregion
	}
}

