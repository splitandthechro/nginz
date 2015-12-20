using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using nginz.Common;

namespace nginz {
	class GeometryProvider : AssetHandler<Geometry> {
		public Dictionary<string, Dictionary<string, Geometry>> modelCache = new Dictionary<string, Dictionary<string, Geometry>> ();

		public GeometryProvider (ContentManager manager)
			: base (manager, "models") { }

		public override Geometry Load (string assetName, params object[] args) {
			if (!modelCache.ContainsKey (assetName))
				modelCache.Add (assetName, AssimpLoader.LoadGeometry (assetName));
			return modelCache[assetName][(string) args[0]];
		}
	}
}
