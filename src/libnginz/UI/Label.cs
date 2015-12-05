using System;
using OpenTK.Graphics;

namespace nginz
{
	public class Label : Control
	{
		public Fontmap Font { get; private set; }

		float transparency;
		public float Transparency {
			get { return transparency; }
			set {
				transparency = value;
				Font.SetColor (ColorWithTransparency);
				updateFont = true;
			}
		}

		public Color4 ColorWithTransparency {
			get {
				var baseColor = foregroundColor;
				var color = new Color4 (
					r: baseColor.R,
					g: baseColor.G,
					b: baseColor.B,
					a: transparency
				);
				return color;
			}
		}

		string text;
		public string Text {
			get { return text; }
			set {
				text = value;
				Font.SetText (value);
				updateFont = true;
			}
		}

		string fontFamily;
		public string FontFamily {
			get { return fontFamily; }
			set {
				fontFamily = value;
				Font.SetFont (value, fontSize);
				updateFont = true;
			}
		}

		float fontSize;
		public float FontSize {
			get { return fontSize; }
			set {
				fontSize = value;
				Font.SetFont (fontFamily, value);
				updateFont = true;
			}
		}

		Color4 foregroundColor;
		public Color4 ForegroundColor {
			get { return foregroundColor; }
			set {
				foregroundColor = value;
				Font.SetColor (value);
				updateFont = true;
			}
		}

		bool updateFont;

		public Label (int width, int height) : base (width, height) {
			transparency = 1f;
			fontFamily = "Arial";
			fontSize = 14.25f;
			foregroundColor = Color4.Black;
			var res = new Resolution (Bounds.Width, Bounds.Height);
			Font = new Fontmap (res, fontFamily, fontSize);
			Font.SetPosition (Position);
			Font.SetColor (ColorWithTransparency);
			Font.Update ();
		}

		public override void Update (GameTime time) {
			if (updateFont) {
				Font.Update ();
				updateFont = false;
			}
			base.Update (time);
		}

		public override void Draw (GameTime time, SpriteBatch batch) {
			Font.Draw (batch);
			base.Draw (time, batch);
		}

		protected override void OnPositionChanged () {
			if (Font != null)
				Font.SetPosition (Position);
			updateFont = true;
			base.OnPositionChanged ();
		}
	}
}

