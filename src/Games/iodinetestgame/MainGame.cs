using System;
using nginz;
using nginz.Common;
using nginz.Scripting.Iodine;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Graphics;

namespace iodinetestgame
{
	public class MainGame : Game
	{
		IodineVM iodine;

		public MainGame (GameConfiguration conf)
			: base (conf) { }

		protected override void Initialize () {
			ContentRoot = "../../assets";
			iodine = new IodineVM (this);
			var script = Content.Load<IodineScript> ("test");
			iodine.LoadLive (script);
			iodine.Call ("initialize", this);
		}

		protected override void Update (GameTime time) {
			GL.ClearColor (Color4.White);
			GL.Clear (ClearBufferMask.ColorBufferBit);
			base.Update (time);
		}

		protected override void Draw (GameTime time) {
			base.Draw (time);
		}
	}
}

