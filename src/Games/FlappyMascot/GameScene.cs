using System;
using nginz;
using OpenTK;
using OpenTK.Graphics;

namespace FlappyMascot
{
	public class GameScene : UIScene
	{
		const float DAYNIGHT_CYCLE_SPEED = 0.0025f;
		const float DAYNIGHT_CYCLE_MIN = 0.25f;
		readonly Game game;
		readonly TubeGenerator generator;
		readonly Bird bird;
		readonly Label lblScore;
		readonly Texture2D texMap;
		int score;
		float scoreDelta;
		float backgroundLeft;
		float blueTint;

		public GameScene () : base ("maingame") {
			game = UIController.Instance.Game;
			generator = new TubeGenerator (game);
			bird = new Bird (game, generator);
			lblScore = new Label (150, 20, "Roboto Regular") {
				X = 25,
				Y = game.Resolution.Height - 45,
				FontSize = 18f,
				Text = "Score: 0"
			};
			backgroundLeft = 0f;
			blueTint = 0f;
			texMap = game.Content.Load<Texture2D> ("flappymascot_map_new.png");
			Controls.Add (lblScore);
		}

		public override void Update (GameTime time) {
			blueTint = MathHelper.Clamp (blueTint + (DAYNIGHT_CYCLE_SPEED * (float) time.Elapsed.TotalSeconds), 0, DAYNIGHT_CYCLE_MIN);
			scoreDelta += 2f * (float) time.Elapsed.TotalSeconds;
			if (scoreDelta > 1f) {
				score += 1;
				lblScore.Text = "Score: " + score;
				scoreDelta -= 1f;
			}
			game.Mouse.CursorVisible = false;
			game.Mouse.ShouldCenterMouse = true;
			if (game.Keyboard.IsKeyTyped (OpenTK.Input.Key.Escape))
				UIController.Instance.SwitchScene ("mainmenu");
			var scrollSpeed = 160f * (float) time.Elapsed.TotalSeconds;
			backgroundLeft = Wrap (backgroundLeft - scrollSpeed, -texMap.Width);
			bird.Update (time);
			generator.Update (time);
			base.Update (time);
		}

		static float Wrap (float n, float min) {
			var val = n < min
				? n + (-min)
				: n;
			return val;
		}

		public override void Draw (GameTime time, SpriteBatch batch) {
			var tint = new Color4 (1 - (blueTint * 3f), 1f - (blueTint * 2f), 1f - blueTint, 1);
			batch.Draw (texMap, new Vector2 (backgroundLeft, 0), tint);
			if (backgroundLeft < 640)
				batch.Draw (texMap, new Vector2 (backgroundLeft + texMap.Width, 0), tint);
			bird.Draw (time, batch);
			generator.Draw (time, batch);
			base.Draw (time, batch);
		}
	}
}

