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
		const int MAX_TUBES = 2;
		const float TUBE_SPEED = .4f;
		readonly Random rng;
		readonly List<TubeInstance> tubes;
		readonly Texture2D tube_top_large, tube_bottom_large;
		readonly Texture2D tube_top_normal, tube_bottom_normal;
		readonly Texture2D tube_top_small, tube_bottom_small;
		readonly Game game;
		int tubeCount;
		int lastIndex;

		public TubeGenerator (Game game) {
			this.game = game;
			rng = new Random ();
			tubes = new List<TubeInstance> ();
			tube_top_large = game.Content.Load<Texture2D> ("flappymascot_tube_large_top.png");
			tube_bottom_large = game.Content.Load<Texture2D> ("flappymascot_tube_large_bottom.png");
			tube_top_normal = game.Content.Load<Texture2D> ("flappymascot_tube_normal_top.png");
			tube_bottom_normal = game.Content.Load<Texture2D> ("flappymascot_tube_normal_bottom.png");
			tube_top_small = game.Content.Load<Texture2D> ("flappymascot_tube_small_top.png");
			tube_bottom_small = game.Content.Load<Texture2D> ("flappymascot_tube_small_bottom.png");
			lastIndex = -1;
			AddTube ();
		}

		public bool CollidesWithTube (RectangleF bounds) {
			return tubes
				.Any (instance => instance.Tubes
					.Any (tube => tube.Bounds.IntersectsWith (bounds)));
		}

		void AddTube () {
			if (tubeCount >= MAX_TUBES)
				return;
			int tubeIndex;
			while ((tubeIndex = rng.Next (0, 5)) == lastIndex) { }
			lastIndex = tubeIndex;
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
				tubeTex1 = tube_top_normal;
				tubeTex2 = tube_bottom_small;
				tubePos1.X = 640;
				tubePos1.Y = 0;
				tubePos2.X = 640;
				tubePos2.Y = game.Resolution.Height - tubeTex2.Height;
				break;
			case 3:
				tubeTex1 = tube_top_small;
				tubeTex2 = tube_bottom_normal;
				tubePos1.X = 640;
				tubePos1.Y = 0;
				tubePos2.X = 640;
				tubePos2.Y = game.Resolution.Height - tubeTex2.Height;
				break;
			case 4:
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

