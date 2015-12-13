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
		GLBuffer<Vector4> positions;
		GLBuffer<Vector2> texCoords;
		GLBuffer<Vector4> colors;

		Texture2D texture;

		Vector4[] pos = new Vector4[] {
			new Vector4 (-.5f, +.5f, .0f, 1f), // 1
			new Vector4 (-.5f, -.5f, .0f, 1f), // 2
			new Vector4 (+.5f, -.5f, .0f, 1f), // 0
			new Vector4 (+.5f, +.5f, .0f, 1f), // 3
		};

		Vector2[] textureCoords = new Vector2[] {
			new Vector2 (0, 0), // 1
			new Vector2 (0, 1), // 2
			new Vector2 (1, 1), // 0
			new Vector2 (1, 0), // 3
		};

		Vector4[] col = new Vector4[] {
			new Vector4 (1.0f), // 0
			new Vector4 (1.0f), // 1
			new Vector4 (1.0f), // 1
			new Vector4 (1.0f), // 1
		};

		ShaderProgram program;

		int abo;

		Camera camera;

		public MainGame (GameConfiguration config)
			: base (config) { }

		protected override void Initialize () {
			Content.ContentRoot = "../../assets";

			texture = Content.Load<Texture2D> ("particles_test.png", TextureConfiguration.Nearest);

			program = Content.Load<ShaderProgram> ("particle");

			positions = new GLBuffer<Vector4> (GLBufferSettings.StaticDraw4FloatArray, pos);
			texCoords = new GLBuffer<Vector2> (GLBufferSettings.StaticDraw2FloatArray, textureCoords);
			colors = new GLBuffer<Vector4> (GLBufferSettings.StaticDraw4FloatArray, col);

			abo = GL.GenVertexArray ();
			GL.BindVertexArray (abo);
			positions.PointTo (program.Attrib ("v_pos"));
			texCoords.PointTo (program.Attrib ("v_tex"));
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
				texture.Bind ();

				program["VP"] = camera.ViewProjectionMatrix;
				program["Right"] = camera.Right;
				program["Up"] = camera.Up;

				program["tex"] = 0;

				GL.DrawArrays (PrimitiveType.Quads, 0, 4);
			});

			base.Draw (time);
		}
	}
}
