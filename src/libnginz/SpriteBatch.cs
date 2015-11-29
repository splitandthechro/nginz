using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;

namespace nginz
{

	/// <summary>
	/// Sprite batch.
	/// </summary>
	public class SpriteBatch
	{
		const string VERTEXSHADER = @"
		#version 150 core

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
		#version 150 core

		in vec3 Color;
		in vec2 Texcoord;
		out vec4 outColor;
		uniform sampler2D tex;

		void main () {
			outColor = texture (tex, Texcoord);
		}";

		readonly float[] vertices;
		readonly int[] indices;
		readonly GLBuffer<float> vbo;
		readonly GLBuffer<int> ibo;
		readonly VertexShader vertShader;
		readonly FragmentShader fragShader;
		readonly ShaderProgram program;
		readonly VertexAttribPointerType pointerType;

		public SpriteBatch () {
			
			// Initialize vertex pointer type
			pointerType = VertexAttribPointerType.Float;

			// Initialize vertices
			vertices = new [] {
				
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
			var vertexSettings = GLBufferSettings.StaticDraw2FloatArray;
			vbo = new GLBuffer<float> (vertexSettings, vertices);

			// Initialize index buffer
			var indexSettings = GLBufferSettings.Indices;
			ibo = new GLBuffer<int> (indexSettings, indices);

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
			using (program) {

				// Get the attributes
				var posAttrib = program.Attrib ("position");
				var colorAttrib = program.Attrib ("color");
				var texAttrib = program.Attrib ("texcoord"); 

				// Setup the vertex attrib pointers
				VertexAttribPointer (index: posAttrib, size: 2);
				VertexAttribPointer (index: colorAttrib, size: 3, offset: 2 * sizeof(float));
				VertexAttribPointer (index: texAttrib, size: 2, offset: 5 * sizeof (float));

				// Bind the texture
				tex.Bind ();
				GL.Uniform1 ((int) program ["tex"], 1);

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
			ibo.Unbind ();
			vbo.Unbind ();
		}

		void VertexAttribPointer (int index, int size, int offset = 0) {
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

