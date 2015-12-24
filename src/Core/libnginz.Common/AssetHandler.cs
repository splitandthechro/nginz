using System;
using System.Collections.Generic;
using System.IO;

namespace nginz.Common
{
	/// <summary>
	/// Asset provider.
	/// </summary>
	public abstract class AssetHandler<T> where T : IAsset
	{
		/// <summary>
		/// Gets the asset root.
		/// </summary>
		/// <value>The asset root.</value>
		public string AssetRoot { get; private set; }
		
		public ContentManager Manager { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="nginz.Common.AssetHandler{T}"/> class.
		/// </summary>
		/// <param name="manager">Content manager.</param>
		/// <param name="assetRoot">Asset root.</param>
		protected AssetHandler (ContentManager manager, string assetRoot) {
			AssetRoot = assetRoot;
			Manager = manager;
		}

		/// <summary>
		/// Get the asset path.
		/// </summary>
		/// <returns>The asset path.</returns>
		/// <param name="asset">Asset.</param>
		public string GetAssetPath (string asset) {
			return Manager.NormalizePath (Path.Combine (Manager.ContentRoot, AssetRoot, asset));
		}

		/// <summary>
		/// Load the specified asset.
		/// </summary>
		/// <param name="assetName">Asset name.</param>
		/// <param name="args">Arguments.</param>
		public abstract T Load (string assetName, params object[] args);
		public virtual List<T> LoadMultiple (string assetName, params object[] args) {
			return new List<T> () { this.Load (assetName, args) };
		}

		/// <summary>
		/// Save the specified asset.
		/// </summary>
		/// <param name="asset">Asset.</param>
		/// <param name="assetPath">Asset path.</param>
		public virtual void Save (T asset, string assetPath) { }
	}
}

