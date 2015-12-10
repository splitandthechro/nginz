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
		class TubeInstance {
			public Tube[] Tubes;
			public bool IsInView () {
				return Tubes.All (t => t.IsInView ());
			}
		}

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
		const float TUBE_SPEED = .2f;
		readonly Random rng;
		readonly List<TubeInstance> tubes;
		readonly Texture2D tube_top_large, tube_bottom_large;
		readonly Texture2D tube_top_small, tube_bottom_small;
		readonly Game game;
		int tubeCount;

		public TubeGenerator (Game game) {
			this.game = game;
			rng = new Random ();
			tubes = new List<TubeInstance> ();
			tube_top_large = game.Content.Load<Texture2D> ("flappymascot_tube_top_large.png");
			tube_bottom_large = game.Content.Load<Texture2D> ("flappymascot_tube_bottom_large.png");
			tube_top_small = game.Content.Load<Texture2D> ("flappymascot_tube_top_small.png");
			tube_bottom_small = game.Content.Load<Texture2D> ("flappymascot_tube_bottom_small.png");
			AddTube ();
		}

		public bool CollidesWithTube (Rectangle bounds) {
			return tubes
				.Any (instance => instance.Tubes
					.Any (tube => tube.Bounds.IntersectsWith (bounds)));
		}

		void AddTube () {
			// Spin the randomizer
			for (var i = 0; i < 1000; i++)
				rng.Next ();
			if (tubeCount >= MAX_TUBES)
				return;
			var tubeIndex = rng.Next (0, 3);
			var tubePos1 = new Vector2 ();
			var tubePos2 = new Vector2 ();
			Texture2D tubeTex1 = null;
			Texture2D tubeTex2 = null;
			switch (tubeIndex) {
			case 0:
				tubeTex1 = tube_top_large;
				tubePos1.X = 640;
				tubePos1.Y = 0;
				break;
			case 1:
				tubeTex1 = tube_top_small;
				tubeTex2 = tube_bottom_small;
				tubePos1.X = 640;
				tubePos1.Y = 0;
				tubePos2.X = 640;
				tubePos2.Y = game.Resolution.Height - tubeTex2.Height;
				break;
			case 2:
				tubeTex1 = tube_bottom_large;
				tubePos1.X = 640;
				tubePos1.Y = game.Resolution.Height - tubeTex1.Height;
				break;
			}
			var instanceTubeCount = 0;
			if (tubeTex1 != null)
				++instanceTubeCount;
			if (tubeTex2 != null)
				++instanceTubeCount;
			var instance = new TubeInstance ();
			instance.Tubes = new Tube[instanceTubeCount];
			instance.Tubes [0] = new Tube { Position = tubePos1, Texture = tubeTex1 };
			if (instanceTubeCount == 2)
				instance.Tubes [1] = new Tube { Position = tubePos2, Texture = tubeTex2 };
			tubes.Add (instance);
			++tubeCount;
		}

		#region IUpdatable implementation

		public void Update (GameTime time) {
			var speed = TUBE_SPEED * (float) time.Elapsed.TotalMilliseconds;
			for (var i = 0; i < tubes.Count; i++) {
				var instance = tubes [i];
				for (var j = 0; j < instance.Tubes.Length; j++) {
					var tube = instance.Tubes [j];
					tube.Position.X -= speed;
					if (tube.Position.X < (160 - (tube.Texture.Width / 2)))
						AddTube ();
					if (tube.Position.X < (-tube.Texture.Width)) {
						tubes.Remove (instance);
						--tubeCount;
						break;
					}
				}
			}
		}

		#endregion

		#region IDrawable implementation

		public void Draw (GameTime time, SpriteBatch batch) {
			tubes.ForEach (instance => {
				if (instance.IsInView ()) {
					for (var i = 0; i < instance.Tubes.Length; i++) {
						batch.Draw (instance.Tubes [i].Texture, instance.Tubes [i].Position, Color4.White);
					}
				}
			});
		}

		#endregion
	}
}

