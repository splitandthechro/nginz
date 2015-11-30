using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using nginz.Common;
using OpenTK;

namespace nginz.Tiles {
	public class TileMap: ICanThrow {
		public int Width;
		public int Height;

		public float Scale;

		public Dictionary<string, TileMapLayer> Layers = new Dictionary<string, TileMapLayer> ();

		public TileMap (int width, int height, float scale) {
			Width = width;
			Height = height;
			Scale = scale;
		}

		public void AddLayer (string name, SpriteSheet2D sheet) {
			Layers[name] = new TileMapLayer (sheet, Width, Height, Scale);
		}

		public TileMapLayer GetLayer (string name) {
			if (!Layers.ContainsKey (name))
				this.Throw ("Layer {0} not found", name);

			return Layers[name];
		}

		public void SetTile (string name, int x, int y, int tile) {
			GetLayer(name).SetTile(x, y, tile);
		}
		public void SetTile (string name, int x, int y, int tileX, int tileY) {
			GetLayer (name).SetTile (x, y, tileX, tileY);
		}

		public void Draw (SpriteBatch batch, Vector2? position = null) {
			Layers.Values.ToList ().ForEach (x => x.Draw (batch, position ?? Vector2.Zero));
		}
	}
}
