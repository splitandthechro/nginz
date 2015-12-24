using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using nginz.Common;

namespace nginz {
	class GeometryProvider : AssetHandler<Geometry> {
		public Dictionary<string, List<Geometry>> modelCache = new Dictionary<string, List<Geometry>> ();

		public GeometryProvider (ContentManager manager)
			: base (manager, "models") { }

		public override Geometry Load (string assetName, params object[] args) {
			if (!modelCache.ContainsKey (assetName))
				modelCache.Add (assetName, AssimpLoader.LoadGeometry (assetName));
			return modelCache[assetName][(int) args[0]];
		}

		public override List<Geometry> LoadMultiple (string assetName, params object[] args) {
			if (!modelCache.ContainsKey (assetName))
				modelCache.Add (assetName, AssimpLoader.LoadGeometry (assetName));
			return modelCache[assetName];
		}
	}
}
