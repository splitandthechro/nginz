using System;
using nginz;
using nginz.Common;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;

namespace othertestgame {
	class Program {
		static void Main (string[] args) {
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

	class TestGame : Game, ICanLog {
		
		Vector3[] points = {
			new Vector3 (-.5f, .5f, .0f), // 0
			new Vector3 (.5f, -.5f, .0f), // 1
			new Vector3 (-.5f, -.5f, .0f),// 2
			new Vector3 (.5f, .5f, .0f),  // 3
		};

		Vector3[] colors = {
			new Vector3 (1.0f, 0.0f, 0.0f), // 0
			new Vector3 (0.0f, 1.0f, 0.0f), // 1
			new Vector3 (0.0f, 0.0f, 1.0f), // 2
			new Vector3 (0.0f, 1.0f, 1.0f), // 3
		};

		uint[] indices = {
			0, 1, 2, // left
			0, 1, 3, // right
		};

		Geometry testGeometry;

		ShaderProgram program;

		public TestGame (GameConfiguration conf) : base (conf) {
		}

		protected override void Initialize () {
			base.Initialize ();

			GraphicsContext.Assert ();

			var vertexShader = BasicShader.FromFile<VertexShader> ("shaders\\passthrough.vs");
			var fragmentShader = BasicShader.FromFile<FragmentShader> ("shaders\\passthrough.fs");
			program = new ShaderProgram (vertexShader, fragmentShader);

			program.Link ();

			var v_pos = new GLBuffer<Vector3> (GLBufferSettings.StaticDraw3FloatArray, points);
			var v_col = new GLBuffer<Vector3> (GLBufferSettings.StaticDraw3FloatArray, colors);

			var ind = new GLBuffer<uint> (GLBufferSettings.Indices, indices);

			testGeometry = new Geometry ()
				.AddBuffer ("v_pos", v_pos)
				.AddBuffer ("v_col", v_col)
				.SetIndices (ind)
				.Construct (program);
        }

		protected override void Update (GameTime time) {
			
			base.Update (time);
		}

		protected override void Draw (GameTime time) {
			GL.ClearColor (.4f, .4f, .4f, 1f);
			GL.Clear (ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			using (program.UseProgram ()) {
				testGeometry.Draw (BeginMode.Triangles);
			}

			base.Draw (time);
		}
	}
}
