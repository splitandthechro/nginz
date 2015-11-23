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
		Texture2 testTexture;
		Camera camera;
		ShaderProgram program;

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
			GL.Enable (EnableCap.CullFace);

			GraphicsContext.Assert ();

			var vertexShader = BasicShader.FromFile<VertexShader> (GetAssetPath ("shaders", "passTex.vs"));
			var fragmentShader = BasicShader.FromFile<FragmentShader> (GetAssetPath ("shaders", "passTex.fs"));
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

			testTexture = Texture2.FromFile (GetAssetPath ("textures", "testWood.jpg"));

			camera = new Camera (60f, new Resolution { Width = Configuration.Width, Height = Configuration.Height }, 0.1f, 64.0f);
			camera.SetAbsolutePosition (new Vector3 (0, 0, 2));
		}

		protected override void Update (GameTime time) {
			//camera.SetRelativePosition (new Vector3(0, 0, (theTime / 60000f) * time.Elapsed.Milliseconds));

			const float speed = 1f;
			var movement = speed * (float) time.Elapsed.TotalSeconds;
			var left = new Vector3 (-movement, 0, 0);
			var up = new Vector3 (0, 0, movement);
			var enlarge = movement;
			var flip = 180f;

			// Move to the left if Left or A is pressed
			if (Keyboard.IsAnyKeyDown (Key.Left, Key.A))
				camera.Position += left;

			// Move to the right if Right or D is pressed
			if (Keyboard.IsAnyKeyDown (Key.Right, Key.D))
				camera.Position -= left;

			// Move up if Up or W is pressed
			if (Keyboard.IsAnyKeyDown (Key.Up, Key.W))
				camera.Position -= up;

			// Move down if Down or S is pressed
			if (Keyboard.IsAnyKeyDown (Key.Down, Key.S))
				camera.Position += up;

			// Scale up if + is pressed
			if (Keyboard.IsAnyKeyDown (Key.Plus, Key.KeypadPlus))
				camera.Position -= new Vector3(0, 0, enlarge);

			// Scale down if - is pressed
			if (Keyboard.IsAnyKeyDown (Key.Minus, Key.KeypadMinus))
				camera.Position += new Vector3 (0, 0, enlarge);

			if (Keyboard.IsKeyDown (Key.E))
				camera.SetRotation (Vector3.UnitY, .25f, relative: true);
			if (Keyboard.IsKeyDown (Key.Q))
				camera.SetRotation (Vector3.UnitY, -.25f, relative: true);

			// Flip texture if f is pressed
			if (Keyboard.IsKeyTyped (Key.F))
				camera.SetRotation (Vector3.UnitZ, flip, relative: true);

			base.Update (time);
		}

		public override void Resize (Resolution resolution) {
			base.Resize (resolution);
			camera.UpdateCameraMatrix (resolution);
		}

		protected override void Draw (GameTime time) {
			GL.ClearColor (.25f, .30f, .35f, 1f);
			GL.Clear (ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			using (program.UseProgram ()) {
				testModel.Draw (program, camera, testTexture);
			}

			base.Draw (time);
		}
	}
}
