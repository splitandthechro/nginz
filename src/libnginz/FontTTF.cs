using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using nginz.Common;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using SharpFont;

namespace nginz
{
	public class FontTTF
	{
		//Size of font textures (in pixels)
		const int TEXTURE_SIZE = 512;

		/// <summary>
		/// Global FT_Library instance.
		/// </summary>
		static Library freetype;

		static FontTTF () {
			freetype = new Library ();
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
		Face face;
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
		/// <param name="emSize">The size of the font in pt.</param>
		public FontTTF (string filename, float emSize) {
			
			if (!File.Exists (filename))
				throw new FileNotFoundException (filename);
			
			face = new Face (freetype, filename);
			face.SetCharSize (new Fixed26Dot6 (0), new Fixed26Dot6 (emSize), 0, 96);
			lineHeight = (int) face.Size.Metrics.Height;
			pages.Add (GetNewTexture ());
			kerning = face.HasKerning;
			//Rasterize ASCII
			for (uint i = 32; i < 127; i++) {
				AddCharacter (i);
			}
		}

		public Point MeasureString(string text)
		{
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
					var kvec = face.GetKerning (glyph.CharIndex, g2.CharIndex, KerningMode.Default);
					penX += (float)kvec.X;
				}
			}
			return new Point ((int) penX, (int) penY);
		}

		public void DrawString(SpriteBatch spriteBatch, string text, Vector2 position, Color4 color)
		{
			DrawString (spriteBatch, text, (int)position.X, (int)position.Y, color);	
		}

		public void DrawString(SpriteBatch spriteBatch, string text, int x, int y, Color4 color)
		{
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
					var kvec = face.GetKerning (glyph.CharIndex, g2.CharIndex, KerningMode.Default);
					penX += (float)kvec.X;
				}
			}
		}
		void AddCharacter (uint cp) {
			//Handle Tab
			if (cp == (uint) '\t') {
				var spaceGlyph = GetGlyph ((uint) ' ');
				glyphs.Add (cp, new GlyphInfo (spaceGlyph.AdvanceX * 5, spaceGlyph.AdvanceY, spaceGlyph.CharIndex));
				return;
			}
			uint index = face.GetCharIndex (cp);
			//Check if Glyph exists
			if (index == 0) {
				if (cp == (uint) '?')
					throw new Exception ("Font does not contain required character '?'");
				glyphs.Add (cp, GetGlyph ((uint) '?'));
				return;
			}
			//Render
			face.LoadGlyph (index, LoadFlags.Default, LoadTarget.Normal);
			face.Glyph.RenderGlyph (RenderMode.Normal);
			//Check for glyph that is only spacing
			if (face.Glyph.Bitmap.Width == 0 || face.Glyph.Bitmap.Rows == 0) {
				glyphs.Add (cp, new GlyphInfo (
					(int) face.Glyph.Advance.X,
					(int) face.Glyph.Advance.Y,
					index
				));
				return;
			}
			var pixels = new RGBA[face.Glyph.Bitmap.Width * face.Glyph.Bitmap.Rows];
			var ftpix = face.Glyph.Bitmap.BufferData;
			if (face.Glyph.Bitmap.PixelMode == PixelMode.Gray) {
				for (int i = 0; i < ftpix.Length; i++) {
					pixels [i] = new RGBA (255, 255, 255, ftpix [i]);
				}
			} else {
				throw new NotImplementedException ();
			}
			if (currentX + face.Glyph.Bitmap.Width > TEXTURE_SIZE) {
				currentX = 0;
				currentY += lineMax;
				lineMax = 0;
			}
			if (currentY + face.Glyph.Bitmap.Rows > TEXTURE_SIZE) {
				currentX = 0;
				currentY = 0;
				lineMax = 0;
				pages.Add (GetNewTexture ());
			}
			lineMax = (int) Math.Max (lineMax, face.Glyph.Bitmap.Rows);
			var rect = new Rectangle (currentX, currentY, face.Glyph.Bitmap.Width, face.Glyph.Bitmap.Rows);
			var tex = pages [pages.Count - 1];
			tex.SetData (pixels, rect);
			currentX += face.Glyph.Bitmap.Width;
			glyphs.Add (cp,
				new GlyphInfo (
					tex,
					rect,
					(int) face.Glyph.Advance.X,
					(int) face.Glyph.Advance.Y,
					(int) face.Glyph.Metrics.HorizontalAdvance,
					face.Glyph.BitmapLeft,
					face.Glyph.BitmapTop,
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
			public RGBA(byte r, byte g, byte b, byte a)
			{
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

