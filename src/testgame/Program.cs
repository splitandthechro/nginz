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
		double time_accum;
		int updates;
		bool first;

		public TestGame (GameConfiguration conf) : base (conf) {
			first = true;
		}

		protected override void Initialize () {

			// Mod directory
			const string mods = "..\\..\\mods\\";

			// Create the iodine vm
			vm = new IodineVM ();

			// Expose functions to the vm
			vm.ExposeFunction ("SayHello", SayHello);

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
			updates++;
			time_accum += time.Elapsed.TotalMilliseconds;
			if (time_accum >= 1000d) {
				//if (!first)
				//	this.Log ("Measured fps: {0}", updates);
				first = false;
				updates = 0;
				time_accum = 1000d - time_accum;
				vm.Invoke ("hello");
			}
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
