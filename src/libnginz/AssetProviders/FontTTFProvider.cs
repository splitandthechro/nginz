using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using nginz.Common;

namespace nginz {
	[CLSCompliant (false)]
	public class FontTTFProvider : AssetProvider<FontTTF> {
		public FontTTFProvider (ContentManager manager)
			: base (manager, "fonts") { }

		public override FontTTF Load (string assetName, params object[] args) {
			return new FontTTF (assetName, args.Length == 1 ? (float) args[0] : 12f);
		}
	}
}
