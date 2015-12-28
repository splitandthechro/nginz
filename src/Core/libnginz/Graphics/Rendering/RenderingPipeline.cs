using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using nginz.Graphics;
using nginz.Lighting;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;

namespace nginz.Rendering {
	public class RenderingPipeline {
		public ShaderProgram LightingPass;
		public ShaderProgram AmbientShader;

		private Game game;

		public GBuffer GBuffer;
		public Framebuffer Framebuffer;

		public List<DirectionalLight> DirectionalLights = new List<DirectionalLight> ();

		public Vector3 AmbientColor { get; set; }

		public RenderingPipeline (Game game) {
			this.game = game;

			this.GBuffer = new GBuffer (game, game.Configuration.Width, game.Configuration.Height);

			Framebuffer = new Framebuffer (FramebufferTarget.Framebuffer, game.Configuration.Width, game.Configuration.Height)
				.AttachTexture (FboAttachment.DiffuseAttachment, DrawBuffersEnum.ColorAttachment0, PixelInternalFormat.Rgb10A2, PixelFormat.Rgb, PixelType.UnsignedByte, InterpolationMode.Linear)
				.Construct ();

			LightingPass = game.Content.Load<ShaderProgram> ("lightingPass");
			AmbientShader = game.Content.Load<ShaderProgram> ("deferredAmbient");

			this.AmbientColor = new Vector3 (.25f, .25f, .25f);
		}

		public void AddDirectionalLight (DirectionalLight light) {
			this.DirectionalLights.Add (light);
		}

		public void Draw (Camera camera, Action<ShaderProgram> draw, Viewport viewport = null) {
			var vp = viewport ?? game.MainViewport;

			GBuffer.Use (shader => {
				GL.Clear (ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
				shader.Use (draw);
			});


			Framebuffer.Bind ();
			GBuffer.fbo.BufferTextures[FboAttachment.DiffuseAttachment].Bind (TextureUnit.Texture0);
			this.AmbientShader["u_diffuse"] = 0;

			this.AmbientShader["ambient_color"] = this.AmbientColor;
			vp.Draw (this.AmbientShader);

			GL.Enable (EnableCap.Blend);
			GL.BlendFunc (BlendingFactorSrc.One, BlendingFactorDest.One);
			GL.DepthMask (false);

			GBuffer.fbo.BufferTextures[FboAttachment.DiffuseAttachment].Bind (TextureUnit.Texture0);
			GBuffer.fbo.BufferTextures[FboAttachment.NormalAttachment].Bind (TextureUnit.Texture1);
			GBuffer.fbo.BufferTextures[FboAttachment.SpecularAttachment].Bind (TextureUnit.Texture2);
			GBuffer.fbo.BufferTextures[FboAttachment.DepthAttachment].Bind (TextureUnit.Texture3);

			this.LightingPass["u_diffuse"] = 0;
			this.LightingPass["u_normal"] = 1;
			this.LightingPass["u_specular"] = 2;
			this.LightingPass["u_depth"] = 3;

			this.LightingPass["inverseCamera"] = camera.ViewProjectionMatrix.Inverted ();

			this.LightingPass["eye_pos"] = camera.Position;

			this.LightingPass.SetDirectionalLight ("directionalLight", this.DirectionalLights[0]);

			vp.Draw (this.LightingPass);
			
			GL.DepthMask (true);
			GL.Disable (EnableCap.Blend);
			this.Framebuffer.Unbind ();

			vp.DrawTexture (Framebuffer.BufferTextures[FboAttachment.DiffuseAttachment]);
		}
	}
}
