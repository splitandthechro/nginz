using System;
using nginz.Common;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;

namespace nginz
{

	/// <summary>
	/// Sprite batch.
	/// </summary>
	public class SpriteBatch : ICanLog
	{
		const string VERTEXSHADER = @"
		#version 150

		in vec2 position;
		in vec3 color;
		in vec2 texcoord;
		out vec3 Color;
		out vec2 Texcoord;

		void main () {
			Color = color;
			Texcoord = texcoord;
			gl_Position = vec4 (position, 0.0, 1.0);
		}";

		const string FRAGMENTSHADER = @"
		#version 150

		in vec3 Color;
		in vec2 Texcoord;
		out vec4 outColor;
		uniform sampler2D tex;

		void main () {
			outColor = texture (tex, Texcoord) * vec4 (Color, 1.0);
		}";

		readonly float[] vertices;
		readonly int[] indices;
		readonly int vao;
		readonly int vbo;
		readonly int ibo;
		readonly VertexShader vertShader;
		readonly FragmentShader fragShader;
		readonly ShaderProgram program;
		readonly VertexAttribPointerType pointerType;

		public SpriteBatch () {
			
			// Initialize vertex pointer type
			pointerType = VertexAttribPointerType.Float;

			// Initialize vertex array object
			vao = GL.GenVertexArray ();
			GL.BindVertexArray (vao);

			// Initialize vertices
			vertices = new [] {

				// Layout: 2x position, 3x color, 2x texcoords

				// top left
				-0.5f, +0.5f, 1.0f, 0.0f, 0.0f, 0.0f, 0.0f,
				// top right
				+0.5f, +0.5f, 0.0f, 1.0f, 0.0f, 1.0f, 0.0f,
				// bottom left
				+0.5f, -0.5f, 0.0f, 0.0f, 1.0f, 1.0f, 1.0f,
				// bottom right
				-0.5f, -0.5f, 1.0f, 1.0f, 1.0f, 0.0f, 1.0f
			};

			// Initialize indices
			indices = new [] {
				0, 1, 2,
				2, 3, 0
			};

			// Initialize vertex buffer
			vbo = GL.GenBuffer ();
			GL.BindBuffer (BufferTarget.ArrayBuffer, vbo);
			GL.BufferData<float> (BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

			// Initialize index buffer
			ibo = GL.GenBuffer ();
			GL.BindBuffer (BufferTarget.ElementArrayBuffer, ibo);
			GL.BufferData<int> (BufferTarget.ElementArrayBuffer, indices.Length * sizeof(int), indices, BufferUsageHint.StaticDraw);

			// Initialize vertex shader
			vertShader = new VertexShader (VERTEXSHADER);

			// Initialize fragment shader
			fragShader = new FragmentShader (FRAGMENTSHADER);

			// Initialize shader program
			program = new ShaderProgram (vertShader, fragShader);
			program.BindFragDataLocation (0, "outColor");
			program.Link ();
		}

		public void Draw (Texture2D tex) {

			// Bind buffers
			//vbo.Bind ();
			//ibo.Bind ();

			// Use the shader program
			using (program) {
				
				var posAttrib = GL.GetAttribLocation (program.ProgramId, "position");
				this.Log ("posAttrib: {0}", posAttrib);
				GL.EnableVertexAttribArray (posAttrib);
				GL.VertexAttribPointer (
					index: posAttrib,
					size: 2,
					type: pointerType,
					normalized: false,
					stride: 7 * sizeof(float),
					offset: 0
				);

				var colorAttrib = GL.GetAttribLocation (program.ProgramId, "color");
				this.Log ("colorAttrib: {0}", colorAttrib);
				GL.EnableVertexAttribArray (colorAttrib);
				GL.VertexAttribPointer (
					index: colorAttrib,
					size: 3,
					type: pointerType,
					normalized: false,
					stride: 7 * sizeof(float),
					offset: 2 * sizeof(float)
				);

				var texAttrib = program.Attrib ("texcoord");
				this.Log ("texAttrib: {0}", texAttrib);
				GL.EnableVertexAttribArray (texAttrib);
				GL.VertexAttribPointer (
					index: texAttrib,
					size: 2,
					type: pointerType,
					normalized: false,
					stride: 7 * sizeof(float),
					offset: 5 * sizeof(float)
				);

				// Bind the texture
				tex.Bind ();
				GL.Uniform1 ((int) program ["tex"], 0);

				// Draw the texture
				GL.DrawElements (
					mode: BeginMode.Triangles,
					count: 6,
					type: DrawElementsType.UnsignedInt,
					offset: 0
				);

				// Unbind the texture
				tex.Unbind ();
			}

			// Unbind buffers
			//ibo.Unbind ();
			//vbo.Unbind ();
		}
	}
}

