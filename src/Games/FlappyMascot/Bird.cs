using System;
using nginz;
using OpenTK;
using OpenTK.Input;

namespace FlappyMascot
{
	public class Bird : IUpdatable, IDrawable2D
	{
		const float MAX_DRAG = 200;
		const float MIN_DRAG = 50;
		const float UP_VELOCITY = 125;
		readonly Game game;
		readonly SpriteSheet2D sheet;
		readonly Animator animator;
		float ypos, ydelta, upydelta;
		float drag;
		bool mouseDown;

		public Bird (Game game) {
			this.game = game;
			var sheetTex = game.Content.Load<Texture2D> ("flappymascot_char.png", TextureConfiguration.Nearest);
			sheet = new SpriteSheet2D (sheetTex, 4, 1);
			animator = new Animator (sheet, 4);
			animator.DurationInMilliseconds = 500;
			animator.Position.X = Game.Resolution.Width * 0.25f;
			ypos = (Game.Resolution.Height / 2) + (sheet [0].Height / 2);
		}

		#region IUpdatable implementation

		public void Update (GameTime time) {
			ypos = MathHelper.Clamp (ypos + (drag * (float) time.Elapsed.TotalSeconds), (sheet [0].Height / 1.5f) / 2, Game.Resolution.Height - ((sheet [0].Height / 1.5f) * 2));
			drag = MathHelper.Clamp (drag + (750 * (float)time.Elapsed.TotalSeconds), MIN_DRAG, MAX_DRAG);
			if (ydelta > 0) {
				var xdrag = (drag * 3 * (float) time.Elapsed.TotalSeconds);
				ypos -= xdrag;
				ydelta = MathHelper.Clamp (ydelta - xdrag, 0, Game.Resolution.Height);
			}
			if (game.Mouse.IsButtonDown (MouseButton.Left) && !mouseDown) {
				ydelta += UP_VELOCITY;
				upydelta = ydelta;
				mouseDown = true;
				drag = MIN_DRAG;
			} else if (game.Mouse.IsButtonUp (MouseButton.Left) && mouseDown)
				mouseDown = false;
			animator.Position.Y = ypos;
			animator.Update (time);
		}

		#endregion

		#region IDrawable2D implementation

		public void Draw (GameTime time, SpriteBatch batch) {
			animator.Draw (time, batch);
		}

		#endregion
	}
}

