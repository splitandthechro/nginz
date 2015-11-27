using System;
using nginz.Common;

namespace nginz
{

	/// <summary>
	/// Object file asset provider.
	/// </summary>
	public class ObjProvider : AssetProvider<ObjFile>
	{

		/// <summary>
		/// Initializes a new instance of the <see cref="nginz.ObjProvider"/> class.
		/// </summary>
		/// <param name="root">Root.</param>
		public ObjProvider (string root)
			: base (root, "models") { }

		/// <summary>
		/// Load the specified asset.
		/// </summary>
		/// <param name="assetName">Asset name.</param>
		/// <param name="args">Arguments.</param>
		public override ObjFile Load (string assetName, params object[] args) {
			var path = GetAssetPath (assetName);
			return ObjLoaderFactory.LoadFrom (path);
		}
	}
}

