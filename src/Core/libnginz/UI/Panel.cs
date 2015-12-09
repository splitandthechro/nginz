using System;
using System.Drawing;
using OpenTK.Graphics;

namespace nginz
{
	public class Panel : Control
	{
		public Texture2D Background { get; set; }

		Rectangle source;
		Rectangle dest;

		public Panel (int width, int height) : base (width, height) {
		}

		public Panel SetBackground (Texture2D tex) {
			Background = tex;
			return this;
		}

		public new Panel SetPosition (int x, int y) {
			base.SetPosition (x, y);
			return this;
		}

		public new Panel SetPosition (float x, float y) {
			base.SetPosition (x, y);
			return this;
		}

		public override void Update (GameTime time) {
			source = new Rectangle ((int) Position.X, (int) Position.Y, Background.Width, Background.Height);
			dest = new Rectangle ((int) Position.X, (int) Position.Y, Width, Height);
			base.Update (time);
		}

		public override void Draw (GameTime time, SpriteBatch batch) {
			if (Background != null)
				batch.Draw (Background, source, dest, Color4.White);
			base.Draw (time, batch);
		}
	}
}

