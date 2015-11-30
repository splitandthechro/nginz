using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using nginz;
using nginz.Common;
using nginz.Staging.Interfaces;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Input;

namespace testgame2D {
	public class MascotActor : IActor {
		public Texture2D MascotTexture;
		public Vector2 Position = Vector2.Zero;
		
		public void Action (GameTime time, KeyboardBuffer keyboard, MouseBuffer mouse) {
			if (keyboard.IsKeyDown (Key.W))
				Position.Y -= 1;
			if (keyboard.IsKeyDown (Key.S))
				Position.Y += 1;
			if (keyboard.IsKeyDown (Key.A))
				Position.X -= 1;
			if (keyboard.IsKeyDown (Key.D))
				Position.X += 1;
		}

		public void Draw (GameTime time, SpriteBatch batch) {
			batch.Draw (MascotTexture, Position, Color4.White, new Vector2 (4));
		}

		public void Initialize (ContentManager content) {
			MascotTexture = content.Load<Texture2D> ("mascot.png", TextureConfiguration.Nearest);
		}
	}
}
