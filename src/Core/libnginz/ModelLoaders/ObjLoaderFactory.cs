using System;
using System.IO;

namespace nginz
{

	/// <summary>
	/// Object file loader factory.
	/// </summary>
	public static class ObjLoaderFactory
	{

		/// <summary>
		/// Load an obj file from source.
		/// </summary>
		/// <param name="source">Source.</param>
		public static ObjFile Load (string source) {
			var loader = new ObjLoader (source);
			return loader.Load ();
		}

		/// <summary>
		/// Load an obj file from disk.
		/// </summary>
		/// <returns>The from.</returns>
		/// <param name="path">Path.</param>
		public static ObjFile LoadFrom (string path) {
			return Load (File.ReadAllText (path));
		}
	}
}

