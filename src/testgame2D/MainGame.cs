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

namespace testgame2D {
	class MainGame : Game {
		
		SpriteBatch batch;
		Texture2D tex;
		Fontmap font;

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
		}

		protected override void Resize () {
			base.Resize ();
		}

		protected override void Update (GameTime time) {

			// Exit if escape is pressed
			if (Keyboard.IsKeyTyped (Key.Escape))
				Exit ();

			base.Update (time);
		}

		protected override void Draw (GameTime time) {
			GL.ClearColor (.25f, .30f, .35f, 1f);
			GL.Clear (ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			batch.Begin ();
			batch.Draw (tex, Vector2.Zero, Color4.White, Vector2.One);
			font.Draw (batch);
			batch.End ();

			base.Draw (time);
		}
	}
}
