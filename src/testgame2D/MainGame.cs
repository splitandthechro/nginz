using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using nginz;
using OpenTK.Input;

namespace testgame2D {
	class MainGame : Game {
		public MainGame (GameConfiguration conf) 
			: base (conf) { }

		protected override void Initialize () {
			base.Initialize ();
		}

		protected override void Resize (Resolution resolution) {
			base.Resize (resolution);
		}

		protected override void Update (GameTime time) {

			// Exit if escape is pressed
			if (Keyboard.IsKeyTyped (Key.Escape))
				Exit ();

			base.Update (time);
		}

		protected override void Draw (GameTime time) {
			base.Draw (time);
		}
	}
}
