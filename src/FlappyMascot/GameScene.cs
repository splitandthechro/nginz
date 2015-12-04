using System;
using nginz;

namespace FlappyMascot
{
	public class GameScene : UIScene
	{
		readonly Bird bird;

		public GameScene () : base ("maingame") {
			bird = new Bird (UIController.Instance.Game);
		}

		public override void Update (GameTime time) {
			bird.Update (time);
			base.Update (time);
		}

		public override void Draw (GameTime time, SpriteBatch batch) {
			bird.Draw (time, batch);
			base.Draw (time, batch);
		}
	}
}

