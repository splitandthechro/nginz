using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using nginz.Common;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using nginz.FtInterop;

namespace nginz
{
	public class FontTTF : IAsset
	{
		//Size of font textures (in pixels)
		const int TEXTURE_SIZE = 512;

		/// <summary>
		/// Global FT_Library instance.
		/// </summary>
		static IntPtr freetype;

		static FontTTF () {
			if (!FT.Loaded)
				FT.Load ();
			var err = FT.FT_Init_FreeType (out freetype);
			if (err != 0)
				throw new Exception ("Failed to initialise freetype");
		}

		class GlyphInfo
		{
			public uint CharIndex;
			public Rectangle Rectangle;
			public Texture2D Texture;
			public int AdvanceX;
			public int AdvanceY;
			public int HorizontalAdvance;
			public int XOffset;
			public int YOffset;
			public bool Render;

			public GlyphInfo (Texture2D t, Rectangle r, int advX, int advY, int horzAdv, int xOff, int yOff, uint idx) {
				Texture = t;
				Rectangle = r;
				AdvanceX = advX;
				AdvanceY = advY;
				HorizontalAdvance = horzAdv;
				XOffset = xOff;
				YOffset = yOff;
				Render = true;
				CharIndex = idx;
			}

			public GlyphInfo (int advX, int advY, uint index) {
				Render = false;
				AdvanceX = advX;
				AdvanceY = advY;
				CharIndex = index;
			}
		}
		//font info
		IntPtr facePtr;
		IntPtr size26d6;
		int lineHeight;
		bool kerning;
		//glyph storage
		List<Texture2D> pages = new List<Texture2D> ();
		Dictionary<uint, GlyphInfo> glyphs = new Dictionary<uint, GlyphInfo> ();
		int currentX = 0;
		int currentY = 0;
		int lineMax = 0;

		public int LineHeight {
			get {
				return lineHeight;
			}
		}

		/// <summary>
		/// Initializes a Font from the specified TTF file
		/// </summary>
		/// <param name="filename">The TTF to load.</param>
		/// <param name="size">The size of the font in pt.</param>
		public FontTTF (string filename, float size) {
			
			if (!File.Exists (filename))
				throw new FileNotFoundException (filename);
			size26d6 = FTMath.To26Dot6 (size);
			if (!File.Exists (filename))
				throw new FileNotFoundException ("Font not found " + filename, filename);

			var err = FT.FT_New_Face (freetype, filename, 0, out facePtr);
			if (err != 0)
				throw new Exception ("Freetype Error");

			FT.FT_Set_Char_Size (facePtr,
				FTMath.To26Dot6 (0),
				size26d6,
				0,
				96
			);
			//get metrics
			var faceRec = (FT.FaceRec) Marshal.PtrToStructure (facePtr, typeof (FT.FaceRec));
			var szRec = (FT.SizeRec) Marshal.PtrToStructure (faceRec.size, typeof (FT.SizeRec));
			lineHeight = (int) FTMath.From26Dot6 (szRec.metrics.height);
			pages.Add (GetNewTexture ());
		}

		public Point MeasureString (string text) {
			if (text == "") //Skip empty strings
				return Point.Empty;
			var iter = new CodepointIterator (text);
			float penX = 0, penY = 0;
			while (iter.Iterate ()) {
				var c = iter.Codepoint;
				if (c == (uint) '\n') {
					penY += lineHeight;
					penX = 0;
					continue;
				}
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
			return new Point ((int) penX, (int) penY);
		}

		public void DrawString (SpriteBatch spriteBatch, string text, Vector2 position, Color4 color) {
			DrawString (spriteBatch, text, (int) position.X, (int) position.Y, color);	
		}

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

		unsafe void AddCharacter (uint cp) {
			//Handle Tab
			if (cp == (uint) '\t') {
				var spaceGlyph = GetGlyph ((uint) ' ');
				glyphs.Add (cp, new GlyphInfo (spaceGlyph.AdvanceX * 5, spaceGlyph.AdvanceY, spaceGlyph.CharIndex));
				return;
			}
			uint index = FT.FT_Get_Char_Index (facePtr, cp);
			//Check if Glyph exists
			if (index == 0) {
				if (cp == (uint) '?')
					throw new Exception ("Font does not contain required character '?'");
				glyphs.Add (cp, GetGlyph ((uint) '?'));
				return;
			}
			//Render
			FT.FT_Load_Glyph (facePtr, index, FT.FT_LOAD_DEFAULT | FT.FT_LOAD_TARGET_NORMAL);
			var faceRec = (FT.FaceRec) Marshal.PtrToStructure (facePtr, typeof (FT.FaceRec));
			FT.FT_Render_Glyph (faceRec.glyph, FT.FT_RENDER_MODE_NORMAL);
			var glyphRec = (FT.GlyphSlotRec) Marshal.PtrToStructure (faceRec.glyph, typeof (FT.GlyphSlotRec));

			//Check for glyph that is only spacing
			if (glyphRec.bitmap.width == 0 || glyphRec.bitmap.rows == 0) {
				glyphs.Add (cp, new GlyphInfo (
					(int)Math.Ceiling(FTMath.From26Dot6 (glyphRec.advance.x)),
					(int)Math.Ceiling(FTMath.From26Dot6 (glyphRec.advance.y)),
					index
				));
				return;
			}
			var pixels = new RGBA[glyphRec.bitmap.width * glyphRec.bitmap.rows];
			if (glyphRec.bitmap.pixel_mode == 2) {
				byte* data = (byte*)glyphRec.bitmap.buffer;
				for (int i = 0; i < glyphRec.bitmap.width * glyphRec.bitmap.rows; i++) {
					//TODO: 4 bytes used for 1 byte of alpha data? investigate compression with GL_RED and shader.
					pixels [i] = new RGBA (255, 255, 255, data [i]);
				}
			} else {
				throw new NotImplementedException ();
			}
			if (currentX + glyphRec.bitmap.width  > TEXTURE_SIZE) {
				currentX = 0;
				currentY += lineMax;
				lineMax = 0;
			}
			if (currentY + glyphRec.bitmap.rows  > TEXTURE_SIZE) {
				currentX = 0;
				currentY = 0;
				lineMax = 0;
				pages.Add (GetNewTexture ());
			}
			lineMax = (int) Math.Max (lineMax, glyphRec.bitmap.rows);
			var rect = new Rectangle (currentX, currentY, glyphRec.bitmap.width, glyphRec.bitmap.rows);
			var tex = pages [pages.Count - 1];
			tex.SetData (pixels, rect);
			currentX += glyphRec.bitmap.width;
			glyphs.Add (cp,
				new GlyphInfo (
					tex, 
					rect, 
					(int)Math.Ceiling (FTMath.From26Dot6 (glyphRec.advance.x)),
					(int)Math.Ceiling (FTMath.From26Dot6 (glyphRec.advance.y)),
					(int)Math.Ceiling (FTMath.From26Dot6 (glyphRec.metrics.horiAdvance)),
					glyphRec.bitmap_left,
					glyphRec.bitmap_top,
					index
				)
			);
		}

		struct RGBA
		{
			public byte R;
			public byte G;
			public byte B;
			public byte A;

			public RGBA (byte r, byte g, byte b, byte a) {
				R = r;
				G = g;
				B = b;
				A = a;
			}
		}

		GlyphInfo GetGlyph (uint cp) {
			if (!glyphs.ContainsKey (cp))
				AddCharacter (cp);
			return glyphs [cp];
		}

		Texture2D GetNewTexture () {
			var texconfig = new TextureConfiguration () {
				Interpolation = InterpolationMode.Linear,
				Mipmap = false
			};
			return new Texture2D (texconfig, TEXTURE_SIZE, TEXTURE_SIZE);
		}

		/// <summary>
		/// Iterates through a string properly for display.
		/// Handles double width codepoints
		/// </summary>
		struct CodepointIterator
		{
			string str;
			int strIndex;
			public int Index;
			public int Count;
			public uint Codepoint;

			public uint PeekNext () {
				if (Index >= Count - 1)
					return 0;
				return (uint) char.ConvertToUtf32 (str, strIndex);
			}

			public bool Iterate () {
				if (Index >= Count)
					return false;
				Codepoint = (uint) char.ConvertToUtf32 (str, strIndex);
				if (char.IsHighSurrogate (str, strIndex))
					strIndex++;
				strIndex++;
				Index++;
				return true;
			}

			public CodepointIterator (string str) {
				this.str = str;
				Count = 0;
				for (int i = 0; i < str.Length; i++) {
					Count++;
					if (char.IsHighSurrogate (str, i))
						i++;
				}
				Index = 0;
				Codepoint = 0;
				strIndex = 0;
			}
		}
	}
}

