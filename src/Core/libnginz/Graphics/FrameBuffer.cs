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

		public Dictionary<FboAttachment, Texture2D> BufferTextures;
		public List<DrawBuffersEnum> Attachments;

		public int Width;
		public int Height;

		private Framebuffer (FramebufferTarget target) {
			this.FramebufferId = GL.GenFramebuffer ();
			this.Target = target;
		}

		public Framebuffer (FramebufferTarget target, int width, int height)
			: this (target) {
			this.Width = width;
			this.Height = height;

			this.BufferTextures = new Dictionary<FboAttachment, Texture2D> ();
			this.Attachments = new List<DrawBuffersEnum> ();
		}

		public Framebuffer AttachTexture (FboAttachment attachment, DrawBuffersEnum mode, PixelInternalFormat internalFormat, PixelFormat format, PixelType type, InterpolationMode interpolation) {
			this.Attachments.Add (mode);
			this.BufferTextures[attachment] = new Texture2D (TextureTarget.Texture2D, internalFormat, format, type, interpolation, false, this.Width, this.Height);
			this.Bind ();
			this.BufferTextures[attachment].Bind ();
			GL.FramebufferTexture (this.Target, (FramebufferAttachment) mode, this.BufferTextures[attachment].TextureId, 0);
			this.Unbind ();
			return this;
		}

		public Framebuffer AttachDepth (PixelInternalFormat internalFormat, PixelFormat format, PixelType type, InterpolationMode interpolation) {
			this.BufferTextures[FboAttachment.DepthAttachment] = new Texture2D (TextureTarget.Texture2D, internalFormat, format, type, interpolation, false, this.Width, this.Height);
			this.Bind ();
			this.BufferTextures[FboAttachment.DepthAttachment].Bind ();
			GL.FramebufferTexture (this.Target, FramebufferAttachment.DepthAttachment, this.BufferTextures[FboAttachment.DepthAttachment].TextureId, 0);
			this.Unbind ();
			return this;
		}

		public Framebuffer Construct () {
			this.Bind ();
			GL.DrawBuffers (this.Attachments.Count, this.Attachments.ToArray ());
			this.Unbind ();
			return this;
		}

		public static void Bind (Framebuffer @this) {
			GL.BindFramebuffer (@this.Target, @this.FramebufferId);
			GL.Viewport (0, 0, @this.Width, @this.Height);
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
