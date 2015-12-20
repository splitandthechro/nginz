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
		public ShaderProgram AmbientShader;
		public ShaderProgram DirectionalShader;

		private Game game;

		DirectionalLight light = new DirectionalLight {
			@base = new BaseLight {
				Color = new Vector3 (1),
				Intensity = .75f,
			},
			direction = new Vector3 (1, 1, 1)
		};

		public Framebuffer Framebuffer;

		public Vector3 AmbientColor { get; set; }

		public RenderingPipeline (Game game) {
			this.game = game;

			Framebuffer = new Framebuffer (FramebufferTarget.Framebuffer, this.game.Configuration.Width, this.game.Configuration.Height);

			AmbientShader = this.game.Content.Load<ShaderProgram> ("forwardAmbient");
			DirectionalShader = this.game.Content.Load<ShaderProgram> ("forwardDirectional");
			this.AmbientColor = new Vector3 (.25f, .25f, .25f);
		}

		public void Draw (Camera camera, Action<ShaderProgram> draw) {
			this.Framebuffer.Bind ();
			GL.Clear (ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			this.AmbientShader["ambient_color"] = this.AmbientColor;

			this.AmbientShader.Use (draw);

			GL.Enable (EnableCap.Blend);
			GL.BlendFunc (BlendingFactorSrc.One, BlendingFactorDest.One);
			GL.DepthMask (false);
			GL.DepthFunc (DepthFunction.Equal);

			this.DirectionalShader["eye_pos"] = camera.Position;
			this.DirectionalShader.SetDirectionalLight ("directionalLight", light);
			this.DirectionalShader.Use (draw);

			GL.DepthFunc (DepthFunction.Less);
			GL.DepthMask (true);
			GL.Disable (EnableCap.Blend);
			this.Framebuffer.Unbind ();
		}

		public void Display (Viewport viewport = null) {
			var vp = viewport ?? game.MainViewport;
			this.Framebuffer.ColorTexture.Bind ();
			vp.DrawTexture (this.Framebuffer.ColorTexture);
		}
	}
}
