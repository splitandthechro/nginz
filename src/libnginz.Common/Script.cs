using System;
using System.IO;

namespace nginz.Common
{
	public class Script : Asset
	{
		readonly public string FilePath;
		readonly public string Source;

		public bool HasValidPath { get { return FilePath != null; } }

		public Script (string path, string source) {
			FilePath = path;
			Source = source;
		}

		public static Script FromSource (string source) {
			return new Script (
				path: null,
				source: source
			);
		}

		public static Script FromFile (string path) {
			path = Path.GetFullPath (path);
			string source;
			using (var file = File.Open (path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
			using (var reader = new StreamReader (file))
				source = reader.ReadToEnd ();
			return new Script (
				path: path,
				source: source
			);
		}
	}
}

