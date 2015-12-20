using System;
using System.Collections.Generic;

namespace nginz
{
	public class UIScene : IUpdatable, IDrawable2D
	{
		public static UIScene Empty;

		static UIScene () {
			Empty = new UIScene ("empty");
		}

		readonly public Guid SceneId;
		readonly public List<Control> Controls;
		public string Name { get; private set; }

		public UIScene (string name) {
			Name = name;
			SceneId = Guid.NewGuid ();
			Controls = new List<Control> ();
			UIController.Instance.RegisterScene (this);
		}

		public void MakeActive () {
			UIController.Instance.SwitchScene (SceneId);
		}

		#region IUpdatable implementation

		public virtual void Update (GameTime time) {
			for (var i = 0; i < Controls.Count; i++)
				Controls [i].Update (time);
		}

		#endregion

		#region IDrawable2D implementation

		public virtual void Draw (GameTime time, SpriteBatch batch) {
			for (var i = 0; i < Controls.Count; i++)
				Controls [i].Draw (time, batch);
		}

		#endregion

		public virtual void OnSceneSwitch () {
		}
	}
}

