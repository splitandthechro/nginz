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
		/// The content root.
		/// </summary>
		string contentRoot;

		/// <summary>
		/// Gets or sets the content root.
		/// </summary>
		/// <value>The content root.</value>
		public string ContentRoot {
			get { return contentRoot; }
			set {
				contentRoot = NormalizePath (value);
			}
		}

		/// <summary>
		/// The asset handlers.
		/// </summary>
		public Dictionary<Type, object> AssetHandlers;

		/// <summary>
		/// Initializes a new instance of the <see cref="nginz.Common.ContentManager"/> class.
		/// </summary>
		/// <param name="root">Root.</param>
		public ContentManager (string root = "") {
			AssetHandlers = new Dictionary<Type, object> ();
			ContentRoot = root;
		}

		public string NormalizePath (string path) {
			return path
				.Replace ('/', Path.DirectorySeparatorChar)
				.Replace ('\\', Path.DirectorySeparatorChar);
		}

		/// <summary>
		/// Register an asset provider.
		/// </summary>
		/// <param name="type">Type.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public void RegisterAssetHandler<T> (Type type) {
			var v = type.GetInterfaces ();
			AssetHandlers[typeof (T)] = Activator.CreateInstance (type, new object[] { this });
		}

		/// <summary>
		/// Load the specified asset.
		/// </summary>
		/// <param name="asset">Asset.</param>
		/// <param name="args">Arguments.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public T Load<T> (string asset, params object[] args)
			where T : IAsset {

			// Check if there is an asset provider for the specified asset type
			if (AssetHandlers.ContainsKey (typeof (T))) {

				// Get the asset provider
				var provider = (AssetHandler<T>) AssetHandlers[typeof (T)];

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
			where T : IAsset {

			// Check if there is an asset provider for the specified asset type
			if (AssetHandlers.ContainsKey (typeof (T))) {

				// Get the asset provider
				var provider = (AssetHandler<T>) AssetHandlers[typeof (T)];

				// Log that the asset was loaded
				// this.Log ("Loaded asset {0} as {1}", Path.GetFileName (path), typeof (T).Name);

				// Return the asset
				return provider.Load (path, args);
			}

			// Log that the asset type is unsupported
			this.Log ("Unsupported {0} asset type!", typeof (T).Name);

			// Return the default value for the specified asset type
			// Usually null for reference types
			return default (T);
		}

		/// <summary>
		/// Load the specified asset.
		/// </summary>
		/// <param name="asset">Asset.</param>
		/// <param name="args">Arguments.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public List<T> LoadMultiple<T> (string asset, params object[] args)
			where T : IAsset {

			// Check if there is an asset provider for the specified asset type
			if (AssetHandlers.ContainsKey (typeof (T))) {

				// Get the asset provider
				var provider = (AssetHandler<T>) AssetHandlers[typeof (T)];

				// Load the asset
				return LoadFromMultiple<T> (provider.GetAssetPath (asset), args);
			}

			// Log that the asset type is unsupported
			this.Log ("Unsupported {0} asset type!", typeof (T).Name);

			// Return the default value for the specified asset type
			// Usually null for reference types
			return null;
		}

		/// <summary>
		/// Load the specified asset from the specified path.
		/// </summary>
		/// <returns>The from.</returns>
		/// <param name="path">Asset path.</param>
		/// <param name="args">Arguments.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public List<T> LoadFromMultiple<T> (string path, params object[] args)
			where T : IAsset {

			// Check if there is an asset provider for the specified asset type
			if (AssetHandlers.ContainsKey (typeof (T))) {

				// Get the asset provider
				var provider = (AssetHandler<T>) AssetHandlers[typeof (T)];

				// Log that the asset was loaded
				// this.Log ("Loaded asset {0} as {1}", Path.GetFileName (path), typeof (T).Name);

				// Return the asset
				return provider.LoadMultiple (path, args);
			}

			// Log that the asset type is unsupported
			this.Log ("Unsupported {0} asset type!", typeof (T).Name);

			// Return the default value for the specified asset type
			// Usually null for reference types
			return null;
		}

		public void Save<T> (T asset, string assetPath)
			where T : IAsset {

			// Check if there is an asset provider for the specified asset type
			if (AssetHandlers.ContainsKey (typeof(T))) {

				// Get the asset provider
				var provider = (AssetHandler<T>) AssetHandlers[typeof (T)];

				// Save the asset
				SaveTo<T> (asset, provider.GetAssetPath (assetPath));
				return;
			}

			// Log that the asset type is unsupported
			this.Log ("Unsupported {0} asset type!", typeof(T).Name);
		}

		public void SaveTo<T> (T asset, string assetPath)
			where T : IAsset {

			// Check if there is an asset provider for the specified asset type
			if (AssetHandlers.ContainsKey (typeof(T))) {
				var provider = (AssetHandler<T>) AssetHandlers [typeof(T)];
				provider.Save (asset, assetPath);
			}
		}
	}
}
