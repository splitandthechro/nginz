using System;
using System.IO;

namespace nginz.Common
{
	public class Script : IAsset
	{
		public string FilePath;
		public string Source;

		public bool HasValidPath { get { return FilePath != null; } }

		public Script () {
			Source = string.Empty;
		}

		public Script (string path, string source) {
			FilePath = path;
			Source = source;
		}

		public void SetSource (string source) {
			Source = source;
		}

		public static Script FromSource (string source) {
			return new Script (
				path: null,
				source: source
			);
		}

		public void SetFile (string path) {
			path = Path.GetFullPath (path);
			string source;
			using (var file = File.Open (path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
			using (var reader = new StreamReader (file))
				source = reader.ReadToEnd ();
			FilePath = path;
			Source = source;
		}

		public static Script FromFile (string path) {
			var script = new Script ();
			script.SetFile (path);
			return script;
		}
	}
}

