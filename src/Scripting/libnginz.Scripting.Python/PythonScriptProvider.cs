using System;
using nginz.Common;

namespace nginz.Scripting.Python
{
	public class PythonScriptProvider : AssetProvider<PythonScript>
	{
		public PythonScriptProvider (ContentManager manager)
			: base (manager, "scripts") { }

		public override PythonScript Load (string assetName, params object[] args) {
			var filename = assetName.EndsWith (".py")
				? assetName
				: string.Format ("{0}.py", assetName);
			var script = new PythonScript ();
			script.SetFile (filename);
			return script;
		}
	}
}

