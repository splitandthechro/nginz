using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using nginz;
using nginz.Common;
using nginz.Staging.Interfaces;
using OpenTK;
using OpenTK.Graphics;

namespace testgame2D {
	public class MascotActor : IActor {
		public Texture2D MascotTexture;

		public void Act (GameTime time) {
		}

		public void Draw (GameTime time, SpriteBatch batch) {
			batch.Draw (MascotTexture, new Vector2 (500, 280), Color4.White, new Vector2 (4));
		}

		public void Initialize (ContentManager content) {
			MascotTexture = content.Load<Texture2D> ("mascot.png", TextureConfiguration.Nearest);
		}
	}
}
