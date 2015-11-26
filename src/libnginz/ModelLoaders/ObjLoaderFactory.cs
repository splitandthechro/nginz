using System;
using System.IO;

namespace nginz
{
	public static class ObjLoaderFactory
	{
		public static ObjFile Load (string source) {
			var loader = new ObjFileLoader (source);
			return loader.Load ();
		}

		public static ObjFile LoadFrom (string path) {
			return Load (File.ReadAllText (path));
		}
	}
}

