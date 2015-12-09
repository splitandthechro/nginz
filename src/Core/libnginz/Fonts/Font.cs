using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using nginz.Common;
using nginz.Interop.FreeType;
using OpenTK;
using OpenTK.Graphics;

namespace nginz
{
	/// <summary>
	/// Font.
	/// </summary>
	public class Font : IAsset
	{
		// Size of font textures in pixels
		const int TEXTURE_SIZE = 512;

		// Kerning
		const bool kerning = false;

		/// <summary>
		/// Global FT_Library instance.
		/// </summary>
		static IntPtr freetype;

		/// <summary>
		/// Initializes the <see cref="nginz.Font"/> class.
		/// </summary>
		static Font () {

			// Load FreeType
			if (!FT.Loaded)
				FT.Load ();

			// Check if there were any errors
			var err = FT.FT_Init_FreeType (out freetype);
			if (err != 0)
				throw new Exception ("Failed to initialise freetype");
		}

		static Texture2D GetNewTexture () {
			var texconfig = new TextureConfiguration {
				Interpolation = InterpolationMode.Linear,
				Mipmap = false
			};
			return new Texture2D (texconfig, TEXTURE_SIZE, TEXTURE_SIZE);
		}

		// Font info
		IntPtr facePtr;
		IntPtr size26d6;
		int lineHeight;

		// Glyph storage
		readonly Dictionary<uint, GlyphInfo> glyphs = new Dictionary<uint, GlyphInfo> ();
		List<Texture2D> pages = new List<Texture2D> ();
		int currentX = 0;
		int currentY = 0;
		int lineMax = 0;

		/// <summary>
		/// Gets the height of the line.
		/// </summary>
		/// <value>The height of the line.</value>
		public int LineHeight { get { return lineHeight; } }

		/// <summary>
		/// Initializes a Font from the specified TTF file
		/// </summary>
		/// <param name="filename">The TTF to load.</param>
		/// <param name="size">The size of the font in pt.</param>
		public Font (string filename, float size) {

			// Check if the file exists
			if (!File.Exists (filename))
				throw new FileNotFoundException ("Font not found: {0}", filename);

			// Get the size
			size26d6 = FTMath.To26Dot6 (size);

			// Get the font face and check for errors
			var err = FT.FT_New_Face (freetype, filename, 0, out facePtr);
			if (err != 0)
				throw new Exception ("Freetype Error");

			// Set the char size
			FT.FT_Set_Char_Size (facePtr,
				FTMath.To26Dot6 (0),
				size26d6,
				0,
				96
			);

			// Calculate metrics
			var faceRec = (FT.FaceRec) Marshal.PtrToStructure (facePtr, typeof(FT.FaceRec));
			var szRec = (FT.SizeRec) Marshal.PtrToStructure (faceRec.size, typeof(FT.SizeRec));
			lineHeight = (int) FTMath.From26Dot6 (szRec.metrics.height);
			pages.Add (GetNewTexture ());
		}

		/// <summary>
		/// Measure a string.
		/// </summary>
		/// <returns>The measurements.</returns>
		/// <param name="text">Text.</param>
		public Point MeasureString (string text) {

			// Skip empty strings
			if (text == "")
				return Point.Empty;

			// Get the codepoint iterator
			var iter = new CodepointIterator (text);

			// Initialize coordinates
			float penX = 0, penY = 0;

			// Iterate using the codepoint iterator
			while (iter.Iterate ()) {

				// Get the current codepoint
				var c = iter.Codepoint;

				// Increment the y coords if the char is a linefeed
				if (c == (uint) '\n') {
					penY += lineHeight;
					penX = 0;
					continue;
				}

				// Get the glyph
				var glyph = GetGlyph (c);

				if (glyph.Render) {
					penX += glyph.HorizontalAdvance;
					penY += glyph.AdvanceY;
				} else {
					penX += glyph.AdvanceX;
					penY += glyph.AdvanceY;
				}

				if (kerning && iter.Index < (iter.Count - 1)) {
					var g2 = GetGlyph (iter.PeekNext ());
					FT.FT_Vector vec;
					FT.FT_Get_Kerning (facePtr, glyph.CharIndex, g2.CharIndex, 2, out vec);
					var krn = FTMath.From26Dot6 (vec.x);
					penX += krn;
				}
			}

			penY = Math.Max (penY, LineHeight);
			return new Point ((int) penX, (int) penY);
		}

		/// <summary>
		/// Draw a string.
		/// </summary>
		/// <param name="spriteBatch">Sprite batch.</param>
		/// <param name="text">Text.</param>
		/// <param name="position">Position.</param>
		/// <param name="color">Color.</param>
		public void DrawString (SpriteBatch spriteBatch, string text, Vector2 position, Color4 color) {
			DrawString (spriteBatch, text, (int) position.X, (int) position.Y, color);	
		}

		/// <summary>
		/// Draw a string.
		/// </summary>
		/// <param name="spriteBatch">Sprite batch.</param>
		/// <param name="text">Text.</param>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		/// <param name="color">Color.</param>
		public void DrawString (SpriteBatch spriteBatch, string text, int x, int y, Color4 color) {
			if (text == "") //Skip empty strings
				return;
			var iter = new CodepointIterator (text);
			float penX = x, penY = y;
			while (iter.Iterate ()) {
				uint c = iter.Codepoint;
				if (c == (uint) '\n') {
					penY += lineHeight;
					penX = x;
					continue;
				}
				var glyph = GetGlyph (c);
				if (glyph.Render) {
					spriteBatch.Draw (
						glyph.Texture,
						glyph.Rectangle,
						new Rectangle (
							(int) penX + glyph.XOffset,
							(int) penY + (LineHeight - glyph.YOffset),
							glyph.Rectangle.Width,
							glyph.Rectangle.Height
						),
						color
					);
					penX += glyph.HorizontalAdvance;
					penY += glyph.AdvanceY;
				} else {
					penX += glyph.AdvanceX;
					penY += glyph.AdvanceY;
				}
				if (iter.Index < iter.Count - 1) {
					var g2 = GetGlyph (iter.PeekNext ());
					FT.FT_Vector vec;
					FT.FT_Get_Kerning (facePtr, glyph.CharIndex, g2.CharIndex, 2, out vec);
					var krn = FTMath.From26Dot6 (vec.x);
					penX += krn;
				}
			}
		}

		/// <summary>
		/// Add a character.
		/// </summary>
		/// <param name="codePoint">Codepoint.</param>
		unsafe void AddCharacter (uint codePoint) {
			
			// Handle Tab
			if (codePoint == (uint) '\t') {
				var spaceGlyph = GetGlyph ((uint) ' ');
				glyphs.Add (codePoint, new GlyphInfo (spaceGlyph.AdvanceX * 5, spaceGlyph.AdvanceY, spaceGlyph.CharIndex));
				return;
			}

			// Check if Glyph exists
			uint index = FT.FT_Get_Char_Index (facePtr, codePoint);
			if (index == 0) {
				if (codePoint == (uint) '?')
					throw new Exception ("Font does not contain required character '?'");
				glyphs.Add (codePoint, GetGlyph ((uint) '?'));
				return;
			}

			// Render the glyph
			FT.FT_Load_Glyph (facePtr, index, FT.FT_LOAD_DEFAULT | FT.FT_LOAD_TARGET_NORMAL);
			var faceRec = (FT.FaceRec) Marshal.PtrToStructure (facePtr, typeof(FT.FaceRec));
			FT.FT_Render_Glyph (faceRec.glyph, FT.FT_RENDER_MODE_NORMAL);
			var glyphRec = (FT.GlyphSlotRec) Marshal.PtrToStructure (faceRec.glyph, typeof(FT.GlyphSlotRec));

			//Check for glyph that is only spacing
			if (glyphRec.bitmap.width == 0 || glyphRec.bitmap.rows == 0) {
				glyphs.Add (codePoint, new GlyphInfo (
					(int) Math.Ceiling (FTMath.From26Dot6 (glyphRec.advance.x)),
					(int) Math.Ceiling (FTMath.From26Dot6 (glyphRec.advance.y)),
					index
				));
				return;
			}

			var pixels = new RGBA[glyphRec.bitmap.width * glyphRec.bitmap.rows];
			if (glyphRec.bitmap.pixel_mode == 2) {
				byte* data = (byte*) glyphRec.bitmap.buffer;
				for (int i = 0; i < glyphRec.bitmap.width * glyphRec.bitmap.rows; i++) {
					// TODO: 4 bytes used for 1 byte of alpha data? investigate compression with GL_RED and shader.
					pixels [i] = new RGBA (255, 255, 255, data [i]);
				}
			} else {
				throw new NotImplementedException ();
			}

			if (currentX + glyphRec.bitmap.width > TEXTURE_SIZE) {
				currentX = 0;
				currentY += lineMax;
				lineMax = 0;
			}

			if (currentY + glyphRec.bitmap.rows > TEXTURE_SIZE) {
				currentX = 0;
				currentY = 0;
				lineMax = 0;
				pages.Add (GetNewTexture ());
			}

			lineMax = Math.Max (lineMax, glyphRec.bitmap.rows);
			var rect = new Rectangle (currentX, currentY, glyphRec.bitmap.width, glyphRec.bitmap.rows);
			var tex = pages [pages.Count - 1];
			tex.SetData (pixels, rect);
			currentX += glyphRec.bitmap.width;

			var glyphInfo = new GlyphInfo (
				                t: tex, 
				                r: rect, 
				                advX: (int) Math.Ceiling (FTMath.From26Dot6 (glyphRec.advance.x)),
				                advY: (int) Math.Ceiling (FTMath.From26Dot6 (glyphRec.advance.y)),
				                horzAdv: (int) Math.Ceiling (FTMath.From26Dot6 (glyphRec.metrics.horiAdvance)),
				                xOff: glyphRec.bitmap_left,
				                yOff: glyphRec.bitmap_top,
				                idx: index
			                );
			glyphs.Add (codePoint, glyphInfo);
		}

		GlyphInfo GetGlyph (uint cp) {
			if (!glyphs.ContainsKey (cp))
				AddCharacter (cp);
			return glyphs [cp];
		}
	}
}

