using System;
using nginz.Common;

namespace nginz.Interop.Lua
{
	public class LuaScript : Script
	{
		public LuaScript (string path, string source)
			: base (path, source) { }
	}
}

