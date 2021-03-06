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
using System.Collections.Generic;

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
				ContentRoot = "../../assets",
			};
			var game = new TestGame (conf);
			game.Run ();
		}
	}

	class TestGame : Game, ICanLog {
		List<Model> models = new List<Model> ();
		Texture2D testTexture;
		FPSCamera camera;

		public TestGame (GameConfiguration conf) : base (conf) {
		}

		protected override void Initialize () {
			GL.ClearColor (.25f, .30f, .35f, 1f);
			Mouse.ShouldCenterMouse = true;
			Mouse.CursorVisible = false;
			GL.CullFace (CullFaceMode.Back);
			GL.Enable (EnableCap.CullFace);

			testTexture = Content.Load<Texture2D> ("testWoodDiffuse.jpg", TextureConfiguration.Linear);
			var normal = Content.Load<Texture2D> ("testWoodNormal.jpg", TextureConfiguration.Linear);
			var res = new Resolution { Width = Configuration.Width, Height = Configuration.Height };
			camera = new FPSCamera (60f, res, Mouse, Keyboard);
			camera.Camera.SetAbsolutePosition (new Vector3 (0, 0, 2));
			camera.MouseRotation.X = MathHelper.DegreesToRadians (180);
			var obj = Content.LoadMultiple<Geometry> ("plane.obj");
			var rand = new Random ();
			foreach (Geometry geom in obj) {
				//geom.Material = new Material (new Color4((byte) rand.Next(0, 255), (byte) rand.Next (0, 255), (byte) rand.Next (0, 255), 255), testTexture, normal, 32, 16);
				geom.Material = new Material (Color4.White, null, null, 32, 16);
				models.Add (new Model (geom));
			}

			/*this.RenderingPipeline.AddDirectionalLight (new DirectionalLight {
				@base = new BaseLight {
					Color = new Vector3 (1f),
					Intensity = 1f,
				},
				direction = new Vector3 (0, 1, -1)
			});*/

			Func<Random, float, float, float> randPos = (rnd, min, max) => min + (float) rnd.NextDouble() * (max - min);

			for (int i = 0; i < 64; i++) {

				this.RenderingPipeline.AddPointLight (new PointLight {
					@base = new BaseLight {
						Color = new Vector3 ((float) rand.NextDouble(), (float) rand.NextDouble (), (float) rand.NextDouble ()),
						Intensity = .25f
					},
					atten = new Attenuation {
						constant = 2f,
						exponent = .5f,
						linear = 1f,
					},
					position = new Vector3 (randPos (rand, -5, 5), randPos(rand, .125f, .75f), randPos (rand, -5, 5)),
				});
			}

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
				GL.Clear (ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
				foreach (Model model in models)
					model.Draw (shader, camera.Camera);
			});

			base.Draw (time);
		}
	}
}
