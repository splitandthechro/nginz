using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nginz {
	public struct Tile {
		public int TileId;
		public float Rotation;

		public static Tile Default = new Tile {
			TileId = -1,
			Rotation = 0f
		};
	}
}
