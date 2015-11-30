using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using nginz;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using nginz.Common;
using System.Reflection;
using nginz.Staging;
using nginz.Tiles;

namespace testgame2D {
	class MainGame : Game {
		
		SpriteBatch batch;
		Texture2D tex;
		Fontmap font;

		SpriteSheet2D testSheet;
		TileMapLayer testLayer;

		Stage stage;

		public MainGame (GameConfiguration conf) 
			: base (conf) { }

		protected override void Initialize () {
			ContentRoot = "../../assets";
			base.Initialize ();

			var version = Assembly.GetEntryAssembly ().GetName ().Version;
			batch = new SpriteBatch ();
			font = new Fontmap (Resolution, "Source Sans Pro", 20.25f)
				.SetColor (Color4.White)
				.SetText ("nginz alpha v{0}", version.ToString (4));
			tex = Content.Load<Texture2D> ("nginz.png", TextureConfiguration.Nearest);

			testSheet = new SpriteSheet2D (Content.Load<Texture2D> ("classical_ruin_tiles_1.png", TextureConfiguration.Nearest), 23, 16);
			testLayer = new TileMapLayer (testSheet, 32, 32, 2);

			testLayer.SetTile (0, 1, 0);
			testLayer.SetTile (1, 1, 1);
			testLayer.SetTile (2, 1, 2);
			testLayer.SetTile (3, 1, 3);

			stage = new Stage (this);
			var mascot = new MascotActor ();
			stage.AddActor (mascot);
			stage.AddAction (mascot);
		}

		protected override void Resize () {
			base.Resize ();
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
			testLayer.Draw (batch);
			font.Draw (batch);
			stage.Draw (time, batch);
			batch.End ();

			base.Draw (time);
		}
	}
}
