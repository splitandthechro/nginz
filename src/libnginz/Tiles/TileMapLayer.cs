using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics;

namespace nginz
{
	/// <summary>
	/// Tile map layer.
	/// </summary>
	public class TileMapLayer
	{

		/// <summary>
		/// The sprite sheet.
		/// </summary>
		public SpriteSheet2D Sheet;

		/// <summary>
		/// The width.
		/// </summary>
		public int Width;

		/// <summary>
		/// The height.
		/// </summary>
		public int Height;

		/// <summary>
		/// The scale.
		/// </summary>
		public float Scale;

		/// <summary>
		/// The layers.
		/// </summary>
		public int[] Layers;

		public TileMapLayer (SpriteSheet2D sheet, int width, int height, float scale) {
			Sheet = sheet;

			Width = width;
			Height = height;
			Scale = scale;

			Layers = new int[Width * Height];
			for (int i = 0; i < Width * Height; i++)
				Layers[i] = -1;
		}

		public void SetTile (int x, int y, int tile) {
			Layers[x + y * Width] = tile;
		}
		public void SetTile (int x, int y, int tileX, int tileY) {
			Layers[x + y * Width] = tileX + tileY * Sheet.TilesY;
		}
		public int GetTile (int x, int y) {
			if (x < 0 || y < 0 || x >= Width || y >= Width)
				return -1;
			return Layers[x + y * Width];
		}

		public void Draw (SpriteBatch batch, Vector2 position) {
			for (int y = 0; y < Height; y++)
				for (int x = 0; x < Width; x++) {
					var tile = Layers[x + y * Width];
					if (tile != -1) {
						var xPos = x * Sheet.TileWidth * Scale;
						var yPos = y * Sheet.TileHeight * Scale;
						batch.Draw (Sheet.Texture, Sheet[tile], position + new Vector2 (xPos, yPos), Color4.White, scale: new Vector2 (Scale));
					}
				}
		}
	}
}
