using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace nginz.Graphics {
	public class Viewport {
		public Resolution Resolution {
			get {
				return new Resolution { Width = this.ViewportRect.Width, Height = this.ViewportRect.Height };
			}
			set {
				this.ViewportRect.Size = new Size (value.Width, value.Height);
			}
		}

		public Vector2 Position {
			get {
				return new Vector2 (ViewportRect.X, ViewportRect.Y);
			}
			set {
				this.ViewportRect.Location = new Point ((int) value.X, (int) value.Y);
			}
		}

		public Rectangle ViewportRect = Rectangle.Empty;

		private TexturedModel ViewportTarget;
		private ShaderProgram ViewportShader;

		private Matrix4 Matrix;

		public Viewport (Resolution resolution, Vector2? position = null, Camera camera = null) {
			this.Resolution = resolution;
			this.Position = position ?? Vector2.Zero;

			var pos = new Vector3[] {
				new Vector3 (this.Resolution.Width, 0, 0),
				new Vector3 (this.Resolution.Width, this.Resolution.Height, 0),
				new Vector3 (0, this.Resolution.Height, 0),
				new Vector3 (0, 0, 0),
			};

			var tex = new Vector2[] {
				new Vector2 (1.0f, 0.0f),
				new Vector2 (1.0f, 1.0f),
				new Vector2 (0.0f, 1.0f),
				new Vector2 (0.0f, 0.0f),
			};

			ViewportShader = Game.ContentManager.Load<ShaderProgram> ("viewport");

			this.ViewportTarget = new TexturedModel (new Geometry (BeginMode.Quads)
													.AddBuffer ("v_pos", new GLBuffer<Vector3> (GLBufferSettings.StaticDraw3FloatArray, pos))
													.AddBuffer ("v_tex", new GLBuffer<Vector2> (GLBufferSettings.StaticDraw2FloatArray, tex))
													.Construct (this.ViewportShader));

			Matrix = Matrix4.CreateOrthographicOffCenter (0, this.Resolution.Width, 0, this.Resolution.Height, 0, 16);
		}

		public void DrawTexture (Texture2D texture) {
			GL.Viewport (this.ViewportRect);
			this.ViewportShader.Use (() => {
				this.ViewportTarget.Position = new Vector3(this.Position.X, this.Position.Y, 0);
				this.ViewportTarget.Draw (this.ViewportShader, Matrix, texture);
			});
		}
	}
}
