using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace nginz {
	public class SpriteSheet2D {
		public Rectangle this[int x, int y] {
			get {
				var xStart = x * TileWidth;
				var yStart = y * TileHeight;
				return new Rectangle (xStart, yStart, TileWidth, TileHeight);
			}
		}
		public Rectangle this[int tile] {
			get {
				var x = tile % TilesX;
				var y = tile / TilesX;
				var xStart = x * TileWidth;
				var yStart = y * TileHeight;
				return new Rectangle (xStart, yStart, TileWidth, TileHeight);
			}
		}

		public int TileWidth { get { return Texture.Width / TilesX; } }
		public int TileHeight { get { return Texture.Height / TilesY; } }

		public int TilesX;
		public int TilesY;

		public Texture2D Texture;

		public SpriteSheet2D (Texture2D texture, int tilesX, int tilesY) {
			TilesX = tilesX;
			TilesY = tilesY;

			Texture = texture;
		}
	}
}
