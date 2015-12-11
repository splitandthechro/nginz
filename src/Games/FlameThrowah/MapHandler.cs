using System;
using nginz;
using nginz.Common;

namespace FlameThrowah
{
	public class MapHandler : AssetHandler<Map>
	{
		public MapHandler (ContentManager content)
			: base (content, "maps") { }

		public override Map Load (string assetName, params object[] args) {
			var filename = assetName.EndsWith (".json")
				? assetName
				: string.Format ("{0}.json", assetName);
			return Map.Load (filename);
		}

		public override void Save (Map asset, string assetPath) {
			var filename = assetPath.EndsWith (".json")
				? assetPath
				: string.Format ("{0}.json", assetPath);
			asset.Save (filename);
		}
	}
}

