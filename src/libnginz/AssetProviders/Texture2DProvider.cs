using System;
using nginz.Common;

namespace nginz
{

	/// <summary>
	/// Texture2D asset provider.
	/// </summary>
	public class Texture2DProvider : AssetProvider<Texture2D>
	{

		/// <summary>
		/// Initializes a new instance of the <see cref="nginz.Texture2DProvider"/> class.
		/// </summary>
		/// <param name="root">Root.</param>
		public Texture2DProvider (string root)
			: base (root, "textures") { }

		/// <summary>
		/// Load the specified asset.
		/// </summary>
		/// <param name="assetName">Asset name.</param>
		/// <param name="args">Arguments.</param>
		public override Texture2D Load (string assetName, params object[] args) {
			return Texture2D.FromFile (assetName);
		}
	}
}
