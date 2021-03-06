﻿using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using nginz.Common;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using GDIPixelFormat = System.Drawing.Imaging.PixelFormat;
using GLPixelFormat = OpenTK.Graphics.OpenGL4.PixelFormat;
using nginz.Interop.StbImage;
using OpenTK;

namespace nginz
{

	/// <summary>
	/// 2D Texture.
	/// </summary>
	public class Texture2D : ICanThrow, IBind, IAsset
	{

		/// <summary>
		/// The texture identifier.
		/// </summary>
		readonly public int TextureId;

		public int Width { get; private set; }
		public int Height { get; private set; }

		public Rectangle Bounds {
			get { return new Rectangle (0, 0, Width, Height); }
		}

		public static Texture2D Dot = new Texture2D (TextureConfiguration.LinearMipmap, 1, 1).SetData (new[] { Color4.White }, pixelType: PixelType.Float);

		public Texture2D (TextureTarget target, PixelInternalFormat internalFormat, GLPixelFormat format, PixelType type, InterpolationMode mode, bool mipmap, int width, int height) {
			Width = width;
			Height = height;

			// Get the texture id
			TextureId = GL.GenTexture ();

			// Bind the texture
			Bind (TextureUnit.Texture0);

			// Choose which filters to use
			TextureMinFilter minfilter = TextureMinFilter.Linear;
			TextureMagFilter magfilter = TextureMagFilter.Linear;
			switch (mode) {
				case InterpolationMode.Linear:
					minfilter = mipmap
						? TextureMinFilter.LinearMipmapLinear
						: TextureMinFilter.Linear;
					magfilter = TextureMagFilter.Linear;
					break;
				case InterpolationMode.Nearest:
					minfilter = mipmap
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

			// Create the texture
			GL.TexImage2D (
				target: target,
				level: 0,
				internalformat: internalFormat,
				width: Width,
				height: Height,
				border: 0,
				format: format,
				type: type,
				pixels: IntPtr.Zero
			);

			// Create a mipmap if requested
			if (mipmap)
				GL.GenerateMipmap (GenerateMipmapTarget.Texture2D);

			Unbind (TextureUnit.Texture0);
		}

		public Texture2D (TextureConfiguration config, int width, int height) 
			: this (TextureTarget.Texture2D, PixelInternalFormat.Rgba, GLPixelFormat.Bgra, PixelType.UnsignedByte, config.Interpolation, config.Mipmap, width, height) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="nginz.Texture2D"/> class.
		/// </summary>
		/// <param name="bmp">The bitmap.</param>
		/// <param name = "config">The configuration.</param>
		/// <param name = "preserveBitmap">Whether the bitmap should be disposed.</param>
		public Texture2D (Bitmap bmp, TextureConfiguration config, bool preserveBitmap = false) 
			: this (config, bmp.Width, bmp.Height) {

			// Update the texture data
			Update (bmp: bmp, preserveBitmap: preserveBitmap);
		}

		public void Update (Bitmap bmp, bool preserveBitmap = false) {

			// Build a rectangle representing the bitmap's size
			var rect = new Rectangle (0, 0, bmp.Width, bmp.Height);

			// Lock the bitmap
			var bmpData = bmp.LockBits (rect, ImageLockMode.ReadOnly, GDIPixelFormat.Format32bppArgb);

			// Bind the texture
			Bind (TextureUnit.Texture0);

			// Set the texture data
			SetData (bmpData.Scan0, rect, GLPixelFormat.Bgra);

			// Unlock the bitmap
			bmp.UnlockBits (bmpData);

			// Dispose the bitmap
			if (!preserveBitmap)
				bmp.Dispose ();

			// Unbind the texture
			Unbind (TextureUnit.Texture0);
		}

		public Texture2D SetData (IntPtr data, Rectangle? rect = null, GLPixelFormat pixelFormat = GLPixelFormat.Rgba, PixelType pixelType = PixelType.UnsignedByte) {
			Rectangle r = rect ?? new Rectangle (0, 0, Width, Height);
			Bind (TextureUnit.Texture0);
			GL.TexSubImage2D (
				target: TextureTarget.Texture2D,
				level: 0,
				xoffset: 0,
				yoffset: 0,
				width: r.Width,
				height: r.Height,
				format: pixelFormat,
				type: pixelType,
				pixels: data
			);
			Unbind (TextureUnit.Texture0);

			return this;
		}

		public Texture2D SetData<T> (T[] data, Rectangle? rect = null, GLPixelFormat pixelFormat = GLPixelFormat.Rgba, PixelType pixelType = PixelType.UnsignedByte) where T: struct {
			Rectangle r = rect ?? new Rectangle (0, 0, Width, Height);
			Bind (TextureUnit.Texture0);
			GL.TexSubImage2D (
				target: TextureTarget.Texture2D,
				level: 0,
				xoffset: r.X,
				yoffset: r.Y,
				width: r.Width,
				height: r.Height,
				format: pixelFormat,
				type: pixelType,
				pixels: data
			);
			Unbind (TextureUnit.Texture0);

			return this;
		}

		/// <summary>
		/// Load a Texture2 from a file.
		/// </summary>
		/// <returns>The Texture2D.</returns>
		/// <param name="path">Path to the texture.</param>
		/// <param name = "config">The texture configuration.</param>
		public static Texture2D FromFile (string path, TextureConfiguration config) {

			// Throw if the file doesn't exist
			if (!File.Exists (path))
				LogExtensions.ThrowStatic ("Could not find file '{0}'", path);

			Texture2D tex;

			// Load the image
			int x = -1, y = -1, n = -1;
			var data = Stb.Load (path, ref x, ref y, ref n, 4);
			tex = new Texture2D (config, x, y);
			tex.SetData (data, null);
			Stb.Free (data);
			// Return the texture
			return tex;
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

