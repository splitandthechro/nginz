using System;
using nginz;
using nginz.Common;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;

namespace FlappyMascot
{
	public class MainGame : Game
	{
		MenuScene menuScene;
		GameScene gameScene;

		public MainGame (GameConfiguration conf)
			: base (conf) {
		}

		protected override void Initialize () {
			Content.ContentRoot = "../../assets";
			menuScene = new MenuScene ();
			gameScene = new GameScene ();
			menuScene.MakeActive ();
			base.Initialize ();
		}

		protected override void Update (GameTime time) {
			UI.Update (time);
			base.Update (time);
		}

		protected override void Draw (GameTime time) {
			GL.ClearColor (Color4.White);
			GL.Clear (ClearBufferMask.ColorBufferBit);
			SpriteBatch.Begin ();
			UI.Draw (time, SpriteBatch);
			SpriteBatch.End ();
			base.Draw (time);
		}
	}
}

