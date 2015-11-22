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

		int vbo = -1;
		int cbo = -1;
		int ibo = -1;
		int abo = -1;

		ShaderProgram program;

		public TestGame (GameConfiguration conf) : base (conf) {
		}

		protected override void Initialize () {
			base.Initialize ();

			GraphicsContext.Assert ();

			//vbo = new GLBuffer<Vector3> (BufferTarget.ArrayBuffer, points, BufferUsageHint.StaticDraw);
			//cbo = new GLBuffer<Color4> (BufferTarget.ArrayBuffer, colors, BufferUsageHint.StaticDraw);

			var vertexShader = BasicShader.FromFile<VertexShader> ("shaders\\passthrough.vs");
			var fragmentShader = BasicShader.FromFile<FragmentShader> ("shaders\\passthrough.fs");
			program = new ShaderProgram (vertexShader, fragmentShader);

			program.Link ();

			abo = GL.GenVertexArray ();
			GL.BindVertexArray (abo);

			vbo = GL.GenBuffer ();
			GL.BindBuffer (BufferTarget.ArrayBuffer, vbo);
			GL.BufferData (BufferTarget.ArrayBuffer, Vector3.SizeInBytes * points.Length, points, BufferUsageHint.StaticDraw);
			GL.EnableVertexAttribArray (0);
			GL.VertexAttribPointer (0, 3, VertexAttribPointerType.Float, false, Vector3.SizeInBytes, 0);
			cbo = GL.GenBuffer ();
			GL.BindBuffer (BufferTarget.ArrayBuffer, cbo);
			GL.BufferData (BufferTarget.ArrayBuffer, Vector3.SizeInBytes * colors.Length, colors, BufferUsageHint.StaticDraw);
			GL.EnableVertexAttribArray (1);
			GL.VertexAttribPointer (1, 3, VertexAttribPointerType.Float, false, Vector3.SizeInBytes, 0);

			ibo = GL.GenBuffer ();
			GL.BindBuffer (BufferTarget.ElementArrayBuffer, ibo);
			GL.BufferData (BufferTarget.ElementArrayBuffer, indices.Length * sizeof (uint), indices, BufferUsageHint.StaticDraw);

			GL.BindVertexArray (0);
		}

		protected override void Update (GameTime time) {
			
			base.Update (time);
		}

		protected override void Draw (GameTime time) {
			GL.ClearColor (.4f, .4f, .4f, 1f);
			GL.Clear (ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			using (program.UseProgram ()) {
				GL.BindVertexArray (abo);
				GL.BindBuffer (BufferTarget.ElementArrayBuffer, ibo);
				GL.DrawElements (BeginMode.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
				GL.BindVertexArray (0);
			}

			base.Draw (time);
		}
	}
}
