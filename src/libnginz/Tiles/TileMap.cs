using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using nginz.Common;
using OpenTK;

namespace nginz.Tiles
{

	/// <summary>
	/// Tile map.
	/// </summary>
	public class TileMap: ICanThrow
	{

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
		public Dictionary<string, TileMapLayer> Layers = new Dictionary<string, TileMapLayer> ();

		/// <summary>
		/// Gets the <see cref="nginz.Tiles.TileMapLayer"/> with the specified name.
		/// </summary>
		/// <param name="name">Name.</param>
		public TileMapLayer this [string name] {
			get { return GetLayer (name); }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="nginz.Tiles.TileMap"/> class.
		/// </summary>
		/// <param name="width">Width.</param>
		/// <param name="height">Height.</param>
		/// <param name="scale">Scale.</param>
		public TileMap (int width, int height, float scale) {
			Width = width;
			Height = height;
			Scale = scale;
		}

		/// <summary>
		/// Add a layer.
		/// </summary>
		/// <param name="name">Name.</param>
		/// <param name="sheet">Sheet.</param>
		public void AddLayer (string name, SpriteSheet2D sheet) {
			Layers[name] = new TileMapLayer (sheet, Width, Height, Scale);
		}

		/// <summary>
		/// Get a layer.
		/// </summary>
		/// <returns>The layer.</returns>
		/// <param name="name">Name.</param>
		public TileMapLayer GetLayer (string name) {
			if (!Layers.ContainsKey (name))
				this.Throw ("Layer {0} not found", name);

			return Layers[name];
		}

		/// <summary>
		/// Set a tile.
		/// </summary>
		/// <param name="name">Name.</param>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		/// <param name="tile">Tile.</param>
		public void SetTile (string name, int x, int y, int tile) {
			GetLayer(name).SetTile(x, y, tile);
		}

		/// <summary>
		/// Set a tile.
		/// </summary>
		/// <param name="name">Name.</param>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		/// <param name="tileX">Tile x.</param>
		/// <param name="tileY">Tile y.</param>
		public void SetTile (string name, int x, int y, int tileX, int tileY) {
			GetLayer (name).SetTile (x, y, tileX, tileY);
		}

		/// <summary>
		/// Draw the tiles.
		/// </summary>
		/// <param name="batch">Batch.</param>
		/// <param name="position">Position.</param>
		public void Draw (SpriteBatch batch, Vector2? position = null) {
			Layers.Values.ToList ().ForEach (x => x.Draw (batch, position ?? Vector2.Zero));
		}
	}
}
