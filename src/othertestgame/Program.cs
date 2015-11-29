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

			Mouse.ShouldCenterMouse = true;

			GL.CullFace (CullFaceMode.Back);

			// Why do we only cull the face? lol
			GL.Enable (EnableCap.CullFace);
			GL.Enable (EnableCap.Blend);
			GL.BlendFunc (BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

			GraphicsContext.Assert ();

			var vertexShader = Content.Load<VertexShader> ("passTex.vs");
			var fragmentShader = Content.Load<FragmentShader> ("passTex.fs");
			program = new ShaderProgram (vertexShader, fragmentShader);

			program.Link ();

			testTexture = Content.Load<Texture2D> ("classical_ruin_tiles_1.png", TextureConfiguration.Nearest);

			var res = new Resolution { Width = Configuration.Width, Height = Configuration.Height };
			camera = new FPSCamera (60f, res, Mouse, Keyboard);
			camera.Camera.SetAbsolutePosition (new Vector3 (0, 0, 2));

			var obj = Content.Load<ObjFile> ("box.obj");

			testModel = new TexturedModel (obj, 0, program);
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

			program.Use (() => testModel.Draw (program, camera.Camera, testTexture));

			base.Draw (time);
		}
	}
}
