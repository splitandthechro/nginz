using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using nginz;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;

namespace TowerDefense {
	public class MainGame : Game {
		Texture2D UITexture;
		SpriteSheet2D SpriteSheet;
		TileMap TileMap;

		Font Font;

		public MainGame (GameConfiguration config) 
			: base (config) { }

		protected override void Initialize () {
			Content.ContentRoot = "../../assets";

			UITexture = Content.Load<Texture2D> ("TDUserInterface.png", TextureConfiguration.Nearest);
			SpriteSheet = new SpriteSheet2D (Content.Load<Texture2D> ("TDSheet.png", TextureConfiguration.Nearest), 16, 16);

			TileMap = new TileMap (Vector2.Zero, 26 / 2, 18 / 2, new Vector2 (16, 16), 4);
			TileMap.AddLayer ("GrassLayer", SpriteSheet);

			for (int y = 0; y < TileMap.Height; y++)
				for (int x = 0; x < TileMap.Width; x++)
					TileMap.SetTile ("GrassLayer", x, y, new Tile { TileId = SpriteSheet.GetTileId(1, 1) });

			TileMap.AddLayer ("Track", SpriteSheet);
			var tiles = new int[][] {
				new int[] { },
				new int[] { 20, 1, 1, 2 },
                new int[] { -1, -1, -1, 32, 1, 2 },
                new int[] { 0, 1, 2, -1, -1, 16, -1, 16 },
                new int[] { 16, -1, 16, -1, -1, 18, -1, 16 },
                new int[] { 16, -1, 32, 1, 1, 34, -1, 16 },
				new int[] { 16, -1, -1, -1, -1, -1, -1, 16 },
                new int[] { 32, 1, 1, 1, 1, 1, 1, 34 }
			};

			for (int y = 0; y < tiles.Length; y++)
				for (int x = 0; x < tiles[y].Length; x++)
					TileMap.SetTile ("Track", x, y, new Tile { TileId = tiles[y][x] });

			Font = Content.Load<Font> ("durselinvenice2015.ttf", 15f);

			base.Initialize ();
		}

		protected override void Draw (GameTime time) {
			GL.ClearColor (.25f, .30f, .35f, 1f);
			GL.Clear (ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			SpriteBatch.Begin ();
			TileMap.Draw (SpriteBatch);
			SpriteBatch.Draw (UITexture, Vector2.Zero, Color4.White, scale: 8f, rotation: 0);
			SpriteBatch.Draw (SpriteSheet.Texture, SpriteSheet[0, 5], new Vector2(856, 96), Color4.White, scale: 1f, rotation: 0);
			SpriteBatch.Draw (SpriteSheet.Texture, SpriteSheet[0, 3], new Vector2 ((float) Math.Floor(Mouse.X / 64), (float) Math.Floor (Mouse.Y / 64)) * 64, Color4.White, scale: 4f, rotation: 0);
			SpriteBatch.Draw (SpriteSheet.Texture, SpriteSheet[1, 5], new Vector2 (856, 144), Color4.White, scale: 1f, rotation: 0);
			Font.DrawString (SpriteBatch, "100", new Vector2 (872, 86), Color4.White);
			Font.DrawString (SpriteBatch, "32", new Vector2 (872, 134), Color4.White);

			SpriteBatch.End ();

			base.Draw (time);
		}
	}
}
