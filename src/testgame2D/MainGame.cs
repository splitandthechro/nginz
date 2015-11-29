using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using nginz;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;

namespace testgame2D {
	class MainGame : Game {
		GLBufferDynamic<Vector3> pos;
		GLBufferDynamic<Vector3> col;
		GLBufferDynamic<uint> ind;

		int abo = -1;

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

		ShaderProgram shader;

		public MainGame (GameConfiguration conf) 
			: base (conf) { }

		protected override void Initialize () {
			base.Initialize ();

			pos = new GLBufferDynamic<Vector3> (GLBufferSettings.StreamDraw3FloatArray, Vector3.SizeInBytes, 4 * Vector3.SizeInBytes);
			col = new GLBufferDynamic<Vector3> (GLBufferSettings.StreamDraw3FloatArray, Vector3.SizeInBytes, 4 * Vector3.SizeInBytes);
			ind = new GLBufferDynamic<uint> (GLBufferSettings.StreamIndices, sizeof (uint), 6 * sizeof (uint));

			shader = Content.Load<ShaderProgram> ("passthrough");

			abo = GL.GenVertexArray ();
		}

		protected override void Resize (Resolution resolution) {
			base.Resize (resolution);
		}

		protected override void Update (GameTime time) {

			// Exit if escape is pressed
			if (Keyboard.IsKeyTyped (Key.Escape))
				Exit ();

			base.Update (time);
		}

		protected override void Draw (GameTime time) {
			GL.ClearColor (.25f, .30f, .35f, 1f);
			GL.Clear (ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			shader.Use (() => {
				GL.BindVertexArray (abo);
				pos.UploadData (points);
				col.UploadData (colors);
				ind.UploadData (indices);

				pos.PointTo (shader.Attrib ("v_pos"));
				col.PointTo (shader.Attrib ("v_col"));

				ind.Bind ();
				GL.DrawElements (BeginMode.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);

				GL.BindVertexArray (0);
			});

			base.Draw (time);
		}
	}
}
