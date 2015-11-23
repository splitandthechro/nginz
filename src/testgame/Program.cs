using System;
using nginz;
using nginz.Interop.Iodine;
using OpenTK;

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

		Vector3[] points = {
			new Vector3 (-.5f, .5f, .0f),
			new Vector3 (.5f, -.5f, .0f),
			new Vector3 (-.5f, -.5f, .0f),
			new Vector3 (.5f, .5f, .0f),
		};

		Vector3[] colors = {
			new Vector3 (1.0f, .0f, .0f),
			new Vector3 (.0f, 1.0f, .0f),
			new Vector3 (.0f, .0f, 1.0f),
			new Vector3 (.0f, 1.0f, .0f),
		};

		uint[] indices = {
			0, 1, 2,
			0, 1, 3,
		};

		//Geometry square;
		//ShaderProgram program;

		public TestGame (GameConfiguration conf) : base (conf) {
		}

		protected override void Initialize () {

			// Directories
			const string mods = "..\\..\\mods\\";
			const string shaders = "shaders\\";

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

			var vertexShader = BasicShader.FromFile<VertexShader> (shaders + "basic.vs");
			var fragmentShader = BasicShader.FromFile<FragmentShader> (shaders + "basic.fs");
			//program = new ShaderProgram (vertexShader, fragmentShader);
			//program.Link ();

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
