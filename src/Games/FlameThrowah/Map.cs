using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using nginz.Common;
using OpenTK;

namespace FlameThrowah
{
	public struct MapFragment {
		public int  texid;
		public PositionVector texpos;
	}

	public class Map : IAsset
	{
		public TextureLookup Lookup;

		public string Name;
		public List<MapFragment> Fragments;

		public Map () { }

		public Map (string name, TextureLookup lookup) {
			Name = name;
			Lookup = lookup;
			Fragments = new List<MapFragment> ();
		}

		public void Add (string name, Vector2 pos) {
			var fragment = new MapFragment {
				texid = Lookup [name],
				texpos = new PositionVector (pos)
			};
			Fragments.Add (fragment);
		}

		public void Save (string path) {
			var json = JsonConvert.SerializeObject (this, Formatting.Indented);
			using (var file = File.Open (path, FileMode.Create, FileAccess.Write, FileShare.Read))
			using (var writer = new StreamWriter (file))
				writer.Write (json);
		}

		public static Map Load (string path) {
			return JsonConvert.DeserializeObject<Map> (File.ReadAllText (path));
		}
	}
}

