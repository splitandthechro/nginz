using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using nginz;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace particlestest {
	class MainGame : Game {
		GLBuffer<Vector3> positions;
		GLBuffer<Vector4> colors;
		GLBuffer<uint> indices;

		Vector3[] pos = new Vector3[] {
			new Vector3 (+1f, -1f, .0f), // 0
			new Vector3 (-1f, +1f, .0f), // 1
			new Vector3 (-1f, -1f, .0f), // 2
			new Vector3 (+1f, +1f, .0f), // 3
		};

		Vector4[] col = new Vector4[] {
			new Vector4 (1.0f), // 0
			new Vector4 (1.0f), // 1
			new Vector4 (1.0f), // 2
			new Vector4 (1.0f), // 3
		};

		ShaderProgram program;

		uint[] ind = {
			0, 1, 2, // left
			1, 0, 3, // right
		};

		int abo;

		Camera camera;

		public MainGame (GameConfiguration config)
			: base (config) { }

		protected override void Initialize () {
			Content.ContentRoot = "../../assets";

			var UITexture = Content.Load<Texture2D> ("TDUserInterface.png", TextureConfiguration.Nearest);

			program = Content.Load<ShaderProgram> ("particle");

			positions = new GLBuffer<Vector3> (GLBufferSettings.StaticDraw3FloatArray, pos);
			colors = new GLBuffer<Vector4> (GLBufferSettings.StaticDraw4FloatArray, col);
			indices = new GLBuffer<uint> (GLBufferSettings.StaticIndices, ind);

			abo = GL.GenVertexArray ();
			GL.BindVertexArray (abo);
			positions.PointTo (program.Attrib ("v_vert"));
			colors.PointTo (program.Attrib ("v_col"));

			camera = new Camera (60f, Resolution, 0.1f, 64f);

			camera.SetRelativePosition (new Vector3 (0, 0, 5));

			base.Initialize ();
		}

		protected override void Draw (GameTime time) {
			GL.ClearColor (.25f, .30f, .35f, 1f);
			GL.Clear (ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			program.Use (() => {
				GL.BindVertexArray (abo);
				indices.Bind ();
				program["VP"] = camera.ViewProjectionMatrix;
				//program["Right"] = camera.Right;
				GL.DrawElements (BeginMode.Triangles, indices.Buffer.Count, DrawElementsType.UnsignedInt, 0);
			});

			base.Draw (time);
		}
	}
}
