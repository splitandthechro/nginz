using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using nginz.Common;
using nginz.Staging.Interfaces;

namespace nginz.Staging {
	public class Stage {
		public List<IAct> Acts = new List<IAct> ();
		public List<IDraw> Drawings = new List<IDraw> ();
		public List<IActor> Actors = new List<IActor> ();

		Game game;

		public Stage (Game game) {
			this.game = game;
		}

		public void AddActor (IActor actor) {
			actor.Initialize (game.Content);
			Actors.Add (actor);
		}
		public void AddAction (IAct action) {
			Acts.Add (action);
		}
		public void AddDrawing (IDraw drawing) {
			Drawings.Add (drawing);
		}

		public void Act (GameTime time) {
			Acts.ForEach (x => x.Act (time));
			Actors.ForEach (x => x.Act (time));
		}
		public void Draw (GameTime time, SpriteBatch batch) {
			Drawings.ForEach (x => x.Draw (time, batch));
			Actors.ForEach (x => x.Draw (time, batch));
		}
	}
}
