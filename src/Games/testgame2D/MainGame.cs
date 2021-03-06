﻿using System;
using nginz;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using System.Reflection;

namespace testgame2D {
	class MainGame : Game {
		
		SpriteBatch batch;
		Texture2D tex;

		SpriteSheet2D testSheet;
		TileMap testMap;

		dynamic stage;

		public MainGame (GameConfiguration conf) 
			: base (conf) { }

		protected override void Initialize () {
			var version = Assembly.GetEntryAssembly ().GetName ().Version;
			batch = new SpriteBatch (this);
			tex = Content.Load<Texture2D> ("nginz.png", TextureConfiguration.Nearest);

			testSheet = new SpriteSheet2D (Content.Load<Texture2D> ("classical_ruin_tiles_1.png", TextureConfiguration.Nearest), 23, 16);
			testMap = new TileMap (new Vector2 (128, 256), 32, 32, new Vector2(32, 32), 2);
			testMap.AddLayer ("testLayer", testSheet);

			/*testMap.SetTile ("testLayer", 0, 1, 0);
			testMap.SetTile ("testLayer", 1, 1, 1);
			testMap.SetTile ("testLayer", 2, 1, 2);
			testMap.SetTile ("testLayer", 3, 1, 3);*/

			stage = new Stage (this);
			var mascot = new MascotActor ();
			stage.AddActor (mascot);
			stage.AddAction (mascot);
			stage.TileMap = testMap;

			base.Initialize ();
		}

		protected override void OnResize () {
			base.OnResize ();
		}

		protected override void Update (GameTime time) {

			// Exit if escape is pressed
			if (Keyboard.IsKeyTyped (Key.Escape))
				Exit ();

			stage.Act (time);

			base.Update (time);
		}

		protected override void Draw (GameTime time) {
			GL.ClearColor (.25f, .30f, .35f, 1f);
            GL.Clear (ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            batch.Begin ();
			batch.Draw (tex, Vector2.Zero, Color4.White, Vector2.One);
			testMap.Draw (batch);
			stage.Draw (time, batch);
			batch.End ();

			base.Draw (time);
		}
	}
}
