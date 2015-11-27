using System;
using System.IO;
using nginz;
using nginz.Common;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using System.Reflection;

namespace othertestgame {
	class Program {
		static void Main (string[] args) {
			var conf = new GameConfiguration {
				Width = 1280,
				Height = 720,
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

	class TestGame : Game, ICanLog {
		
		Vector3[] points = {
			new Vector3 (+1f, -1f, .0f), // 0
			new Vector3 (-1f, +1f, .0f), // 1
			new Vector3 (-1f, -1f, .0f), // 2
			new Vector3 (+1f, +1f, .0f), // 3
		};

		Vector3[] colors = {
			new Vector3 (1.0f, 1.0f, 1.0f), // 0
			new Vector3 (1.0f, 1.0f, 1.0f), // 1
			new Vector3 (1.0f, 1.0f, 1.0f), // 2
			new Vector3 (1.0f, 1.0f, 1.0f), // 3
		};

		Vector2[] texCoords = {
			new Vector2 (1.0f, 1.0f), // 0
			new Vector2 (0.0f, 0.0f), // 1
			new Vector2 (0.0f, 1.0f), // 2
			new Vector2 (1.0f, 0.0f), // 3
		};

		uint[] indices = {
			0, 1, 2, // left
			1, 0, 3, // right
		};

		TexturedModel testModel;
		Texture2D testTexture;
		FPSCamera camera;
		ShaderProgram program;
		Fontmap testFont;

		public TestGame (GameConfiguration conf) : base (conf) {
		}

		static string GetAssetPath (string directory, string asset) {
			var location = Assembly.GetEntryAssembly ().Location;
			var directoryName = Path.GetDirectoryName (location);
			return Path.Combine (directoryName, directory, asset);
		}

		protected override void Initialize () {
			base.Initialize ();

			GL.CullFace (CullFaceMode.Back);

			// Why do we only cull the face? lol
			//GL.Enable (EnableCap.CullFace);

			GraphicsContext.Assert ();

			var vertexShader = content.Load<VertexShader> ("passTex.vs");
			var fragmentShader = content.Load<FragmentShader> ("passTex.fs");
			program = new ShaderProgram (vertexShader, fragmentShader);

			program.Link ();

			var v_pos = new GLBuffer<Vector3> (GLBufferSettings.StaticDraw3FloatArray, points);
			var v_col = new GLBuffer<Vector3> (GLBufferSettings.StaticDraw3FloatArray, colors);
			var v_tex = new GLBuffer<Vector2> (GLBufferSettings.StaticDraw2FloatArray, texCoords);

			var ind = new GLBuffer<uint> (GLBufferSettings.Indices, indices);

			var testGeometry = new Geometry (BeginMode.Triangles)
				.AddBuffer ("v_pos", v_pos)
				.AddBuffer ("v_col", v_col)
				.AddBuffer ("v_tex", v_tex)
				.SetIndices (ind)
				.Construct (program);

			testModel = new TexturedModel (testGeometry);

			testTexture = content.Load<Texture2D> ("testWood.jpg");

			var res = new Resolution { Width = Configuration.Width, Height = Configuration.Height };
			camera = new FPSCamera (60f, res, Mouse, Keyboard);
			camera.Camera.SetAbsolutePosition (new Vector3 (0, 0, 2));

			testFont = new Fontmap (camera.Camera, "Consolas", 11.25f);
			testFont.SetText ("Hello, World!");
		}

		protected override void Update (GameTime time) {
			camera.Update (time);

			// Exit if escape is pressed
			if (Keyboard.IsKeyTyped (Key.Escape))
				Exit ();

			base.Update (time);
		}

		protected override void Resize (Resolution resolution) {
			base.Resize (resolution);
			camera.Camera.UpdateCameraMatrix (resolution);
		}

		protected override void Draw (GameTime time) {
			GL.ClearColor (.25f, .30f, .35f, 1f);
			GL.Clear (ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			using (program.UseProgram ()) {
				testModel.Draw (program, camera.Camera, testTexture);
			}

			using (testFont.program.UseProgram ()) {
				testFont.Draw ();
			}

			base.Draw (time);
		}
	}
}
