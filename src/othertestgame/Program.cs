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
			new Vector3 (.0f, .5f, .0f),
			new Vector3 (.5f, -.5f, .0f),
			new Vector3 (-.5f, -.5f, .0f),
		};

		Color4[] colors = {
			new Color4 (1.0f, 0.0f, 0.0f, 1.0f),
			new Color4 (0.0f, 1.0f, 0.0f, 1.0f),
			new Color4 (0.0f, 0.0f, 1.0f, 1.0f),
		};

		GLBuffer<Vector3> vbo;
		GLBuffer<Color4> cbo;
		int abo = -1;

		ShaderProgram program;

		public TestGame (GameConfiguration conf) : base (conf) {
		}

		protected override void Initialize () {
			base.Initialize ();

			GraphicsContext.Assert ();

			vbo = new GLBuffer<Vector3> (BufferTarget.ArrayBuffer, points, BufferUsageHint.StaticDraw);
			cbo = new GLBuffer<Color4> (BufferTarget.ArrayBuffer, colors, BufferUsageHint.StaticDraw);

			var vertexShader = BasicShader.FromFile<VertexShader> ("shaders\\passthrough.vs");
			var fragmentShader = BasicShader.FromFile<FragmentShader> ("shaders\\passthrough.fs");
			program = new ShaderProgram (vertexShader, fragmentShader);

			program.Link ();

			abo = GL.GenVertexArray ();
			GL.BindVertexArray (abo);
			
			vbo.PointTo (program.Attrib ("v_pos"), VertexAttribPointerType.Float);
			cbo.PointTo (program.Attrib ("v_col"), VertexAttribPointerType.Float);
		}

		protected override void Update (GameTime time) {
			
			base.Update (time);
		}

		protected override void Draw (GameTime time) {
			GL.ClearColor (.4f, .4f, .4f, 1f);
			GL.Clear (ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			using (program.UseProgram ()) {
				GL.DrawArrays (PrimitiveType.Triangles, 0, 3);
			}

			base.Draw (time);
		}
	}
}
