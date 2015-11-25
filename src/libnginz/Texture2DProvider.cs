using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using nginz.Common;

namespace nginz {
	public class Texture2DProvider : AssetProvider<Texture2D> {
		public Texture2DProvider (string root)
			: base (root, "textures/") {
		}

		public override Texture2D Load (string assetName, params object[] args) {
			return Texture2D.FromFile (assetName);
		}
	}
}
