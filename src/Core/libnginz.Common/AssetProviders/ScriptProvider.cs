using System;

namespace nginz.Common
{
	public class ScriptProvider : AssetHandler<Script>
	{
		public ScriptProvider (ContentManager manager)
			: base (manager, "scripts") { }

		public override Script Load (string assetName, params object[] args) {
			return Script.FromFile (assetName);
		}
	}
}

