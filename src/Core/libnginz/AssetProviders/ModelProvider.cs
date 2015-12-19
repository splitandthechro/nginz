using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using nginz.Common;

namespace nginz {
	class ModelProvider : AssetHandler<Model> {
		public Dictionary<string, Dictionary<string, Model>> modelCache = new Dictionary<string, Dictionary<string, Model>> ();

		public ModelProvider (ContentManager manager)
			: base (manager, "models") { }

		public override Model Load (string assetName, params object[] args) {
			if (!modelCache.ContainsKey (assetName))
				modelCache.Add (assetName, AssimpLoader.LoadModels (assetName, (ShaderProgram) args[1]));
			return modelCache[assetName][(string) args[0]];
		}
	}
}
