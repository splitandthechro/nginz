using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using nginz.Common;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;

namespace nginz {
	public partial class SpriteBatch : ICanThrow {
		Texture2D dot;
		ShaderProgram shader;
		Camera camera;
		Texture2D currentTexture;

		GLBufferDynamic<Vertex2D> verts;
		GLBuffer<uint> indices;
		Vertex2D[] vertices;

		int abo = -1;

		bool active = false;

		int vertexCount = 0;
		int indexCount = 0;

		const int maxBatches = 64 * 32;
		const int maxVertices = maxBatches * 4;
		const int maxIndices = maxBatches * 6;

		//Swap two vertices
		static void SwapVec (ref Vector2 a, ref Vector2 b) {
			var temp = a;
			a = b;
			b = temp;
		}

		public SpriteBatch (ShaderProgram shader = null, Camera camera = null) {
			dot = new Texture2D (TextureConfiguration.Nearest, 1, 1);
			dot.SetData (new[] { Color4.White } , null, pixelType: PixelType.Float);

			#region shader
			if (shader == null) {
				var vertShader = new VertexShader (vert_source);
				var fragShader = new FragmentShader (frag_source);
				this.shader = new ShaderProgram (vertShader, fragShader);
				this.shader.Link ();
			} else
				this.shader = shader;
			#endregion
			#region settings
			var settings = new GLBufferSettings {
				AttribSize = 0,
				Hint = BufferUsageHint.DynamicDraw,
				Normalized = false,
				Offset = 0,
				Target = BufferTarget.ArrayBuffer,
				Type = VertexAttribPointerType.Float
			};
			#endregion

			#region Indices setup
			int indPtr = 0;
			var indices = new uint[maxIndices];

			for (uint i = 0; i < maxVertices; i += 4) {
				// Triangle 1
				indices[indPtr++] = i;
				indices[indPtr++] = i + 1;
				indices[indPtr++] = i + 2;
				// Triangle 2
				indices[indPtr++] = i + 1;
				indices[indPtr++] = i + 3;
				indices[indPtr++] = i + 2;
			}
			#endregion

			this.indices = new GLBuffer<uint> (GLBufferSettings.DynamicIndices, indices);

			this.camera = camera ?? new Camera (60f, Game.Resolution, 0, 16, type: ProjectionType.Orthographic);

			currentTexture = dot;

			abo = GL.GenVertexArray ();

			verts = new GLBufferDynamic<Vertex2D> (settings, Vertex2D.Size, maxVertices);

			vertices = new Vertex2D[maxVertices];
        }

		public void Begin () {
			if (active)
				this.Throw ("Cannot begin an active sprite batch.");
			active = true;
			vertexCount = 0;
			indexCount = 0;
		}
		public void End () {
			if (!active)
				this.Throw ("Cannot end an inactive sprite batch.");
			Flush ();
			active = false;
		}

		public void Draw (Texture2D texture, Rectangle? sourceRect, Vector2 dest, Color4 color, Vector2 origin, Vector2 scale, int depth = 0, float rotation = 0) {
			Draw (texture, sourceRect, new Rectangle ((int) dest.X, (int) dest.Y, texture.Width, texture.Height), color, scale, origin, depth, rotation);
		}
		public void Draw (Texture2D texture, Rectangle? sourceRect, Rectangle destRect, Color4 color, Vector2 origin, Vector2 scale, int depth = 0, float rotation = 0) {
			DrawInternal (
				texture,
				sourceRect,
				destRect,
				color,
				scale,
				-(origin.X),
				-(origin.Y),
				depth,
				(float) Math.Sin (rotation),
				(float) Math.Cos (rotation));
		}
		public void Draw (Texture2D texture, Vector2 position, Color4 color, Vector2 scale, int depth = 0) {
			Draw (texture, null, position, color, scale: scale, depth: depth);
		}
		public void Draw (Texture2D texture, Rectangle? sourceRect, Vector2 position, Color4 color, Vector2 scale, int depth = 0) {
			var r = new Rectangle ((int) position.X, (int) position.Y, texture.Width, texture.Height);
			if (sourceRect != null) {
				r.Width = sourceRect.Value.Width;
				r.Height = sourceRect.Value.Height;
			}
			Draw (texture, sourceRect, r, color, scale: scale, depth: depth);
		}
		public void Draw (Texture2D texture, Rectangle? sourceRect, Rectangle destRect, Color4 color, int depth = 0) {
			Draw (texture, sourceRect, destRect, color, scale: Vector2.One, depth: depth, effects: SpriteEffects.None);
		}
		public void Draw (Texture2D texture, Rectangle? sourceRect, Vector2 dest, Color4 color, Vector2 origin, int depth = 0, float rotation = 0) {
			Draw (texture, sourceRect, new Rectangle ((int) dest.X, (int) dest.Y, texture.Width, texture.Height), color, origin, Vector2.One, depth, rotation);
		}
		public void Draw (Texture2D texture, Rectangle? sourceRect, Rectangle destRect, Color4 color, Vector2 origin, int depth = 0, float rotation = 0) {
			DrawInternal (
				texture,
				sourceRect,
				destRect,
				color,
				Vector2.One,
				-(origin.X),
				-(origin.Y),
				depth,
				(float) Math.Sin (rotation),
				(float) Math.Cos (rotation));
		}
		public void Draw (Texture2D texture, Vector2 position, Color4 color, int depth = 0) {
			Draw (texture, null, position, color, Vector2.One, depth);
		}
		public void Draw (Texture2D texture, Rectangle? sourceRect, Vector2 position, Color4 color, int depth = 0) {
			var r = new Rectangle ((int) position.X, (int) position.Y, texture.Width, texture.Height);
			if (sourceRect != null) {
				r.Width = sourceRect.Value.Width;
				r.Height = sourceRect.Value.Height;
			}
			Draw (texture, sourceRect, r, color, Vector2.One, depth);
		}

		public void Draw (Texture2D texture, Rectangle? sourceRect, Rectangle destRect, Color4 color, Vector2 scale, float depth = 0, SpriteEffects effects = SpriteEffects.None) {
			if (currentTexture.TextureId != -1 && texture.TextureId != currentTexture.TextureId)
				Flush ();
			currentTexture = texture;
			if (indexCount + 6 >= maxIndices || vertexCount + 4 >= maxVertices)
				Flush ();
			Rectangle source = sourceRect ?? new Rectangle (0, 0, texture.Width, texture.Height);

			float x = destRect.X;
			float y = destRect.Y;
			float w = destRect.Width * scale.X;
			float h = destRect.Height * scale.Y;
			float srcX = source.X;
			float srcY = source.Y;
			float srcW = source.Width;
			float srcH = source.Height;

			Vector2 topLeftCoord = new Vector2 (srcX / (float) texture.Width,
				srcY / (float) texture.Height);
			Vector2 topRightCoord = new Vector2 ((srcX + srcW) / (float) texture.Width,
				srcY / (float) texture.Height);
			Vector2 bottomLeftCoord = new Vector2 (srcX / (float) texture.Width,
				(srcY + srcH) / (float) texture.Height);
			Vector2 bottomRightCoord = new Vector2 ((srcX + srcW) / (float) texture.Width,
				(srcY + srcH) / (float) texture.Height);
			if ((effects & SpriteEffects.FlipHorizontal) == SpriteEffects.FlipHorizontal) {
				SwapVec (ref topLeftCoord, ref topRightCoord);
				SwapVec (ref bottomLeftCoord, ref bottomRightCoord);
			}
			if ((effects & SpriteEffects.FlipVertical) == SpriteEffects.FlipVertical) {
				SwapVec (ref topLeftCoord, ref bottomLeftCoord);
				SwapVec (ref topRightCoord, ref bottomRightCoord);
			}
			/* Top Left */
			vertices[vertexCount++] = new Vertex2D (
				new Vector3 (x, y, depth),
				topLeftCoord,
				color
			);
			/* Top Right */
			vertices[vertexCount++] = new Vertex2D (
				new Vector3 (x + w, y, depth),
				topRightCoord,
				color
			);
			/* Bottom Left */
			vertices[vertexCount++] = new Vertex2D (
				new Vector3 (x, y + h, depth),
				bottomLeftCoord,
				color
			);
			/* Bottom Right */
			vertices[vertexCount++] = new Vertex2D (
				new Vector3 (x + w, y + h, depth),
				bottomRightCoord,
				color
			);

			indexCount += 6;
		}

		void DrawInternal (Texture2D texture, Rectangle? sourceRect, Rectangle destRect, Color4 color, Vector2 scale, float dx, float dy, float depth, float sin, float cos) {
			if (currentTexture.TextureId != -1 && texture.TextureId != currentTexture.TextureId)
				Flush ();
			currentTexture = texture;
			if (indexCount + 6 >= maxIndices || vertexCount + 4 >= maxVertices)
				Flush ();
			Rectangle source = sourceRect ?? new Rectangle (0, 0, texture.Width, texture.Height);

			float x = destRect.X;
			float y = destRect.Y;
			float w = destRect.Width * scale.X;
			float h = destRect.Height * scale.Y;

			//Top Left
			vertices[vertexCount++] =
				new Vertex2D (
					new Vector3 (x + dx * cos - dy * sin,
						y + dx * sin + dy * cos,
						depth),
					new Vector2 (source.X / (float) texture.Width,
						source.Y / (float) texture.Height),
					color);
			//Top Right
			vertices[vertexCount++] =
				new Vertex2D (
					new Vector3 (x + (dx + w) * cos - dy * sin,
						y + (dx + w) * sin + dy * cos,
						depth),
					new Vector2 ((source.X + source.Width) / (float) texture.Width,
						source.Y / (float) texture.Height),
					color);
			//Bottom Left
			vertices[vertexCount++] =
				new Vertex2D (
					new Vector3 (x + dx * cos - (dy + h) * sin,
						y + dx * sin + (dy + h) * cos,
						depth),
					new Vector2 (source.X / (float) texture.Width,
						(source.Y + source.Height) / (float) texture.Height),
					color);
			//Bottom Right
			vertices[vertexCount++] =
				new Vertex2D (
					new Vector3 (x + (dx + w) * cos - (dy + h) * sin,
						y + (dx + w) * sin + (dy + h) * cos,
						depth),
					new Vector2 ((source.X + source.Width) / (float) texture.Width,
						(source.Y + source.Height) / (float) texture.Height),
					color);

			indexCount += 6;
		}

		public void Flush () {
			if (indexCount == 0)
				return;
			shader.Use (() => {
				GL.BindVertexArray (abo);

				verts.UploadData (vertices);
				verts.PointTo (this.shader.Attrib ("v_pos"), 2, 0);
				verts.PointTo (this.shader.Attrib ("v_tex"), 2, 3 * sizeof (float));
				verts.PointTo (this.shader.Attrib ("v_col"), 4, 5 * sizeof (float));

				currentTexture.Bind (TextureUnit.Texture0);

				indices.Bind ();
				shader["MVP"] = camera.ViewProjectionMatrix;
				GL.DrawElements (BeginMode.Triangles, indices.Buffer.Count, DrawElementsType.UnsignedInt, 0);

				GL.BindVertexArray (0);
				vertexCount = 0;
				indexCount = 0;
			});
		}

		[Flags]
		public enum SpriteEffects {
			None = 0,
			FlipHorizontal = 1,
			FlipVertical = 2
		}
	}
}
