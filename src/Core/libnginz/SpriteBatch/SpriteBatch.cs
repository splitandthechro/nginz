using System;
using System.Drawing;
using nginz.Common;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;

namespace nginz
{

	/// <summary>
	/// Sprite batch.
	/// </summary>
	public partial class SpriteBatch : ICanThrow, IDisposable
	{

		/// <summary>
		/// The maximum batch count.
		/// </summary>
		const int MAX_BATCHES = 64 * 32;

		/// <summary>
		/// The maximum vertex count.
		/// </summary>
		const int MAX_VERTICES = MAX_BATCHES * 4;

		/// <summary>
		/// The maximum index count.
		/// </summary>
		const int MAX_INDICES = MAX_BATCHES * 6;

		/// <summary>
		/// The game.
		/// </summary>
		Game Game;

		/// <summary>
		/// The default texture.
		/// </summary>
		Texture2D Dot;

		/// <summary>
		/// The current texture.
		/// </summary>
		Texture2D CurrentTexture;

		/// <summary>
		/// The shader program.
		/// </summary>
		ShaderProgram Program;

		/// <summary>
		/// The internal camera.
		/// </summary>
		Camera InternalCamera;

		/// <summary>
		/// The current camera.
		/// </summary>
		Camera CurrentCamera;

		/// <summary>
		/// The vertices.
		/// </summary>
		Vertex2D[] Vertices;

		/// <summary>
		/// The vertex buffer object.
		/// </summary>
		GLBufferDynamic<Vertex2D> vbo;

		/// <summary>
		/// The index buffer object.
		/// </summary>
		GLBuffer<uint> ibo;

		/// <summary>
		/// The array buffer object.
		/// </summary>
		int abo = -1;

		/// <summary>
		/// The vertex count.
		/// </summary>
		int vertexCount;

		/// <summary>
		/// The index count.
		/// </summary>
		int indexCount;

		/// <summary>
		/// Whether the sprite batch is currently active.
		/// </summary>
		volatile bool active;

		/// <summary>
		/// Swap two vertices.
		/// </summary>
		/// <param name="a">The alpha component.</param>
		/// <param name="b">The blue component.</param>
		static void SwapVec (ref Vector2 a, ref Vector2 b) {
			var temp = a;
			a = b;
			b = temp;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="nginz.SpriteBatch"/> class.
		/// </summary>
		/// <param name = "game">The game.</param>
		/// <param name="shader">Shader.</param>
		public SpriteBatch (Game game, ShaderProgram shader = null) {

			// Set the game
			Game = game;

			// Set dot texture
			Dot = new Texture2D (TextureConfiguration.Nearest, 1, 1);
			Dot.SetData (new[] { Color4.White } , null, pixelType: PixelType.Float);

			// Compile predefined shaders if no shader program is given
			if (shader == null) {
				var vertShader = new VertexShader (vert_source);
				var fragShader = new FragmentShader (frag_source);
				Program = new ShaderProgram (vertShader, fragShader);
				Program.Link ();
			} else
				Program = shader;

			// Create the optimal buffer settings
			var settings = new GLBufferSettings {
				AttribSize = 0,
				Hint = BufferUsageHint.DynamicDraw,
				Normalized = false,
				Offset = 0,
				Target = BufferTarget.ArrayBuffer,
				Type = VertexAttribPointerType.Float
			};

			// Create temp variables for indices
			int indPtr = 0;
			var tempIndices = new uint[MAX_INDICES];

			// Fill temporary indices
			for (uint i = 0; i < MAX_VERTICES; i += 4) {
				
				// Triangle 1
				tempIndices [indPtr++] = i;
				tempIndices [indPtr++] = i + 1;
				tempIndices [indPtr++] = i + 2;

				// Triangle 2
				tempIndices [indPtr++] = i + 1;
				tempIndices [indPtr++] = i + 3;
				tempIndices [indPtr++] = i + 2;
			}

			// Set camera
			InternalCamera = new Camera (60f, game.Resolution, 0, 16, type: ProjectionType.Orthographic);

			// Set current texture
			CurrentTexture = Dot;

			// Generate array buffer object
			abo = GL.GenVertexArray ();

			// Create vertex buffer object
			vbo = new GLBufferDynamic<Vertex2D> (settings, Vertex2D.Size, MAX_VERTICES);

			// Create index buffer object
			ibo = new GLBuffer<uint> (GLBufferSettings.DynamicIndices, tempIndices);

			// Initialize vertices
			Vertices = new Vertex2D[MAX_VERTICES];
        }

		/// <summary>
		/// Begin batching sprites.
		/// </summary>
		public void Begin () {
			Begin (null);
		}

		/// <summary>
		/// Begin batching sprites.
		/// </summary>
		public void Begin (Camera camera) {

			// Throw if the sprite batch is active
			if (active)
				this.Throw ("Cannot begin an active sprite batch.");

			// Mark the sprite batch as active
			active = true;

			// Set the current camera
			CurrentCamera = camera ?? InternalCamera;

			// Reset the vertex and index counts
			vertexCount = 0;
			indexCount = 0;
		}

		/// <summary>
		/// End batching sprites.
		/// </summary>
		public void End () {

			// Throw if the sprite batch is inactive
			if (!active)
				this.Throw ("Cannot end an inactive sprite batch.");

			// Flush the sprites
			Flush ();

			// Mark the sprite batch as inactive
			active = false;
		}

		public void Draw (Texture2D texture, Rectangle? sourceRect, Rectangle destRect, Color4 color, Vector2 origin, Vector2 scale, int depth = 0, float rotation = 0) {

			// Draw the texture
			DrawInternal (
				texture: texture,
				sourceRect: sourceRect,
				destRect: destRect,
				color: color,
				scale: scale,
				dx: -(origin.X),
				dy: -(origin.Y),
				depth: depth,
				rot: rotation
			);
		}

		public void Draw (Texture2D texture, Rectangle? sourceRect, Vector2 position, Color4 color, float scale, int depth = 0, float rotation = 0) {
			var destRect = new Rectangle ((int) position.X, (int) position.Y, sourceRect.Value.Width, sourceRect.Value.Height);

			// Draw the texture
			DrawInternal (
				texture: texture,
				sourceRect: sourceRect,
				destRect: destRect,
				color: color,
				scale: new Vector2 (scale),
				dx: 0,
				dy: 0,
				depth: depth,
				rot: rotation
			);
		}

		public void Draw (Texture2D texture, Rectangle? sourceRect, Rectangle destRect, Color4 color, Vector2 origin, float scale, int depth = 0, float rotation = 0) {

			// Draw the texture
			DrawInternal (
				texture: texture,
				sourceRect: sourceRect,
				destRect: destRect,
				color: color,
				scale: new Vector2 (scale),
				dx: -(origin.X),
				dy: -(origin.Y),
				depth: depth,
				rot: rotation
			);
		}

		public void Draw (Texture2D texture, Rectangle? sourceRect, Vector2 position, Color4 color, Vector2 origin, Vector2 scale, int depth = 0, float rotation = 0) {

			// Draw the texture
			Draw (
				texture: texture,
				sourceRect: sourceRect,
				destRect: new Rectangle ((int) position.X, (int) position.Y, sourceRect.Value.Width, sourceRect.Value.Height),
				color: color,
				origin: origin,
				scale: scale,
				depth: depth,
				rotation: rotation
			);
		}

		public void Draw (Texture2D texture, Vector2 position, Color4 color, float scale, int depth = 0, float rotation = 0) {
			var destRect = new Rectangle ((int) position.X, (int) position.Y, texture.Width, texture.Height);

			// Draw the texture
			DrawInternal (
				texture: texture,
				sourceRect: null,
				destRect: destRect,
				color: color,
				scale: new Vector2 (scale),
				depth: depth,
				dx: 0,
				dy: 0,
				rot: rotation
			);
		}

		public void Draw (Texture2D texture, Vector2 position, Color4 color, Vector2 scale, int depth = 0) {

			// Draw the texture
			Draw (
				texture: texture,
				sourceRect: null,
				position: position,
				color: color,
				scale: scale,
				depth: depth
			);
		}

		public void Draw (Texture2D texture, Vector2 position, Color4 color, float scale, int depth = 0) {

			// Draw the texture
			Draw (
				texture: texture,
				sourceRect: null,
				position: position,
				color: color,
				scale: new Vector2 (scale),
				depth: depth
			);
		}

		public void Draw (Texture2D texture, Rectangle? sourceRect, Vector2 position, Color4 color, Vector2 scale, int depth = 0) {

			// Create destination rectangle from position and texture size
			var destRect = new Rectangle ((int) position.X, (int) position.Y, sourceRect.Value.Width, sourceRect.Value.Height);

			// Create source rectangle if the specified source rectangle is null
			if (sourceRect != null) {
				destRect.Width = sourceRect.Value.Width;
				destRect.Height = sourceRect.Value.Height;
			}

			// Draw the texture
			Draw (
				texture: texture,
				sourceRect: sourceRect,
				destRect: destRect,
				color: color,
				scale: scale,
				depth: depth
			);
		}

		public void Draw (Texture2D texture, Rectangle? sourceRect, Vector2 position, Color4 color, int depth = 0) {

			// Draw the texture
			Draw (
				texture: texture,
				sourceRect: sourceRect,
				position: position,
				color: color,
				scale: Vector2.One,
				depth: depth
			);
		}

		public void Draw (Texture2D texture, Rectangle? sourceRect, Rectangle destRect, Color4 color, int depth = 0) {

			// Draw the texture
			Draw (
				texture: texture,
				sourceRect: sourceRect,
				destRect: destRect,
				color: color,
				scale: Vector2.One,
				depth: depth
			);
		}

		public void Draw (Texture2D texture, Rectangle? sourceRect, Vector2 position, Color4 color, Vector2 scale, Vector2? origin = null, int depth = 0, float rotation = 0) {

			// Draw the texture
			Draw (
				texture: texture,
				sourceRect: sourceRect,
				destRect: new Rectangle ((int) position.X, (int) position.Y, sourceRect.Value.Width, sourceRect.Value.Height),
				color: color,
				origin: origin ?? Vector2.Zero,
				scale: scale,
				depth: depth,
				rotation: rotation
			);
		}

		public void Draw (Texture2D texture, Rectangle? sourceRect, Vector2 dest, Color4 color, Vector2 origin, int depth = 0, float rotation = 0) {

			// Draw the texture
			Draw (
				texture: texture,
				sourceRect: sourceRect,
				destRect: new Rectangle ((int) dest.X, (int) dest.Y, sourceRect.Value.Width, sourceRect.Value.Height),
				color: color,
				origin: origin,
				scale: Vector2.One,
				depth: depth,
				rotation: rotation
			);
		}

		public void Draw (Texture2D texture, Rectangle? sourceRect, Rectangle destRect, Color4 color, Vector2 origin, int depth = 0, float rotation = 0) {

			// Draw the texture
			DrawInternal (
				texture: texture,
				sourceRect: sourceRect,
				destRect: destRect,
				color: color,
				scale: Vector2.One,
				dx: -(origin.X),
				dy: -(origin.Y),
				depth: depth,
				rot: rotation
			);
		}
		public void Draw (Texture2D texture, Vector2 position, Color4 color, int depth = 0) {

			// Draw the texture
			Draw (
				texture: texture,
				sourceRect: null,
				position: position,
				color: color,
				scale: Vector2.One,
				depth: depth
			);
		}

		public void Draw (Texture2D texture, Rectangle? sourceRect, Rectangle destRect, Color4 color, Vector2 scale, float depth = 0, SpriteEffects effects = SpriteEffects.None) {

			// Flush if the current texture is valid
			// and the new texture differs from the current texture
			if (CurrentTexture.TextureId != -1 && texture.TextureId != CurrentTexture.TextureId)
				Flush ();

			// Set the current texture to the new texture
			CurrentTexture = texture;

			// Flush if the vertex or index counts exceeds the maximum
			if (indexCount + 6 >= MAX_INDICES || vertexCount + 4 >= MAX_VERTICES)
				Flush ();

			// Construct source rectangle
			Rectangle source = sourceRect ?? new Rectangle (0, 0, texture.Width, texture.Height);

			// Decompose destination rectangle
			float x = destRect.X;
			float y = destRect.Y;
			float w = destRect.Width * scale.X;
			float h = destRect.Height * scale.Y;

			// Decompose source rectangle
			float srcX = source.X;
			float srcY = source.Y;
			float srcW = source.Width;
			float srcH = source.Height;

			var topLeftCoord = new Vector2 (
				x: srcX / (float) texture.Width,
				y: srcY / (float) texture.Height);
			
			var topRightCoord = new Vector2 (
				x: (srcX + srcW) / (float) texture.Width,
				y: srcY / (float) texture.Height);
			
			var bottomLeftCoord = new Vector2 (
				x: srcX / (float) texture.Width,
				y: (srcY + srcH) / (float) texture.Height);
			
			var bottomRightCoord = new Vector2 (
				x: (srcX + srcW) / (float) texture.Width,
				y: (srcY + srcH) / (float) texture.Height);

			// Flip the texture horizontally if requested
			if (effects.HasFlag (SpriteEffects.FlipHorizontal)) {
				SwapVec (ref topLeftCoord, ref topRightCoord);
				SwapVec (ref bottomLeftCoord, ref bottomRightCoord);
			}

			// Flip the texture horizontally if requested
			if (effects.HasFlag (SpriteEffects.FlipVertical)) {
				SwapVec (ref topLeftCoord, ref bottomLeftCoord);
				SwapVec (ref topRightCoord, ref bottomRightCoord);
			}

			// Top left
			Vertices[vertexCount++] = new Vertex2D (
				new Vector3 (x, y, depth),
				topLeftCoord,
				color
			);

			// Top right
			Vertices[vertexCount++] = new Vertex2D (
				new Vector3 (x + w, y, depth),
				topRightCoord,
				color
			);

			// Bottom left
			Vertices[vertexCount++] = new Vertex2D (
				new Vector3 (x, y + h, depth),
				bottomLeftCoord,
				color
			);

			// Bottom right
			Vertices[vertexCount++] = new Vertex2D (
				new Vector3 (x + w, y + h, depth),
				bottomRightCoord,
				color
			);

			// Increment the index count
			indexCount += 6;
		}

		internal void DrawInternal (Texture2D texture, Rectangle? sourceRect, Rectangle destRect, Color4 color, Vector2 scale, float dx, float dy, float depth, float rot) {

			// Flush if the current texture is valid
			// and the new texture differs from the current texture
			if (CurrentTexture.TextureId != -1 && texture.TextureId != CurrentTexture.TextureId)
				Flush ();

			// Set the current texture to the new texture
			CurrentTexture = texture;

			// Flush if the vertex or index counts exceeds the maximum
			if (indexCount + 6 >= MAX_INDICES || vertexCount + 4 >= MAX_VERTICES)
				Flush ();

			// Construct source rectangle
			Rectangle source = sourceRect ?? new Rectangle (0, 0, texture.Width, texture.Height);

			var quat = new Quaternion (0, 0, rot);
			var pos = new Vector2 (destRect.X, destRect.Y);
			var size = new Vector2 (destRect.Width * scale.X, destRect.Height * scale.Y);
			Console.WriteLine (size);

			var tl = Vector2.Transform (new Vector2 (pos.X, destRect.Y), quat);
			var tr = Vector2.Transform (new Vector2 (pos.X + size.X, pos.Y), quat);
			var bl = Vector2.Transform (new Vector2 (pos.X, pos.Y + size.Y), quat);
			var br = Vector2.Transform (new Vector2 (pos.X + size.X, pos.Y + size.Y), quat);

			// Top left
			Vertices[vertexCount++] = new Vertex2D (
				pos: new Vector3 (
					tl.X + dx,
					tl.Y + dy,
					z: depth
				),
				texcoord:
				new Vector2 (
					x: source.X / (float) texture.Width,
					y: source.Y / (float) texture.Height
				),
				color: color
			);
			
			// Top right
			Vertices[vertexCount++] = new Vertex2D (
				pos: new Vector3 (
					tr.X + dx,
					tr.Y + dy,
					z: depth
				),
				texcoord: new Vector2 (
					x: (source.X + source.Width) / (float) texture.Width,
					y: source.Y / (float) texture.Height
				),
				color: color
			);
			
			// Bottom left
			Vertices[vertexCount++] = new Vertex2D (
				pos: new Vector3 (
					bl.X + dx,
					bl.Y + dy,
					z: depth
				),
				texcoord: new Vector2 (
					x: source.X / (float) texture.Width,
					y: (source.Y + source.Height) / (float) texture.Height
				),
				color: color
			);
			
			// Bottom right
			Vertices[vertexCount++] = new Vertex2D (
				pos: new Vector3 (
					br.X + dx,
					br.Y + dy,
					z: depth
				),
				texcoord: new Vector2 (
					x: (source.X + source.Width) / (float) texture.Width,
					y: (source.Y + source.Height) / (float) texture.Height
				),
				color: color
			);

			// Increment index count
			indexCount += 6;
		}

		public void Flush () {

			// Return if there's nothing to draw
			if (indexCount == 0)
				return;

			// Use the shader program
			Program.Use (() => {

				// Bind the array buffer object
				GL.BindVertexArray (abo);

				// Upload vertices to the vertex buffer object
				vbo.UploadData (dataArray:Vertices);

				// Point the vertex buffer object to the right point
				vbo.PointTo (Program.Attrib ("v_pos"), 2, 0);
				vbo.PointTo (Program.Attrib ("v_tex"), 2, 3 * sizeof (float));
				vbo.PointTo (Program.Attrib ("v_col"), 4, 5 * sizeof (float));

				// Bind the current texture to texture unit 0
				CurrentTexture.Bind (TextureUnit.Texture0);

				// Bind the index buffer object
				ibo.Bind ();

				// Set the MVP uniform to the view projection matrix of the camera
				Program ["MVP"] = CurrentCamera.ViewProjectionMatrix;

				// Draw the elements
				GL.DrawElements (BeginMode.Triangles, ibo.Buffer.Count, DrawElementsType.UnsignedInt, 0);

				// Unbind the array buffer object
				GL.BindVertexArray (0);

				Array.Clear (Vertices, 0, Vertices.Length);

				// Reset the vertex and index counts
				vertexCount = 0;
				indexCount = 0;
			});
		}

		#region IDisposable implementation

		/// <summary>
		/// Releases all resource used by the <see cref="nginz.SpriteBatch"/> object.
		/// </summary>
		/// <remarks>Call <see cref="Dispose"/> when you are finished using the <see cref="nginz.SpriteBatch"/>. The
		/// <see cref="Dispose"/> method leaves the <see cref="nginz.SpriteBatch"/> in an unusable state. After calling
		/// <see cref="Dispose"/>, you must release all references to the <see cref="nginz.SpriteBatch"/> so the garbage
		/// collector can reclaim the memory that the <see cref="nginz.SpriteBatch"/> was occupying.</remarks>
		public void Dispose () {
			Program.Dispose ();
		}

		#endregion
	}
}
