using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using nginz.Common;
using OpenTK.Graphics.OpenGL4;

namespace nginz.Graphics {
	public class Framebuffer : IBind {
		public int FramebufferId;

		public FramebufferTarget Target;

		public Texture2D ColorTexture;
		public Texture2D DepthTexture;

		public int Width;
		public int Height;

		private Framebuffer (FramebufferTarget target) {
			this.FramebufferId = GL.GenFramebuffer ();
			this.Target = target;
		}

		public Framebuffer (FramebufferTarget target, int width, int height)
			: this (target) {
			this.Bind ();

			this.Width = width;
			this.Height = height;

			this.ColorTexture = new Texture2D (TextureTarget.Texture2D, PixelInternalFormat.Rgba, PixelFormat.Rgba, PixelType.UnsignedByte, InterpolationMode.Nearest, false, this.Width, this.Height);
			this.DepthTexture = new Texture2D (TextureTarget.Texture2D, PixelInternalFormat.DepthComponent24, PixelFormat.DepthComponent, PixelType.Float, InterpolationMode.Nearest, false, this.Width, this.Height);

			this.ColorTexture.Bind ();
			GL.FramebufferTexture (FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, this.ColorTexture.TextureId, 0);
			GL.FramebufferTexture (FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, this.DepthTexture.TextureId, 0);

			GL.DrawBuffer (DrawBufferMode.ColorAttachment0);

			this.Unbind ();
		}

		public static void Bind (Framebuffer @this) {
			GL.BindFramebuffer (@this.Target, @this.FramebufferId);
		}

		public static void Unbind (Framebuffer @this) {
			GL.BindFramebuffer (@this.Target, 0);
		}

		public void Bind () {
			Framebuffer.Bind (this);
		}

		public void Unbind () {
			Framebuffer.Unbind (this);
		}
	}
}
