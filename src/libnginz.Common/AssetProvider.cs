using System;
using System.IO;

namespace nginz.Common
{
	/// <summary>
	/// Asset provider.
	/// </summary>
	public abstract class AssetProvider<T> where T : IAsset
	{
		/// <summary>
		/// Gets the asset root.
		/// </summary>
		/// <value>The asset root.</value>
		public string AssetRoot { get; private set; }
		
		public ContentManager Manager { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="nginz.Common.AssetProvider{T}"/> class.
		/// </summary>
		/// <param name="manager">Content manager.</param>
		/// <param name="assetRoot">Asset root.</param>
		protected AssetProvider (ContentManager manager, string assetRoot) {
			AssetRoot = assetRoot;
			Manager = manager;
		}

		/// <summary>
		/// Get the asset path.
		/// </summary>
		/// <returns>The asset path.</returns>
		/// <param name="asset">Asset.</param>
		public string GetAssetPath (string asset) {
			return Path.Combine (Manager.ContentRoot, AssetRoot, asset);
		}

		/// <summary>
		/// Load the specified asset.
		/// </summary>
		/// <param name="assetName">Asset name.</param>
		/// <param name="args">Arguments.</param>
		public abstract T Load (string assetName, params object[] args);
	}
}

