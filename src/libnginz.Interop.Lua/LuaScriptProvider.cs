using System;
using nginz.Common;

namespace nginz.Interop.Lua
{
	public class LuaScriptProvider : AssetProvider<LuaScript>
	{
		public LuaScriptProvider (ContentManager manager)
			: base (manager, "scripts") { }

		public override LuaScript Load (string assetName, params object[] args) {
			var filename = assetName.EndsWith (".lua")
				? assetName
				: string.Format ("{0}.lua", assetName);
			return (LuaScript) Script.FromFile (filename);
		}
	}
}

