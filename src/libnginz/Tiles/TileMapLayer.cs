using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics;

namespace nginz.Tiles {
	public class TileMapLayer {
		public SpriteSheet2D Sheet;

		public int Width;
		public int Height;
		public int Scale;

		public int[] Layer;

		public TileMapLayer (SpriteSheet2D sheet, int width, int height, int scale) {
			Sheet = sheet;

			Width = width;
			Height = height;
			Scale = scale;

			Layer = new int[Width * Height];
			for (int i = 0; i < Width * Height; i++)
				Layer[i] = -1;
		}

		public void SetTile (int x, int y, int tile) {
			Layer[x + y * Width] = tile;
		}
		public void SetTile (int x, int y, int tileX, int tileY) {
			Layer[x + y * Width] = tileX + tileY * Sheet.TilesY;
		}

		public void Draw (SpriteBatch batch) {
			for (int y = 0; y < Height; y++)
				for (int x = 0; x < Width; x++) {
					var tile = Layer[x + y * Width];
					if (tile != -1) {
						var xPos = x * Sheet.TileWidth * Scale;
						var yPos = y * Sheet.TileHeight * Scale;
						batch.Draw (Sheet.Texture, Sheet[tile], new Vector2 (xPos, yPos), Color4.White, scale: new Vector2 (Scale));
					}
				}
		}
	}
}
