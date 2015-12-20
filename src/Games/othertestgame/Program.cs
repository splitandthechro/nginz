﻿using System;
using System.IO;
using nginz;
using nginz.Common;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using System.Reflection;
using nginz.Graphics;
using nginz.Lighting;

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
		Model testModel;
		Texture2D testTexture;
		FPSCamera camera;

		Framebuffer framebuffer;

		public TestGame (GameConfiguration conf) : base (conf) {
		}

		protected override void Initialize () {
			GL.ClearColor (.25f, .30f, .35f, 1f);
			Content.ContentRoot = "../../assets";
			Mouse.ShouldCenterMouse = true;
			Mouse.CursorVisible = false;
			GL.CullFace (CullFaceMode.Back);
			GL.Enable (EnableCap.CullFace);

			testTexture = Content.Load<Texture2D> ("testWood.jpg", TextureConfiguration.Linear);
			var res = new Resolution { Width = Configuration.Width, Height = Configuration.Height };
			camera = new FPSCamera (60f, res, Mouse, Keyboard);
			camera.Camera.SetAbsolutePosition (new Vector3 (0, 0, 2));
			camera.MouseRotation.X = MathHelper.DegreesToRadians (180);
			var obj = Content.Load<Geometry> ("suzanne.obj", "");
			obj.Material = new Material (Color4.White, testTexture, 2, 4);
			testModel = new Model(obj);

			framebuffer = new Framebuffer (FramebufferTarget.Framebuffer, this.Configuration.Width, this.Configuration.Height);

			base.Initialize ();
		}

		protected override void Update (GameTime time) {
			camera.Update (time);

			// Exit if escape is pressed
			if (Keyboard.IsKeyTyped (Key.Escape))
				Exit ();

			base.Update (time);
		}

		protected override void OnResize () {
			base.OnResize ();
			camera.Camera.UpdateCameraMatrix (Resolution);
		}

		protected override void Draw (GameTime time) {
			GL.Clear (ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			RenderingPipeline.Draw (camera.Camera, shader => {
				testModel.Draw (shader, camera.Camera);
			});
			RenderingPipeline.Display ();

			base.Draw (time);
		}
	}
}
