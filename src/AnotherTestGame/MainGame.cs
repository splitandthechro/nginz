using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using nginz;
using nginz.Graphics;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;

namespace AnotherTestGame {
	class MainGame : Game {
		Texture2D test;

		public MainGame (GameConfiguration config)
			: base (config) { }

		protected override void Initialize () {
			GL.ClearColor (.25f, .30f, .35f, 1f);
			Content.ContentRoot = "../../assets";

			test = Content.Load<Texture2D> ("nginz.png", TextureConfiguration.Nearest);
			
			base.Initialize ();
		}

		protected override void Draw (GameTime time) {
			GL.Clear (ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
			
			SpriteBatch.Begin ();
			SpriteBatch.Draw (test, Vector2.Zero, Color4.White);
			SpriteBatch.End ();

			base.Draw (time);
		}
	}
}
