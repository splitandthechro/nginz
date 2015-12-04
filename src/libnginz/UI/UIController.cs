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

		UIController () {
			Scenes = new List<UIScene> ();
			ActiveScene = UIScene.Empty;
		}

		public void Bind (Game game) {
			Game = game;
		}

		public void RegisterScene (UIScene scene) {
			if (Scenes.All (s => s.SceneId != scene.SceneId)) {
				this.Log ("Adding scene: {0}", scene.Name);
				Scenes.Add (scene);
			}
		}

		public void SwitchScene (Guid guid) {
			if (Scenes.Any (s => s.SceneId.Equals (guid)))
				ActiveScene = Scenes.First (s => s.SceneId.Equals (guid));
		}

		public void SwitchScene (string name) {
			if (Scenes.Any (s => s.Name == name)) {
				var scene = Scenes.First (s => s.Name == name).SceneId;
				this.Log ("Switching to scene: {0}", name);
				SwitchScene (scene);
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

