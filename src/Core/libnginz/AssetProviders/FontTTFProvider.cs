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
			return GetFont (assetName, args.Length == 1 ? (float) args[0] : 12f);
		}

		Dictionary<Tuple<string, float>,Font> cachedFonts = new Dictionary<Tuple<string, float>, Font>();
		Font GetFont(string name, float arg)
		{
			var k = new Tuple<string,float> (name, arg);
			if (!cachedFonts.ContainsKey (k))
				cachedFonts.Add (k, new Font (name, arg));
			return cachedFonts [k];
		}
	}
}
