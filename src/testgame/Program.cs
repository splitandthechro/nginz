using System;
using Iodine.Runtime;
using nginz;
using nginz.Common;
using nginz.Interop.Iodine;
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
		IodineVM vm;

		public TestGame (GameConfiguration conf) : base (conf) {
		}

		protected override void Initialize () {

			// Mod directory
			const string mods = "..\\..\\mods\\";

			// Create the iodine vm
			vm = new IodineVM (this);

			// Load modules
			vm.LoadModules (
				mods + "test.id"
			);

			// Observe modules for live-reload
			vm.Observe (
				mods + "test.id"
			);	

			base.Initialize ();
		}

		protected override void Update (GameTime time) {
			vm.Invoke ("update");
			base.Update (time);
		}

		protected override void Draw (GameTime time) {
			vm.Invoke ("draw");
			GL.Clear (ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
			base.Draw (time);
		}
	}
}
