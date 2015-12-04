using System;
using nginz;
using OpenTK.Graphics;
using nginz.Common;

namespace FlappyMascot
{
	public class MenuScene : UIScene, ICanLog
	{
		public MenuScene () : base ("mainmenu") {

			// Get the game
			var game = UIController.Instance.Game;

			// The background panel
			var background = new Panel (Game.Resolution.Width, Game.Resolution.Height);
			background.SetBackground (game.Content.Load<Texture2D> ("flappymascot_background.png"));

			// The start button
			var btnStart = new Button (200, 40) {
				X = (Game.Resolution.Width / 2) - 100,
				Y = (Game.Resolution.Height / 2) - 25,
				BackgroundTexture = game.Content.Load<Texture2D> ("button.png"),
				Text = "Start game",
				FontFamily = "Source Sans Pro",
				FontSize = 18f,
				ForegroundColor = new Color4 (.1f, .1f, .1f, 1)
			};
			btnStart.Click += (sender, e) => UIController.Instance.SwitchScene ("maingame");

			// The exit button
			var btnExit = new Button (200, 40) {
				X = (Game.Resolution.Width / 2) - 100,
				Y = (Game.Resolution.Height / 2) + 25,
				BackgroundTexture = game.Content.Load<Texture2D> ("button.png"),
				Text = "Exit",
				FontFamily = "Source Sans Pro",
				FontSize = 18f,
				ForegroundColor = new Color4 (.1f, .1f, .1f, 1)
			};
			btnExit.Click += (sender, e) => game.Exit ();

			// Add the controls to the UI controller
			background.Controls.Add (btnStart);
			background.Controls.Add (btnExit);
			Controls.Add (background);
		}
	}
}

