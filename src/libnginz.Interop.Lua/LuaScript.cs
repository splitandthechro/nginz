using System;
using System.IO;
using nginz.Common;

namespace nginz.Interop.Lua
{
	public class LuaScript : Asset
	{
		readonly public string FilePath;
		readonly public string Source;

		public bool HasValidPath { get { return FilePath != null; } }

		public LuaScript (string path, string source) {
			FilePath = path;
			Source = source;
		}

		public static LuaScript FromSource (string source) {
			return new LuaScript (
				path: null,
				source: source
			);
		}

		public static LuaScript FromFile (string path) {
			path = Path.GetFullPath (path);
			string source;
			using (var file = File.Open (path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
			using (var reader = new StreamReader (file))
				source = reader.ReadToEnd ();
			return new LuaScript (
				path: path,
				source: source
			);
		}
	}
}

