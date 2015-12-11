using System;
using nginz;
using nginz.Common;

namespace FlameThrowah
{
	public class MapProvider : AssetProvider<Map>
	{
		public MapProvider (ContentManager content)
			: base (content, "maps") { }

		public override Map Load (string assetName, params object[] args) {
			var filename = assetName.EndsWith (".xml")
				? assetName
				: string.Format ("{0}.xml", assetName);
			return Map.Load (filename);
		}
	}
}

