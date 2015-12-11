using System;
using nginz.Common;

namespace nginz
{
	public class SoundProvider : AssetHandler<Sound>
	{
		public SoundProvider (ContentManager content)
			: base (content, "sounds") { }

		public override Sound Load (string assetName, params object[] args) {
			return new Sound (assetName);
		}
	}
}

