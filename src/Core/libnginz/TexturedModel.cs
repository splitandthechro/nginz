using System;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace nginz
{
	/// <summary>
	/// Textured model.
	/// </summary>
	public class TexturedModel : Model
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="nginz.TexturedModel"/> class.
		/// </summary>
		/// <param name="geometry">Geometry.</param>
		public TexturedModel (Geometry geometry)
			: base (geometry) { }

		public TexturedModel (ObjFile objModel, int groupNum, ShaderProgram program)
			: base (objModel, groupNum, program) { }

		public TexturedModel (Model model)
			: base (model.Geometry) { }
			 
		/// <summary>
		/// Draw the model.
		/// </summary>
		/// <param name="program">Shader program.</param>
		/// <param name="camera">Camera.</param>
		/// <param name="texture">Texture.</param>
		public void Draw (ShaderProgram program, Camera camera, Texture2D texture) {

			// Bind the texture
			texture.Bind (TextureUnit.Texture0);

			// Set the texture uniform in the shader program
			program ["tex"] = 0;

			// Draw the geometry
			base.Draw (program, camera);

			// Unbind the texture
			texture.Unbind (TextureUnit.Texture0);
		}

		/// <summary>
		/// Draw the model.
		/// </summary>
		/// <param name="program">Shader program.</param>
		/// <param name="camera">Camera.</param>
		/// <param name="texture">Texture.</param>
		public void Draw (ShaderProgram program, Matrix4 VP, Texture2D texture) {

			// Bind the texture
			texture.Bind (TextureUnit.Texture0);

			// Set the texture uniform in the shader program
			program["tex"] = 0;

			// Draw the geometry
			base.Draw (program, VP);

			// Unbind the texture
			texture.Unbind (TextureUnit.Texture0);
		}
	}
}

