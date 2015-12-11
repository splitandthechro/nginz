using System;
using nginz.Common;

namespace nginz
{
	public class Sound : IAsset
	{
		public string Filename;

		public Sound (string filename) {
			Filename = filename;
		}
	}
}

