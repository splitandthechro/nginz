using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK.Graphics.OpenGL4;

namespace nginz.Graphics {
	public class GBuffer {
		public Framebuffer fbo;

		public ShaderProgram GeometryPass;

		public GBuffer (Game game, int width, int height) {
			this.fbo = new Framebuffer (FramebufferTarget.Framebuffer, width, height)
				.AttachTexture (FboAttachment.DiffuseAttachment, DrawBuffersEnum.ColorAttachment0, PixelInternalFormat.Srgb, PixelFormat.Rgb, PixelType.UnsignedByte, InterpolationMode.Linear)
				.AttachTexture (FboAttachment.NormalAttachment, DrawBuffersEnum.ColorAttachment1, PixelInternalFormat.Rgb16f, PixelFormat.Rgb, PixelType.Float, InterpolationMode.Linear)
				.AttachTexture (FboAttachment.SpecularAttachment, DrawBuffersEnum.ColorAttachment2, PixelInternalFormat.Rgb10A2, PixelFormat.Rgb, PixelType.UnsignedByte, InterpolationMode.Linear)
				.AttachDepth (PixelInternalFormat.DepthComponent32, PixelFormat.DepthComponent, PixelType.Float, InterpolationMode.Linear)
				.Construct ();

			GeometryPass = game.Content.Load<ShaderProgram> ("geometryPass");
		}

		public void Use (Action<ShaderProgram> draw) {
			this.fbo.Bind ();
			draw (GeometryPass);
		}
	}
}
