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
				Width = 1280,
				Height = 720,
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
			new Vector3 (.35f, -.5f, .0f), // 0
			new Vector3 (-.35f, .5f, .0f), // 1
			new Vector3 (-.35f, -.5f, .0f),// 2
			new Vector3 (.35f, .5f, .0f),  // 3
		};

		Vector3[] colors = {
			new Vector3 (0.0f, 1.0f, 0.0f), // 0
			new Vector3 (1.0f, 0.0f, 0.0f), // 1
			new Vector3 (0.0f, 0.0f, 1.0f), // 2
			new Vector3 (0.0f, 1.0f, 1.0f), // 3
		};

		uint[] indices = {
			0, 1, 2, // left
			1, 0, 3, // right
		};

		Model testModel;
		Camera camera;
		ShaderProgram program;

		public TestGame (GameConfiguration conf) : base (conf) {
		}

		protected override void Initialize () {
			base.Initialize ();

			GL.CullFace (CullFaceMode.Back);
			GL.Enable (EnableCap.CullFace);

			GraphicsContext.Assert ();

			var vertexShader = BasicShader.FromFile<VertexShader> ("shaders\\passthrough.vs");
			var fragmentShader = BasicShader.FromFile<FragmentShader> ("shaders\\passthrough.fs");
			program = new ShaderProgram (vertexShader, fragmentShader);

			program.Link ();

			var v_pos = new GLBuffer<Vector3> (GLBufferSettings.StaticDraw3FloatArray, points);
			var v_col = new GLBuffer<Vector3> (GLBufferSettings.StaticDraw3FloatArray, colors);

			var ind = new GLBuffer<uint> (GLBufferSettings.Indices, indices);

			var testGeometry = new Geometry (BeginMode.Triangles)
				.AddBuffer ("v_pos", v_pos)
				.AddBuffer ("v_col", v_col)
				.SetIndices (ind)
				.Construct (program);

			testModel = new Model (testGeometry);

			camera = new Camera (60f, new Resolution { Width = conf.Width, Height = conf.Height }, 0.1f, 64.0f);
			camera.SetAbsolutePosition (new Vector3 (0, 0, 1));
		}
		int theTime = 0;
		protected override void Update (GameTime time) {
			camera.SetRelativePosition (new Vector3(0, 0, (theTime / 60000f) * time.Elapsed.Milliseconds));
			theTime++;
			base.Update (time);
		}

		protected override void Draw (GameTime time) {
			GL.ClearColor (.25f, .30f, .35f, 1f);
			GL.Clear (ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			using (program.UseProgram ()) {
				testModel.Draw (program, camera);
			}

			base.Draw (time);
		}
	}
}
