using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using nginz.Common;
using OpenTK.Graphics.OpenGL4;
using GDIPixelFormat = System.Drawing.Imaging.PixelFormat;
using GLPixelFormat = OpenTK.Graphics.OpenGL4.PixelFormat;

namespace nginz
{

	/// <summary>
	/// 2D Texture.
	/// </summary>
	public class Texture2D : ICanThrow, IBind, Asset
	{

		/// <summary>
		/// The texture identifier.
		/// </summary>
		readonly public int TextureId;

		/// <summary>
		/// Initializes a new instance of the <see cref="nginz.Texture2D"/> class.
		/// </summary>
		/// <param name="bmp">The bitmap.</param>
		/// <param name = "mipmapped">Whether the texture uses mipmapping.</param>
		/// <param name = "interpolation">Interpolation mode.</param>
		public Texture2D (Bitmap bmp, bool mipmapped = false,
			InterpolationMode interpolation = InterpolationMode.Linear) {

			// Get the texture id
			TextureId = GL.GenTexture ();

			// Bind the texture
			Bind (TextureUnit.Texture0);

			// Choose which filters to use
			TextureMinFilter minfilter = TextureMinFilter.Linear;
			TextureMagFilter magfilter = TextureMagFilter.Linear;
			switch (interpolation) {
			case InterpolationMode.Linear:
				minfilter = mipmapped
					? TextureMinFilter.LinearMipmapLinear
					: TextureMinFilter.Linear;
				magfilter = TextureMagFilter.Linear;
				break;
			case InterpolationMode.Nearest:
				minfilter = mipmapped
					? TextureMinFilter.LinearMipmapNearest
					: TextureMinFilter.Nearest;
				magfilter = TextureMagFilter.Nearest;
				break;
			}

			// Set the min filter parameter
			GL.TexParameter (
				target: TextureTarget.Texture2D,
				pname: TextureParameterName.TextureMinFilter,
				param: (int) minfilter
			);

			// Set the mag filter parameter
			GL.TexParameter (
				target: TextureTarget.Texture2D,
				pname: TextureParameterName.TextureMagFilter,
				param: (int) magfilter
			);

			// Create a mipmap if requested
			if (mipmapped)
				GL.GenerateMipmap (GenerateMipmapTarget.Texture2D);

			// Build a rectangle representing the bitmap's size
			var rect = new Rectangle (0, 0, bmp.Width, bmp.Height);

			// Lock the bitmap
			var bmpData = bmp.LockBits (rect, ImageLockMode.ReadOnly, GDIPixelFormat.Format32bppArgb);

			// Create the texture
			GL.TexImage2D (
				target: TextureTarget.Texture2D,
				level: 0,
				internalformat: PixelInternalFormat.Rgba,
				width: rect.Width,
				height: rect.Height,
				border: 0,
				format: GLPixelFormat.Bgra,
				type: PixelType.UnsignedByte,
				pixels: bmpData.Scan0
			);

			// Unlock the bitmap
			bmp.UnlockBits (bmpData);

			// Unbind the texture
			Unbind (TextureUnit.Texture0);
		}

		/// <summary>
		/// Load a Texture2 from a file.
		/// </summary>
		/// <returns>The Texture2D.</returns>
		/// <param name="path">Path to the texture.</param>
		public static Texture2D FromFile (string path) {

			// Throw if the file doesn't exist
			if (!File.Exists (path))
				LogExtensions.ThrowStatic ("Could not find file '{0}'", path);

			// Load the image
			var img = Image.FromFile (path);

			// Load the image as bitmap
			var bmp = new Bitmap (img);

			// Return the texture
			return new Texture2D (bmp);
		}

		/// <summary>
		/// Bind the texture.
		/// </summary>
		public void Bind () {

			// Bind the texture to texture unit 0
			Bind (TextureUnit.Texture0);
		}

		/// <summary>
		/// Bind the texture.
		/// </summary>
		public void Bind (TextureUnit unit) {

			// Make the texture unit the active one
			GL.ActiveTexture (unit);

			// Bind the texture
			GL.BindTexture (TextureTarget.Texture2D, TextureId);
		}

		/// <summary>
		/// Unbind the texture.
		/// </summary>
		public void Unbind () {

			// Unbind the texture value from texture unit 0
			Unbind (TextureUnit.Texture0);
		}

		/// <summary>
		/// Unbind the texture.
		/// </summary>
		public void Unbind (TextureUnit unit) {

			// Make the texture unit the active one
			GL.ActiveTexture (unit);

			// Unbind the texture
			GL.BindTexture (TextureTarget.Texture2D, 0);
		}
	}
}

