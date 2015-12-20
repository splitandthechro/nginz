using System;
using OpenTK.Graphics;
using System.Drawing;
using OpenTK.Input;
using OpenTK;

namespace nginz
{
	public class Button : Control
	{
		public event EventHandler Click;

		public Label Label;

		public bool UseTexture { get; set; }

		float transparency;
		public float Transparency {
			get { return transparency; }
			set {
			 	transparency = value;
				Label.Transparency = value;
			}
		}

		public string Text {
			get { return Label.Text; }
			set { Label.Text = value; }
		}

		public float FontSize {
			get { return Label.FontSize; }
			set { Label.FontSize = value; }
		}

		Color4 foregroundColor;
		public Color4 ForegroundColor {
			get { return foregroundColor; }
			set {
				foregroundColor = value;
				if (!highlighted)
					Label.ForegroundColor = value;
			}
		}
		public Color4 HighlightForegroundColor { get; set; }
		public Texture2D BackgroundTexture { get; set; }

		bool mouseDown;
		bool highlighted;

		public Button (int width, int height, Font font) : base (width, height) {
			Label = new Label (width, height, font);
			Label.CenterText = true;
			FontSize = 14.25f;
			ForegroundColor = Color4.White;
			HighlightForegroundColor = Color4.White;
			UseTexture = true;
			transparency = 1f;
			Controls.Add (Label);
			Click += delegate { };
		}

		public Button (int width, int height, string fontName)
			: this (width, height, UIController.Instance.Fonts [fontName]) {
		}

		public static Button Create (int width, int height, Font font) {
			return new Button (width, height, font);
		}

		public static Button Create (int width, int height, string fontName) {
			return new Button (width, height, fontName);
		}

		public Button SetFontProperties (float emSize, Color4 color) {
			FontSize = emSize;
			ForegroundColor = color;
			return this;
		}

		protected override void OnPositionChanged () {
			if (Label != null) {
				Label.Position = Position;
			}
			base.OnPositionChanged ();
		}

		public override void Update (GameTime time) {
			var mouse = UIController.Instance.Game.Mouse;
			var mouseRect = new Rectangle ((int) mouse.X, (int) mouse.Y, 1, 1);
			if (mouseDown && mouse.IsButtonUp (MouseButton.Left))
				mouseDown = false;
			if (Bounds.IntersectsWith (mouseRect)) {
				Label.ForegroundColor = HighlightForegroundColor;
				highlighted = true;
				if (!mouseDown && mouse.IsButtonDown (MouseButton.Left)) {
					Click (this, EventArgs.Empty);
					mouseDown = true;
				}
			} else {
				if (highlighted) {
					Label.ForegroundColor = ForegroundColor;
					highlighted = false;
				}
			}
			base.Update (time);
		}

		public override void Draw (GameTime time, SpriteBatch batch) {
			if (UseTexture && BackgroundTexture != null)
				batch.Draw (BackgroundTexture, BackgroundTexture.Bounds, Bounds, new Color4 (1, 1, 1, Transparency));
			base.Draw (time, batch);
		}
	}
}

