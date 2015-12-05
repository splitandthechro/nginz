using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using nginz.Common;

namespace nginz {
	[CLSCompliant (false)]
	public class FontTTFProvider : AssetProvider<Font> {
		public FontTTFProvider (ContentManager manager)
			: base (manager, "fonts") { }

		public override Font Load (string assetName, params object[] args) {
			return new Font (assetName, args.Length == 1 ? (float) args[0] : 12f);
		}
	}
}
