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

		string text;
		public string Text {
			get { return text; }
			set {
				text = value;
				RecreateFont ();
			}
		}

		string fontFamily;
		public string FontFamily {
			get { return fontFamily; }
			set {
				fontFamily = value;
				RecreateFont ();
			}
		}

		float fontSize;
		public float FontSize {
			get { return fontSize; }
			set {
				fontSize = value;
				RecreateFont ();
			}
		}

		Color4 foregroundColor;
		public Color4 ForegroundColor {
			get { return foregroundColor; }
			set {
				foregroundColor = value;
				RecreateFont ();
			}
		}

		public Texture2D BackgroundTexture { get; set; }

		bool mouseDown;
		bool highlighted;

		public Button (int width, int height) : base (width, height) {
			fontFamily = "Arial";
			fontSize = 14.25f;
			foregroundColor = Color4.Black;
			UseTexture = true;
			RecreateFont ();
			Click += delegate { };
		}

		public static Button Create (int width, int height) {
			return new Button (width, height);
		}

		public new Button SetPosition (int x, int y) {
			base.SetPosition (x, y);
			RecreateFont ();
			return this;
		}

		public new Button SetPosition (float x, float y) {
			base.SetPosition (x, y);
			RecreateFont ();
			return this;
		}

		public Button SetFont (string fontFamily, float emSize, Color4 color) {
			this.fontFamily = fontFamily;
			this.fontSize = emSize;
			this.foregroundColor = color;
			RecreateFont ();
			return this;
		}

		public override void Update (GameTime time) {
			var mouse = UIController.Instance.Game.Mouse;
			var mouseRect = new Rectangle ((int) mouse.X, (int) mouse.Y, 1, 1);
			if (mouseDown && mouse.IsButtonUp (MouseButton.Left))
				mouseDown = false;
			if (Bounds.IntersectsWith (mouseRect)) {
				highlighted = true;
				if (!mouseDown && mouse.IsButtonDown (MouseButton.Left)) {
					Click (this, EventArgs.Empty);
					mouseDown = true;
				}
			} else
				highlighted = false;
			base.Update (time);
		}

		public override void Draw (GameTime time, SpriteBatch batch) {
			if (UseTexture && BackgroundTexture != null) {
				batch.Draw (BackgroundTexture, BackgroundTexture.Bounds, Bounds, Color4.White);
			}
			Font.Draw (batch);
			base.Draw (time, batch);
		}

		void RecreateFont () {
			var res = new Resolution { Width = Width, Height = Height };
			Font = new Fontmap (res, fontFamily, fontSize);
			Font.SetColor (foregroundColor);
			Font.SetPosition (Position);
			Font.SetText (text);
		}
	}
}

