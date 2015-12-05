using System;
using nginz;
using OpenTK;
using OpenTK.Graphics;

namespace FlappyMascot
{
	public class GameScene : UIScene
	{
		readonly Game game;
		readonly Bird bird;
		readonly Label lblScore;
		readonly Texture2D texMap;
		int score;
		float scoreDelta;

		float backgroundLeft;

		public GameScene () : base ("maingame") {
			game = UIController.Instance.Game;
			bird = new Bird (game);
			lblScore = new Label (150, 20, "Roboto Regular") {
				X = 25,
				Y = Game.Resolution.Height - 45,
				FontSize = 18f,
				Text = "Score: 0"
			};
			backgroundLeft = 0f;
			texMap = game.Content.Load<Texture2D> ("flappymascot_map.png");
			Controls.Add (lblScore);
		}

		public override void Update (GameTime time) {
			scoreDelta += 2f * (float) time.Elapsed.TotalSeconds;
			if (scoreDelta > 1f) {
				score += 1;
				lblScore.Text = "Score: " + score.ToString ();
				scoreDelta -= 1f;
			}
			game.Mouse.CursorVisible = false;
			game.Mouse.ShouldCenterMouse = true;
			if (game.Keyboard.IsKeyTyped (OpenTK.Input.Key.Escape))
				UIController.Instance.SwitchScene ("mainmenu");
			var scrollSpeed = 160f * (float) time.Elapsed.TotalSeconds;
			backgroundLeft = Wrap (backgroundLeft - scrollSpeed, -texMap.Width);
			bird.Update (time);
			base.Update (time);
		}

		static float Wrap (float n, float min) {
			var val = n < min
				? n + (-min)
				: n;
			return val;
		}

		public override void Draw (GameTime time, SpriteBatch batch) {
			batch.Draw (texMap, new Vector2 (backgroundLeft, 0), Color4.White);
			if (backgroundLeft < 640)
				batch.Draw (texMap, new Vector2 (backgroundLeft + texMap.Width, 0), Color4.White);
			bird.Draw (time, batch);
			base.Draw (time, batch);
		}
	}
}

