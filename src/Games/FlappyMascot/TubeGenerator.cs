using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using nginz;
using OpenTK;
using OpenTK.Graphics;

namespace FlappyMascot
{
	public class TubeGenerator : IUpdatable, IDrawable2D
	{
		class Tube {
			public Texture2D Texture;
			public Vector2 Position;
			public Rectangle Bounds {
				get {
					return new Rectangle (
						x: (int) Position.X,
						y: (int) Position.Y,
						width: Texture.Width,
						height: Texture.Height
					);
				}
			}
			public bool IsInView () {
				if (Position.X < (-Texture.Width) || Position.X > 640)
					return false;
				return true;
			}
		}

		const int MAX_TUBES = 2;
		const float TUBE_SPEED = .3f;
		readonly Random rng;
		readonly List<Tube> tubes;
		readonly Texture2D tube_top, tube_bottom;
		readonly Game game;
		int tubeCount;

		public TubeGenerator (Game game) {
			this.game = game;
			rng = new Random ();
			tubes = new List<Tube> ();
			tube_top = game.Content.Load<Texture2D> ("flappymascot_tube_top_large.png");
			tube_bottom = game.Content.Load<Texture2D> ("flappymascot_tube_bottom_large.png");
			AddTube ();
		}

		public bool CollidesWithTube (Rectangle bounds) {
			return tubes.Any (tube => tube.Bounds.IntersectsWith (bounds));
		}

		void AddTube () {
			if (tubeCount >= MAX_TUBES)
				return;
			var tubeIndex = rng.Next (0, 3);
			var tubePos = new Vector2 ();
			Texture2D tubeTex = null;
			switch (tubeIndex) {
			case 0:
				tubeTex = tube_top;
				tubePos.X = 640;
				tubePos.Y = 0;
				break;
			case 1:
			case 2:
				tubeTex = tube_bottom;
				tubePos.X = 640;
				tubePos.Y = tubeTex.Width;
				break;
			}
			tubes.Add (new Tube { Position = tubePos, Texture = tubeTex });
			++tubeCount;
		}

		#region IUpdatable implementation

		public void Update (GameTime time) {
			var speed = TUBE_SPEED * (float) time.Elapsed.TotalMilliseconds;
			for (var i = 0; i < tubes.Count; i++) {
				var tube = tubes [i];
				tube.Position.X -= speed;
				if (tube.Position.X < (160 - (tube.Texture.Width / 2)))
					AddTube ();
				if (tube.Position.X < (-tube.Texture.Width)) {
					tubes.Remove (tube);
					--tubeCount;
				}
			}
		}

		#endregion

		#region IDrawable implementation

		public void Draw (GameTime time, SpriteBatch batch) {
			tubes.ForEach (tube => {
				if (tube.IsInView ())
					batch.Draw (tube.Texture, tube.Position, Color4.White);
			});
		}

		#endregion
	}
}

