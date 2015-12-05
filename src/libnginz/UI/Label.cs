using System;
using OpenTK;
using OpenTK.Graphics;

namespace nginz
{
	public class Label : Control
	{
		public Font Font { get; private set; }

		public Color4 ColorWithTransparency {
			get {
				var baseColor = ForegroundColor;
				var color = new Color4 (
					r: baseColor.R,
					g: baseColor.G,
					b: baseColor.B,
					a: Transparency
				);
				return color;
			}
		}

		public float Transparency { get; set; }
		public string Text { get; set; }
		public float FontSize { get; set; }
		public Color4 ForegroundColor { get; set; }
		public bool CenterText { get; set; }

		Vector2 cachedFontPosition;

		public Label (int width, int height, Font font) : base (width, height) {
			Transparency = 1f;
			FontSize = 14.25f;
			ForegroundColor = Color4.Black;
			Font = font;
		}

		public Label (int width, int height, string fontName)
			: this (width, height, UIController.Instance.Fonts [fontName]) {
		}

		public override void Update (GameTime time) {
			var measurement = Font.MeasureString (Text);
			if (CenterText) {
				cachedFontPosition = new Vector2 (
					x: (float) X + ((WidthF / 2f) - ((float) measurement.X / 2f)),
					y: (float) Y + ((HeightF / 2f) - ((float) measurement.Y / 2f))
				);
			} else {
				cachedFontPosition = Position;
			}
			base.Update (time);
		}

		public override void Draw (GameTime time, SpriteBatch batch) {
			Font.DrawString (batch, Text, cachedFontPosition, ColorWithTransparency);
			base.Draw (time, batch);
		}
	}
}

