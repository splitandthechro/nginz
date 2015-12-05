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
	public class Fontmap : IDisposable
	{

		public StringAlignment HorizontalAlignment;
		public StringAlignment VerticalAlignment;

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
		/// The text.
		/// </summary>
		string Text;

		/// <summary>
		/// Initializes a new instance of the <see cref="nginz.Fontmap"/> class.
		/// </summary>
		/// <param name = "res"></param>
		/// <param name="fontFamily">Font family.</param>
		/// <param name="emSize">Font size in em.</param>
		/// <param name="fontStyle">Font style.</param>
		public Fontmap (Resolution res, string fontFamily, float emSize, FontStyle fontStyle = 0x0) {

			HorizontalAlignment = StringAlignment.Near;
			VerticalAlignment = StringAlignment.Near;
			SetPosition (Vector2.Zero, false);
			SetFont (fontFamily, emSize, fontStyle, false);
			SetText (string.Empty, false);

			// Create a transparent bitmap with the desired resolution
			Bitmap = new Bitmap (res.Width, res.Height, GDIPixelFormat.Format32bppArgb);

			// Create a texture from the bitmap
			Texture = new Texture2D (Bitmap, TextureConfiguration.Nearest, true);

			// Update the texture
			Update ();
		}

		public Fontmap SetFont (string fontFamily, float emSize, FontStyle fontStyle = 0x0, bool update = false) {

			// Dispose the old font
			if (Font != null)
				Font.Dispose ();

			// Set the new font
			Font = new Font (fontFamily, emSize, fontStyle, GraphicsUnit.Pixel);

			// Update the texture
			if (update) Update ();

			// Return this instance
			return this;
		}

		public void Update () {
			
			// Get the position as System.Drawing.PointF
			var bounds = new RectangleF (0, 0, Bitmap.Width, Bitmap.Height);

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

				// Set the string format
				var format = new StringFormat ();
				format.Alignment = HorizontalAlignment;
				format.LineAlignment = VerticalAlignment;

				// Draw the specified string onto the bitmap
				graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
				graphics.DrawString (Text, Font, new SolidBrush (color), bounds, format);
			}

			// Update the texture
			Texture.Update (Bitmap, true);
		}
			

		/// <summary>
		/// Set or update the text.
		/// </summary>
		/// <param name="text">Text.</param>
		public Fontmap SetText (string text, bool update = false) {

			// Update text
			Text = text;

			// Update texture
			if (update) Update ();

			// Return this instance
			return this;
		}

		public Fontmap SetText (string format, params object[] args) {

			// Set the text
			return SetText (string.Format (format, args));
		}

		public Fontmap SetPosition (int x, int y, bool update = false) {

			// Set the position
			Position = new Vector2 (x, y);

			// Update texture
			if (update) Update ();

			// Return this instance
			return this;
		}

		public Fontmap SetPosition (Vector2 pos, bool update = false) {

			// Set the position
			Position = pos;

			// Update texture
			if (update) Update ();

			// Return this instance
			return this;
		}

		public Fontmap SetColor (int r, int g, int b, int a = 255, bool update = false) {

			// Set the color
			Color = new GLColor (r / 255f, g / 255f, b / 255f, a / 255f);

			// Update texture
			if (update) Update ();

			// Return this instance
			return this;
		}

		public Fontmap SetColor (float r, float g, float b, float a = 1f, bool update = false) {

			// Set the color
			Color = new GLColor (r, g, b, a);

			// Update texture
			if (update) Update ();

			// Return this instance
			return this;
		}

		public Fontmap SetColor (GLColor color, bool update = false) {

			// Set the color
			Color = color;

			// Update texture
			if (update) Update ();

			// Return this instance
			return this;
		}

		public Fontmap SetColor (GDIColor color, bool update = false) {

			// Set the color
			return SetColor (color.R, color.G, color.B, color.A, false);
		}

		public void Draw (SpriteBatch batch) {
			if (Texture != null)
				batch.Draw (Texture, Position, GLColor.White);
		}

		#region IDisposable implementation

		/// <summary>
		/// Releases all resource used by the <see cref="nginz.Fontmap"/> object.
		/// </summary>
		/// <remarks>Call <see cref="Dispose"/> when you are finished using the <see cref="nginz.Fontmap"/>. The <see cref="Dispose"/>
		/// method leaves the <see cref="nginz.Fontmap"/> in an unusable state. After calling <see cref="Dispose"/>, you must
		/// release all references to the <see cref="nginz.Fontmap"/> so the garbage collector can reclaim the memory that the
		/// <see cref="nginz.Fontmap"/> was occupying.</remarks>
		public void Dispose () {
			Bitmap.Dispose ();
			Font.Dispose ();
		}

		#endregion
	}
}

