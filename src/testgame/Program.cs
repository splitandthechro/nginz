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
				FixedFramerate = false,
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

			// Directories
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

			var suzanne = ObjLoaderFactory.LoadFrom ("models\\suzanne.obj");
			var vpos = new GLBuffer<Vector3> (GLBufferSettings.StaticDraw3FloatArray, suzanne.Vertices);
			var geometry = new Geometry (BeginMode.Quads)
				.AddBuffer ("v_pos", vpos);
			var model = new Model (geometry);

			base.Initialize ();
		}

		protected override void Update (GameTime time) {
			vm.Invoke ("update");
			base.Update (time);
		}

		protected override void Draw (GameTime time) {
			vm.Invoke ("draw");
			base.Draw (time);
		}
	}
}
