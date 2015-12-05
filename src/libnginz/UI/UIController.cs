using System;
using System.Collections.Generic;
using System.Linq;
using nginz.Common;

namespace nginz
{
	public class UIController : IUpdatable, IDrawable2D, ICanLog
	{
		static object syncRoot;
		static UIController instance;

		public static UIController Instance {
			get {
				if (instance == null)
					lock (syncRoot)
						if (instance == null)
							instance = new UIController ();
				return instance;
			}
		}

		static UIController () {
			syncRoot = new object ();
		}

		readonly List<UIScene> Scenes;
		public UIScene ActiveScene { get; private set; }
		public Game Game { get; private set; }
		public Dictionary<string, Font> Fonts;

		UIController () {
			Scenes = new List<UIScene> ();
			ActiveScene = UIScene.Empty;
			Fonts = new Dictionary<string, Font> ();
		}

		public void Bind (Game game) {
			Game = game;
		}

		public void RegisterScene (UIScene scene) {
			if (Scenes.All (s => s.SceneId != scene.SceneId)) {
				Scenes.Add (scene);
				this.Log ("Added scene: {0}", scene.Name);
			}
		}

		public void SwitchScene (Guid guid) {
			if (Scenes.Any (s => s.SceneId.Equals (guid)))
				ActiveScene = Scenes.First (s => s.SceneId.Equals (guid));
		}

		public void SwitchScene (string name) {
			if (Scenes.Any (s => s.Name == name)) {
				var scene = Scenes.First (s => s.Name == name).SceneId;
				SwitchScene (scene);
			}
		}

		public void LoadDefaultFonts () {
			var fonts = new [] {
				"Roboto-Black",
				"Roboto-Black-Italic",
				"Roboto-Bold",
				"Roboto-Bold-Italic",
				"Roboto-Bold-Condensed",
				"Roboto-Bold-Condensed-Italic",
				"Roboto-Condensed",
				"Roboto-Condensed-Italic",
				"Roboto-Italic",
				"Roboto-Light",
				"Roboto-Light-Italic",
				"Roboto-Medium",
				"Roboto-Medium-Italic",
				"Roboto-Regular",
				"Roboto-Thin",
				"Roboto-Thin-Italic"
			};
			foreach (var font in fonts) {
				try {
					var loaded = Game.Content.Load<Font> (font + ".ttf");
					Fonts.Add (font.Replace ('-', ' '), loaded);
				} catch (Exception ex) {
					this.Log ("Could not load font: {0}", font);
					this.Log ("Reason: {0}", ex.Message);
				}
			}
		}

		#region IUpdatable implementation

		public void Update (GameTime time) {
			ActiveScene.Update (time);
		}

		#endregion

		#region IDrawable2D implementation

		public void Draw (GameTime time, SpriteBatch batch) {
			ActiveScene.Draw (time, batch);
		}

		#endregion
	}
}

