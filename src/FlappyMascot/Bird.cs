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
		readonly Texture2D tex;
		readonly Game game;
		float ypos, ydelta;
		const float drag = 150;
		bool mouseDown;

		public Bird (Game game) {
			tex = game.Content.Load<Texture2D> ("flappymascot.png");
			this.game = game;
			ypos = (Game.Resolution.Height / 2) + (tex.Height / 2);
		}

		#region IUpdatable implementation

		public void Update (GameTime time) {
			ypos = MathHelper.Clamp (ypos + (drag * (float) time.Elapsed.TotalSeconds), tex.Height / 2, Game.Resolution.Height - (tex.Height * 2));
			if (ydelta > 0) {
				var xdrag = (drag * 5 * (float) time.Elapsed.TotalSeconds);
				ypos -= xdrag;
				ydelta = MathHelper.Clamp (ydelta - xdrag, 0, Game.Resolution.Height);
			}
			if (game.Mouse.IsButtonDown (MouseButton.Left) && !mouseDown) {
				ydelta += drag * .75f;
				mouseDown = true;
			} else if (game.Mouse.IsButtonUp (MouseButton.Left) && mouseDown)
				mouseDown = false;
		}

		#endregion

		#region IDrawable2D implementation

		public void Draw (GameTime time, SpriteBatch batch) {
			batch.Draw (tex, new Vector2 (Game.Resolution.Width * .25f, ypos), Color4.White, new Vector2 (2, 2));
		}

		#endregion
	}
}

