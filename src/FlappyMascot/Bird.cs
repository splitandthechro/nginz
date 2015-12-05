using System;
using nginz;
using nginz.Common;
using OpenTK;
using OpenTK.Input;
using OpenTK.Graphics;

namespace FlappyMascot
{
	public class Bird : IUpdatable, IDrawable2D
	{
		readonly Game game;
		Texture2D[] tex;
		float ypos, ydelta;
		const float drag = 250;
		bool mouseDown;
		int currentAnimation;
		float animationDelta;

		public Bird (Game game) {
			tex = new Texture2D[4];
			for (var i = 0; i < tex.Length; i++)
				tex [i] = game.Content.Load<Texture2D> (string.Format ("anim/flappymascot_char_anim{0}.png", i), TextureConfiguration.Nearest);
			this.game = game;
			ypos = (Game.Resolution.Height / 2) + (tex [0].Height / 2);
		}

		#region IUpdatable implementation

		public void Update (GameTime time) {
			ypos = MathHelper.Clamp (ypos + (drag * (float) time.Elapsed.TotalSeconds), tex [0].Height / 2, Game.Resolution.Height - (tex [0].Height * 2));
			if (ydelta > 0) {
				var xdrag = (drag * 3 * (float) time.Elapsed.TotalSeconds);
				ypos -= xdrag;
				ydelta = MathHelper.Clamp (ydelta - xdrag, 0, Game.Resolution.Height);
			}
			if (game.Mouse.IsButtonDown (MouseButton.Left) && !mouseDown) {
				ydelta += drag * .75f;
				mouseDown = true;
			} else if (game.Mouse.IsButtonUp (MouseButton.Left) && mouseDown)
				mouseDown = false;
			if (animationDelta < 100f)
				animationDelta = MathHelper.Clamp (animationDelta + (2f * (float) time.Elapsed.TotalMilliseconds), 0f, 100f);
			else {
				currentAnimation++;
				if (currentAnimation >= tex.Length)
					currentAnimation = 0;
				animationDelta -= 100f;
			}
		}

		#endregion

		#region IDrawable2D implementation

		public void Draw (GameTime time, SpriteBatch batch) {
			batch.Draw (tex [currentAnimation], new Vector2 (Game.Resolution.Width * .25f, ypos), Color4.White, new Vector2 (1.5f, 1.5f));
		}

		#endregion
	}
}

