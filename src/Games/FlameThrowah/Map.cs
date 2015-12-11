using System;
using OpenTK;
using nginz.Common;
using System.Collections.Generic;

namespace FlameThrowah
{
	[Serializable]
	public class Map : IAsset
	{
		[NonSerialized]
		public TextureLookup Lookup;

		public string Name;
		public List<Vector2> Positions;
		public List<int> TexturesIndices;

		public Map (string name, TextureLookup lookup) {
			Name = name;
			Lookup = lookup;
			Positions = new List<Vector2> ();
			TexturesIndices = new List<int> ();
		}

		public void Add (int textureId, Vector2 pos) {
			Positions.Add (pos);
			TexturesIndices.Add (textureId);
		}

		public void Save (string path) {
			this.Serialize (path);
		}

		public static Map Load (string path) {
			return XMLSerializer.Deserialize<Map> (path);
		}
	}
}

