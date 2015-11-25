using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nginz.Common {
	public interface Asset {
	}

	public abstract class AssetProvider<T> where T : Asset {
		public AssetProvider (string root, string assetRoot) {
			this.Root = root;
			this.AssetRoot = assetRoot;
		}

		public string Root { get; }
		public string AssetRoot { get; }

		public string GetAssetPath (string asset) {
			return Root + AssetRoot + asset;
		}

		public abstract T Load (string assetName, params object[] args);
	}

	public class ContentManager: ICanLog {
		public string ContentRoot { get; set; }

		public Dictionary<Type, object> AssetProviders = new Dictionary<Type, object> ();

		public ContentManager (string root = "") {
			this.ContentRoot = root;
		}

		public void RegisterAssetProvider<T>(Type type) {
			this.AssetProviders[typeof (T)] = Activator.CreateInstance (type, new[] { ContentRoot });
			this.Log ("Registered: {0}", typeof (T).Name);
		}

		public T Load<T>(string asset, params object[] args) where T : Asset {
			if (AssetProviders.ContainsKey (typeof (T))) {
				AssetProvider<T> provider = (AssetProvider<T>) AssetProviders[typeof (T)];
				this.Log ("Loaded asset {0} as {1}", asset, typeof (T).Name);
				return provider.Load (provider.GetAssetPath (asset), args);
			}
			this.Log ("Unsupported {0} asset type!", typeof (T).Name);
			return default (T);
		}

		public T LoadAbsolute<T>(string asset, params object[] args) where T : Asset {
			if (AssetProviders.ContainsKey (typeof (T))) {
				AssetProvider<T> provider = (AssetProvider<T>) AssetProviders[typeof (T)];
				this.Log ("Loaded asset {0} as {1}", asset, typeof (T).Name);
				return provider.Load (asset, args);
			}
			this.Log ("Unsupported {0} asset type!", typeof (T).Name);
			return default (T);
		}
	}
}
