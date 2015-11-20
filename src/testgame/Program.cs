using System;
using nginz;
using nginz.Interop.Iodine;
using OpenTK.Graphics.OpenGL4;
using Iodine.Runtime;

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
			};
			var game = new TestGame (conf);
			game.Run ();
		}
	}

    class TestGame : Game
    {
		public TestGame (GameConfiguration conf) : base (conf) {
		}

		protected override void Initialize () {
			var vm = new IodineVM ();
			vm.ExposeFunction ("SayHello", SayHello);
			vm.LoadModule ("test.id");
			vm.Invoke ("hello");
			base.Initialize ();
		}

		protected override void Update (GameTime time) {
			base.Update (time);
		}

		protected override void Draw (GameTime time) {
			GL.ClearColor (.4f, .4f, .4f, 1f);
			GL.Clear (ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
			base.Draw (time);
		}

		static IodineObject SayHello (params IodineObject[] args) {
			Console.WriteLine ("Hello from C#, invoked from Iodine!");
			return null;
		}
    }
}
