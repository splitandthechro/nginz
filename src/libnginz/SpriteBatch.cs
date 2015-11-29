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
		readonly GLBuffer<float> vbo;
		readonly GLBuffer<int> ibo;
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
			vbo = new GLBuffer<float> (GLBufferSettings.StaticDraw2FloatArray, vertices);

			// Initialize index buffer
			ibo = new GLBuffer<int> (GLBufferSettings.Indices, indices);

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
			vbo.Bind ();
			ibo.Bind ();

			// Use the shader program
			program.Use (() => {

				// Get the attribute locations
				var posAttrib = program.Attrib ("position");
				var colorAttrib = program.Attrib ("color");
				var texAttrib = program.Attrib ("texcoord");

				glVertexAttribPointerEx (posAttrib, 2);
				glVertexAttribPointerEx (colorAttrib, 3, 2 * sizeof(float));
				glVertexAttribPointerEx (texAttrib, 2, 5 * sizeof(float));

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
			});

			// Unbind buffers
			ibo.Unbind ();
			vbo.Unbind ();
		}

		void glVertexAttribPointerEx (int index, int size, int offset = 0) {
			GL.EnableVertexAttribArray (index);
			GL.VertexAttribPointer (
				index: index,
				size: size,
				type: pointerType,
				normalized: false,
				stride: 7 * sizeof(float),
				offset: offset
			);
		}
	}
}

