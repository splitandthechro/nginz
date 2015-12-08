using System;
using nginz.Common;

namespace nginz.Interop.Iodine
{
	public class IodineScriptProvider : AssetProvider<IodineScript>
	{
		public IodineScriptProvider (ContentManager manager)
			: base (manager, "scripts") { }

		public override IodineScript Load (string assetName, params object[] args) {
			var filename = assetName.EndsWith (".id")
				? assetName
				: string.Format ("{0}.id", assetName);
			var script = new IodineScript ();
			script.SetFile (filename);
			return script;
		}
	}
}

