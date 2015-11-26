﻿using System;

namespace nginz.Common
{
	/// <summary>
	/// Asset provider.
	/// </summary>
	public abstract class AssetProvider<T> where T : Asset
	{

		/// <summary>
		/// Gets the root.
		/// </summary>
		/// <value>The root.</value>
		public string Root { get; private set; }

		/// <summary>
		/// Gets the asset root.
		/// </summary>
		/// <value>The asset root.</value>
		public string AssetRoot { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="nginz.Common.AssetProvider{T}"/> class.
		/// </summary>
		/// <param name="root">Root.</param>
		/// <param name="assetRoot">Asset root.</param>
		protected AssetProvider (string root, string assetRoot) {
			Root = root;
			AssetRoot = assetRoot;
		}

		/// <summary>
		/// Get the asset path.
		/// </summary>
		/// <returns>The asset path.</returns>
		/// <param name="asset">Asset.</param>
		public string GetAssetPath (string asset) {
			return Root + AssetRoot + asset;
		}

		/// <summary>
		/// Load the specified asset.
		/// </summary>
		/// <param name="assetName">Asset name.</param>
		/// <param name="args">Arguments.</param>
		public abstract T Load (string assetName, params object[] args);
	}
}
