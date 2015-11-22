using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nginz {
	public static class DictionaryExtensions {
		public static T Get<T> (this Dictionary<string, object> @this, string key) where T : class {
			return @this[key] as T;
		}
	}
}
