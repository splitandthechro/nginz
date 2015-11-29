using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using GDIPixelFormat = System.Drawing.Imaging.PixelFormat;
using GLPixelFormat = OpenTK.Graphics.OpenGL4.PixelFormat;

namespace nginz
{

	/// <summary>
	/// Fontmap.
	/// </summary>
	public class Fontmap
	{

		/*
		 * IMPORTANT
		 * 
		 * This doesn't work yet.
		 * I had a working code but it is deprecated so
		 * we can't use it. I tried my best to implement
		 * it the right way using buffers and shaders and
		 * all that stuff, but I failed.
		 * 
		 * It's be very nice if you could fix this up.
		 * ~ Splitty
		 * 
		 */

		public ShaderProgram program;

		Font font;
		Texture2D texture;
		Bitmap bitmap;
		Matrix4 ortho;
		Geometry geometry;
		TexturedModel model;
		VertexShader vertexShader;
		FragmentShader fragmentShader;
		Camera camera;

		Vector2[] vao;
		GLBuffer<Vector2> vbo;
		uint[] iao;
		GLBuffer<uint> ibo;

		int areaWidth;
		int areaHeight;

		const string fragmentShaderSource = @"
		#version 450
		in vec3 f_col;
		in vec2 f_tex;
		uniform sampler2D tex;
		out vec4 frag_color;
		void main () {
			frag_color = vec4(f_col, 1.0) * texture(tex, f_tex);
		}";

		const string vertexShaderSource = @"
		#version 450
		in vec3 v_pos;
		in vec3 v_col;
		uniform mat4 MVP;
		out vec3 f_col;
		void main () {
			f_col = v_col;
			gl_Position = MVP * vec4 (v_pos, 1.0);
		}";

		/// <summary>
		/// Initializes a new instance of the <see cref="nginz.Fontmap"/> class.
		/// </summary>
		/// <param name="cam">Camera.</param>
		/// <param name="fontFamily">Font family.</param>
		/// <param name="emSize">Font size in em.</param>
		/// <param name="fontStyle">Font style.</param>
		public Fontmap (Camera cam, string fontFamily, float emSize, FontStyle fontStyle = 0x0) {

			// Set the camera
			camera = cam;

			// Set the graphics unit for the font
			var unit = GraphicsUnit.Pixel;

			// Create the font
			font = new Font (fontFamily, emSize, fontStyle, unit);

			// Load the font
			Load (cam.Resolution.Width, cam.Resolution.Height);
		}

		public void SetText (string text) {

			using (var graphics = Graphics.FromImage (bitmap)) {
				graphics.Clear (Color.Black);
				graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
				graphics.DrawString (text, font, Brushes.White, new Point (0, 0));
			}

			var bmpRect = new Rectangle (0, 0, areaWidth, areaHeight);
			var bmpData = bitmap.LockBits (bmpRect, ImageLockMode.ReadOnly, GDIPixelFormat.Format32bppArgb);

			// Create the texture
			GL.TexSubImage2D (
				target: TextureTarget.Texture2D,
				level: 0,
				xoffset: 0,
				yoffset: 0,
				width: bmpRect.Width,
				height: bmpRect.Height,
				format: GLPixelFormat.Bgra,
				type: PixelType.UnsignedByte,
				pixels: bmpData.Scan0
			);

			// Unlock the bitmap
			bitmap.UnlockBits (bmpData);
		}

		public void Draw () {

			// Draw the font
			model.Draw (program, camera, texture);
			
			/*
			 * The following is the code that was originally used
			 * to draw the font.
			 * 
			 * However, it's deprecated since OpenGL 3.3.
			 * Since we're using an OpenGL 4.5 core context here,
			 * we won't be able to use it.
			 * 
			 * I probably did a very bad job at trying to make this work.
			 * So please edit this to actually work lol.
			 * ~ Splitty
			 * 
			 */

			/*
			GL.PushMatrix ();
			GL.LoadIdentity ();
			GL.MatrixMode (MatrixMode.Projection);
			GL.PushMatrix ();
			GL.LoadMatrix (ref ortho);
			GL.Enable (EnableCap.Blend);
			GL.BlendFunc (BlendingFactorSrc.One, BlendingFactorDest.DstColor);
			GL.Enable (EnableCap.Texture2D);
			texture.Bind ();
			GL.Begin (PrimitiveType.Quads);
			GL.TexCoord2 (0, 0);
			GL.Vertex2 (0, 0);
			GL.TexCoord2 (1, 0);
			GL.Vertex2 (bitmap.Width, 0);
			GL.TexCoord2 (1, 1);
			GL.Vertex2 (bitmap.Width, bitmap.Height);
			GL.TexCoord2 (0, 1);
			GL.Vertex2 (0, bitmap.Height);
			GL.End ();
			GL.PopMatrix ();
			texture.Unbind ();
			GL.Disable (EnableCap.Blend);
			GL.Disable (EnableCap.Texture2D);
			GL.MatrixMode (MatrixMode.Modelview);
			GL.PopMatrix ();
			*/
		}

		void Load (int areawidth, int areaheight) {
			areaWidth = areawidth;
			areaHeight = areaheight;
			bitmap = new Bitmap (areawidth, areaheight);
			texture = new Texture2D (bitmap, TextureConfiguration.Nearest);
			ortho = Matrix4.CreateOrthographicOffCenter (0, areaWidth, areaHeight, 0, -1, 1);
			program = new ShaderProgram ();
			iao = new uint[] {
				0, 1, 2, // left
				1, 0, 3, // right
			};
			ibo = new GLBuffer<uint> (GLBufferSettings.StaticIndices, iao);
			vao = new [] {
				new Vector2 (0, 0),
				new Vector2 (1, 0),
				new Vector2 (1, 1),
				new Vector2 (0, 1),
			};
			var pao = new Vector3[] {
				new Vector3 (+1f, -1f, .0f), // 0
				new Vector3 (-1f, +1f, .0f), // 1
				new Vector3 (-1f, -1f, .0f), // 2
				new Vector3 (+1f, +1f, .0f), // 3
			};
			var pbo = new GLBuffer<Vector3> (GLBufferSettings.StaticDraw3FloatArray, pao);
			vbo = new GLBuffer<Vector2> (GLBufferSettings.StaticDraw2FloatArray, vao);
			vertexShader = new VertexShader (vertexShaderSource);
			fragmentShader = new FragmentShader (fragmentShaderSource);
			program = new ShaderProgram (vertexShader, fragmentShader);
			geometry = new Geometry (BeginMode.Quads);
			geometry.AddBuffer ("v_pos", pbo);
			geometry.AddBuffer ("v_tex", vbo);
			geometry.SetIndices (ibo);
			geometry.Construct (program);
			model = new TexturedModel (geometry);
		}
	}
}

