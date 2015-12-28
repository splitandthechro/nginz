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
		public ShaderProgram DeferredDirectional;
		public ShaderProgram DeferredPoint;
		public ShaderProgram AmbientShader;

		private Game game;

		public GBuffer GBuffer;
		public Framebuffer Framebuffer;

		public List<DirectionalLight> DirectionalLights = new List<DirectionalLight> ();
		public List<PointLight> PointLights = new List<PointLight> ();

		public Vector3 AmbientColor { get; set; }

		public RenderingPipeline (Game game) {
			this.game = game;

			this.GBuffer = new GBuffer (game, game.Configuration.Width * 2, game.Configuration.Height * 2);

			Framebuffer = new Framebuffer (FramebufferTarget.Framebuffer, game.Configuration.Width, game.Configuration.Height)
				.AttachTexture (FboAttachment.DiffuseAttachment, DrawBuffersEnum.ColorAttachment0, PixelInternalFormat.Rgb10A2, PixelFormat.Rgb, PixelType.UnsignedByte, InterpolationMode.Linear)
				.Construct ();

			DeferredDirectional = game.Content.Load<ShaderProgram> ("deferredDirectional");
			DeferredPoint = game.Content.Load<ShaderProgram> ("deferredPoint");
			AmbientShader = game.Content.Load<ShaderProgram> ("deferredAmbient");

			this.AmbientColor = new Vector3 (.25f, .25f, .25f);
		}

		public void AddDirectionalLight (DirectionalLight light) {
			this.DirectionalLights.Add (light);
		}

		public void AddPointLight (PointLight light) {
			this.PointLights.Add (light);
		}

		public void Draw (Camera camera, Action<ShaderProgram> draw, Viewport viewport = null) {
			var vp = viewport ?? game.MainViewport;

			GBuffer.Use (shader => {
				shader.Use (draw);
			});


			Framebuffer.Bind ();
			GBuffer.fbo.BufferTextures[FboAttachment.DiffuseAttachment].Bind (TextureUnit.Texture0);
			GBuffer.fbo.BufferTextures[FboAttachment.NormalAttachment].Bind (TextureUnit.Texture1);
			GBuffer.fbo.BufferTextures[FboAttachment.SpecularAttachment].Bind (TextureUnit.Texture2);
			GBuffer.fbo.BufferTextures[FboAttachment.DepthAttachment].Bind (TextureUnit.Texture3);

			this.AmbientShader["u_diffuse"] = 0;

			this.AmbientShader["ambient_color"] = this.AmbientColor;
			vp.Draw (this.AmbientShader);

			GL.Enable (EnableCap.Blend);
			GL.BlendFunc (BlendingFactorSrc.One, BlendingFactorDest.One);

			if (DirectionalLights.Count >= 1) {
				this.DeferredDirectional["u_diffuse"] = 0;
				this.DeferredDirectional["u_normal"] = 1;
				this.DeferredDirectional["u_specular"] = 2;
				this.DeferredDirectional["u_depth"] = 3;
				this.DeferredDirectional["inverseCamera"] = camera.ViewProjectionMatrix.Inverted ();
				this.DeferredDirectional["eye_pos"] = camera.Position;

				foreach (DirectionalLight light in this.DirectionalLights) {
					this.DeferredDirectional.SetDirectionalLight ("directionalLight", light);

					vp.Draw (this.DeferredDirectional);
				}
			}

			if (PointLights.Count >= 1) {
				this.DeferredPoint["u_diffuse"] = 0;
				this.DeferredPoint["u_normal"] = 1;
				this.DeferredPoint["u_specular"] = 2;
				this.DeferredPoint["u_depth"] = 3;
				this.DeferredPoint["inverseCamera"] = camera.ViewProjectionMatrix.Inverted ();
				this.DeferredPoint["eye_pos"] = camera.Position;

				foreach (PointLight light in this.PointLights) {
					this.DeferredPoint.SetPointLight ("pointLight", light);

					vp.Draw (this.DeferredPoint);
				}
			}

			GL.Disable (EnableCap.Blend);
			this.Framebuffer.Unbind ();

			vp.DrawTexture (Framebuffer.BufferTextures[FboAttachment.DiffuseAttachment]);
		}
	}
}
