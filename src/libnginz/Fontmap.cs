using System;
using System.Drawing;
using System.Drawing.Text;
using OpenTK;
using GDIColor = System.Drawing.Color;
using GDIPixelFormat = System.Drawing.Imaging.PixelFormat;
using GLColor = OpenTK.Graphics.Color4;
using GLPixelFormat = OpenTK.Graphics.OpenGL4.PixelFormat;

namespace nginz
{

	/// <summary>
	/// Fontmap.
	/// </summary>
	public class Fontmap
	{

		/// <summary>
		/// The texture.
		/// </summary>
		readonly Texture2D Texture;

		/// <summary>
		/// The bitmap.
		/// </summary>
		readonly Bitmap Bitmap;

		/// <summary>
		/// The font.
		/// </summary>
		Font Font;

		/// <summary>
		/// The position.
		/// </summary>
		Vector2 Position;

		/// <summary>
		/// The color.
		/// </summary>
		GLColor Color;

		/// <summary>
		/// Initializes a new instance of the <see cref="nginz.Fontmap"/> class.
		/// </summary>
		/// <param name = "res"></param>
		/// <param name="fontFamily">Font family.</param>
		/// <param name="emSize">Font size in em.</param>
		/// <param name="fontStyle">Font style.</param>
		public Fontmap (Resolution res, string fontFamily, float emSize, FontStyle fontStyle = 0x0) {

			SetPosition (Vector2.Zero);
			SetFont (fontFamily, emSize, fontStyle);

			// Create a transparent bitmap with the desired resolution
			Bitmap = new Bitmap (res.Width, res.Height, GDIPixelFormat.Format32bppArgb);

			// Create a texture from the bitmap
			Texture = new Texture2D (Bitmap, TextureConfiguration.Nearest, true);
		}

		public Fontmap SetFont (string fontFamily, float emSize, FontStyle fontStyle = 0x0) {

			// Dispose the old font
			if (Font != null)
				Font.Dispose ();

			// Set the new font
			Font = new Font (fontFamily, emSize, fontStyle, GraphicsUnit.Pixel);

			// Return this instance
			return this;
		}
			

		/// <summary>
		/// Set or update the text.
		/// </summary>
		/// <param name="text">Text.</param>
		public Fontmap SetText (string text) {

			// Get the position as System.Drawing.PointF
			var pos = new PointF (Position.X, Position.Y);

			// Get the color as System.Drawing.Color
			var color = GDIColor.FromArgb (
				red: MathHelper.Clamp ((int) (Color.R * 255), 0, 255),
				green: MathHelper.Clamp ((int) (Color.G * 255), 0, 255),
				blue: MathHelper.Clamp ((int) (Color.B * 255), 0, 255),
				alpha: MathHelper.Clamp ((int) (Color.A * 255), 0, 255)
			);

			// Use a new graphics object representing the bitmap
			using (var graphics = Graphics.FromImage (Bitmap)) {

				// Clear the bitmap with transparent pixels
				graphics.Clear (GDIColor.Transparent);

				// Draw the specified string onto the bitmap
				graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
				graphics.DrawString (text, Font, new SolidBrush (color), pos);
			}

			// Update the texture
			Texture.Update (Bitmap, true);

			// Return this instance
			return this;
		}

		public Fontmap SetText (string format, params object[] args) {

			// Set the text
			return SetText (string.Format (format, args));
		}

		public Fontmap SetPosition (int x, int y) {

			// Set the position
			Position = new Vector2 (x, y);

			// Return this instance
			return this;
		}

		public Fontmap SetPosition (Vector2 pos) {

			// Set the position
			Position = new Vector2 (pos.X, pos.Y);

			// Return this instance
			return this;
		}

		public Fontmap SetColor (int r, int g, int b, int a = 255) {

			// Set the color
			Color = new GLColor (r / 255f, g / 255f, b / 255f, a / 255f);

			// Return this instance
			return this;
		}

		public Fontmap SetColor (float r, float g, float b, float a = 1f) {

			// Set the color
			Color = new GLColor (r, g, b, a);

			// Return this instance
			return this;
		}

		public Fontmap SetColor (GLColor color) {

			// Set the color
			Color = color;

			// Return this instance
			return this;
		}

		public Fontmap SetColor (GDIColor color) {

			// Set the color
			return SetColor (color.R, color.G, color.B, color.A);
		}

		public void Draw (SpriteBatch batch) {
			if (Texture != null)
				batch.Draw (Texture, Vector2.Zero, GLColor.White);
		}
	}
}

