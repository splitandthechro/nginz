using System;
using System.Collections.Generic;
using System.IO;

namespace nginz.Common
{

	/// <summary>
	/// Content manager.
	/// </summary>
	public class ContentManager: ICanLog
	{

		/// <summary>
		/// Gets or sets the content root.
		/// </summary>
		/// <value>The content root.</value>
		public string ContentRoot { get; set; }

		/// <summary>
		/// The asset providers.
		/// </summary>
		public Dictionary<Type, object> AssetProviders;

		/// <summary>
		/// Initializes a new instance of the <see cref="nginz.Common.ContentManager"/> class.
		/// </summary>
		/// <param name="root">Root.</param>
		public ContentManager (string root = "") {
			AssetProviders = new Dictionary<Type, object> ();
			ContentRoot = root;
		}

		/// <summary>
		/// Register an asset provider.
		/// </summary>
		/// <param name="type">Type.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public void RegisterAssetProvider<T> (Type type) {
			AssetProviders[typeof (T)] = Activator.CreateInstance (type, new object[] { ContentRoot, this });
			this.Log ("Registered: {0}", typeof (T).Name);
		}

		/// <summary>
		/// Load the specified asset.
		/// </summary>
		/// <param name="asset">Asset.</param>
		/// <param name="args">Arguments.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public T Load<T> (string asset, params object[] args)
			where T : Asset {

			// Check if there is an asset provider for the specified asset type
			if (AssetProviders.ContainsKey (typeof (T))) {

				// Get the asset provider
				var provider = (AssetProvider<T>) AssetProviders[typeof (T)];

				// Load the asset
				return LoadFrom<T> (provider.GetAssetPath (asset), args);
			}

			// Log that the asset type is unsupported
			this.Log ("Unsupported {0} asset type!", typeof (T).Name);

			// Return the default value for the specified asset type
			// Usually null for reference types
			return default (T);
		}

		/// <summary>
		/// Load the specified asset from the specified path.
		/// </summary>
		/// <returns>The from.</returns>
		/// <param name="path">Asset path.</param>
		/// <param name="args">Arguments.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public T LoadFrom<T> (string path, params object[] args)
			where T : Asset {

			// Check if there is an asset provider for the specified asset type
			if (AssetProviders.ContainsKey (typeof (T))) {

				// Get the asset provider
				var provider = (AssetProvider<T>) AssetProviders[typeof (T)];

				// Log that the asset was loaded
				this.Log ("Loaded asset {0} as {1}", Path.GetFileName (path), typeof (T).Name);

				// Return the asset
				return provider.Load (path, args);
			}

			// Log that the asset type is unsupported
			this.Log ("Unsupported {0} asset type!", typeof (T).Name);

			// Return the default value for the specified asset type
			// Usually null for reference types
			return default (T);
		}
	}
}
