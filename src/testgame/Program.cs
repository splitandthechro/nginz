using System;
using nginz;
using nginz.Interop.Iodine;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace testgame
{
	class MainClass
	{
		public static void Main (string[] args) {
			var conf = new GameConfiguration {
				Width = 640,
				Height = 480,
				WindowTitle = "nginz Game",
				FixedWindow = false,
				Vsync = VsyncMode.Off,
				FixedFramerate = true,
				TargetFramerate = 60,
			};
			var game = new TestGame (conf);
			game.Run ();
		}
	}

	class TestGame : Game
	{
		SpriteBatch_old batch;
		Texture2D wood;

		public TestGame (GameConfiguration conf) : base (conf) {
		}

		protected override void Initialize () {
			batch = new SpriteBatch_old ();
			wood = Content.Load<Texture2D> ("testWood.jpg");
			base.Initialize ();
		}

		protected override void Update (GameTime time) {
			if (Keyboard.IsKeyDown (OpenTK.Input.Key.Escape))
				Exit ();
			base.Update (time);
		}

		protected override void Draw (GameTime time) {
			GL.ClearColor (1.0f, 1.0f, 1.0f, 1.0f);
			GL.Clear (ClearBufferMask.ColorBufferBit);
			batch.Draw (wood);
			base.Draw (time);
		}
	}
}
