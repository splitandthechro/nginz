using System;
using nginz;
using OpenTK.Graphics;
using nginz.Common;
using OpenTK;

namespace FlappyMascot
{
	public class MenuScene : UIScene, ICanLog
	{
		Game game;
		Texture2D texMenu, texMap, texOverlay;
		Button btnStart, btnExit;

		bool fadeIn;
		bool fadeOut;
		float overlayAlpha;
		float menuAlpha;

		public MenuScene () : base ("mainmenu") {

			// Get the game
			game = UIController.Instance.Game;

			// Load textures
			texMenu = game.Content.Load<Texture2D> ("flappymascot_menu_background.png");
			texMap = game.Content.Load<Texture2D> ("flappymascot_map_new.png");
			texOverlay = game.Content.Load<Texture2D> ("flappymascot_overlay.png");

			// Create the layout
			CreateLayout ();

			// Subscribe to events
			btnStart.Click += (sender, e) => fadeOut = true;
			btnExit.Click += (sender, e) => game.Exit ();

			// Reset the layout
			ResetLayout (false);

			// Add the controls to the UI controller
			Controls.Add (btnStart);
			Controls.Add (btnExit);
		}

		public override void Update (GameTime time) {
			game.Mouse.CursorVisible = true;
			game.Mouse.ShouldCenterMouse = false;
			if (fadeOut) {
				var fadeSpeed = 2f * (float) time.Elapsed.TotalSeconds;
				overlayAlpha = MathHelper.Clamp (overlayAlpha - fadeSpeed, 0f, 1f);
				menuAlpha = MathHelper.Clamp (menuAlpha - fadeSpeed * 2f, 0f, 1f);
				btnStart.Transparency = MathHelper.Clamp (btnStart.Transparency - (fadeSpeed * 2f), 0f, 1f);
				btnExit.Transparency = MathHelper.Clamp (btnStart.Transparency - (fadeSpeed * 2f), 0f, 1f);
				if (overlayAlpha <= .1f && menuAlpha <= .1f) {
					ResetLayout (true);
					UIController.Instance.SwitchScene ("maingame");
				}
			} else if (fadeIn) {
				var fadeSpeed = 5f * (float) time.Elapsed.TotalSeconds;
				menuAlpha = MathHelper.Clamp (menuAlpha + fadeSpeed, 0f, 1f);
				btnStart.Transparency = MathHelper.Clamp (btnStart.Transparency + (fadeSpeed * 2f), 0f, 1f);
				btnExit.Transparency = MathHelper.Clamp (btnStart.Transparency + (fadeSpeed * 2f), 0f, 1f);
				overlayAlpha = MathHelper.Clamp (overlayAlpha + (fadeSpeed * 2f), 0f, .5f);
			}
			base.Update (time);
		}

		public override void Draw (GameTime time, SpriteBatch batch) {
			batch.Draw (texMap, OpenTK.Vector2.Zero, Color4.White);
			batch.Draw (texOverlay, OpenTK.Vector2.Zero, new Color4 (1, 1, 1, overlayAlpha));
			batch.Draw (texMenu, OpenTK.Vector2.Zero, new Color4 (1, 1, 1, menuAlpha));
			base.Draw (time, batch);
		}

		void CreateLayout () {
			btnStart = new Button (200, 40, "Roboto Regular") {
				BackgroundTexture = game.Content.Load<Texture2D> ("button.png"),
				Text = "Start game",
				FontSize = 18f,
				ForegroundColor = new Color4 (.1f, .1f, .1f, 1),
				HighlightForegroundColor = new Color4 (.3f, .3f, .3f, 1)
			};
			btnExit = new Button (200, 40, "Roboto Regular") {
				BackgroundTexture = game.Content.Load<Texture2D> ("button.png"),
				Text = "Exit",
				FontSize = 18f,
				ForegroundColor = new Color4 (.1f, .1f, .1f, 1),
				HighlightForegroundColor = new Color4 (0.5f, 0.263f, 0.235f, 1)
			};
		}

		void ResetLayout (bool setFadeIn = false) {
			btnStart.X = (game.Resolution.Width / 2) - 100;
			btnStart.Y = (game.Resolution.Height / 2) - 25;
			btnStart.Transparency = 1f;
			btnExit.X = (game.Resolution.Width / 2) - 100;
			btnExit.Y = (game.Resolution.Height / 2) + 25;
			btnExit.Transparency = 1f;
			fadeIn = setFadeIn;
			fadeOut = false;
			if (fadeIn) {
				btnStart.Transparency = 0f;
				btnExit.Transparency = 0f;
				overlayAlpha = 0f;
				menuAlpha = 0f;
				btnStart.Text = "Continue";
			} else {
				overlayAlpha = .5f;
				menuAlpha = 1f;
				btnStart.Text = "New game";
			}
		}
	}
}

