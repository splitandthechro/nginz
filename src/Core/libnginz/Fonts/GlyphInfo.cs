using System;
using System.Drawing;

namespace nginz
{

	/// <summary>
	/// Glyph info.
	/// </summary>
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
}

