using System;
using OpenTK.Graphics;
using System.Drawing;
using OpenTK.Graphics.ES30;
using OpenTK.Input;

namespace nginz
{
	public class Button : Control
	{
		public event EventHandler Click;

		public Fontmap Font { get; private set; }
		public bool UseTexture { get; set; }

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
				var baseColor = highlighted
					? HighlightForegroundColor
					: foregroundColor;
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

		public Color4 HighlightForegroundColor { get; set; }
		public Texture2D BackgroundTexture { get; set; }

		bool mouseDown;
		bool highlighted;
		bool updateFont;

		public Button (int width, int height) : base (width, height) {
			fontFamily = "Arial";
			fontSize = 14.25f;
			foregroundColor = Color4.Black;
			HighlightForegroundColor = Color4.White;
			UseTexture = true;
			Click += delegate { };
			var res = new Resolution (Bounds.Width, Bounds.Height);
			Font = new Fontmap (res, fontFamily, fontSize);
			Font.SetPosition (Position);
			Font.HorizontalAlignment = StringAlignment.Center;
			Font.VerticalAlignment = StringAlignment.Center;
			Font.Update ();
			transparency = 1f;
		}

		public static Button Create (int width, int height) {
			return new Button (width, height);
		}

		public new Button SetPosition (int x, int y) {
			return SetPosition ((float) x, (float) y);
		}

		public new Button SetPosition (float x, float y) {
			base.SetPosition (x, y);
			Font.SetPosition (Position);
			updateFont = true;
			return this;
		}

		protected override void OnPositionChanged () {
			if (Font != null)
				Font.SetPosition (Position);
			updateFont = true;
			base.OnPositionChanged ();
		}

		public Button SetFont (string fontFamily, float emSize, Color4 color) {
			this.fontFamily = fontFamily;
			this.fontSize = emSize;
			this.foregroundColor = color;
			updateFont = true;
			return this;
		}

		public override void Update (GameTime time) {
			var mouse = UIController.Instance.Game.Mouse;
			var mouseRect = new Rectangle ((int) mouse.X, (int) mouse.Y, 1, 1);
			if (mouseDown && mouse.IsButtonUp (MouseButton.Left))
				mouseDown = false;
			if (Bounds.IntersectsWith (mouseRect)) {
				highlighted = true;
				Font.SetColor (ColorWithTransparency);
				updateFont = true;
				if (!mouseDown && mouse.IsButtonDown (MouseButton.Left)) {
					Click (this, EventArgs.Empty);
					mouseDown = true;
				}
			} else {
				if (highlighted) {
					highlighted = false;
					Font.SetColor (ColorWithTransparency);
					updateFont = true;
				}
			}
			if (updateFont) {
				Font.Update ();
				updateFont = false;
			}
			base.Update (time);
		}

		public override void Draw (GameTime time, SpriteBatch batch) {
			if (UseTexture && BackgroundTexture != null)
				batch.Draw (BackgroundTexture, BackgroundTexture.Bounds, Bounds, new Color4 (1, 1, 1, Transparency));
			Font.Draw (batch);
			base.Draw (time, batch);
		}
	}
}

