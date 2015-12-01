using System;
using System.IO;
using nginz.Common;

namespace nginz.Interop.IronPython
{
	public class PythonScript : Asset
	{
		readonly public string FilePath;
		readonly public string Source;

		public bool HasValidPath { get { return FilePath != null; } }

		public PythonScript (string path, string source) {
			FilePath = path;
			Source = source;
		}

		public static PythonScript FromSource (string source) {
			return new PythonScript (
				path: null,
				source: source
			);
		}

		public static PythonScript FromFile (string path) {
			path = Path.GetFullPath (path);
			string source;
			using (var file = File.Open (path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
			using (var reader = new StreamReader (file))
				source = reader.ReadToEnd ();
			return new PythonScript (
				path: path,
				source: source
			);
		}
	}
}

