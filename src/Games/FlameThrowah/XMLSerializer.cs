using System;
using System.Xml.Serialization;
using System.IO;

namespace FlameThrowah
{
	public static class XMLSerializer
	{
		public static void Serialize<T> (this T baseType, string path) {
			var serializer = new XmlSerializer (typeof(T));
			using (var stream = new FileStream (path, FileMode.Create, FileAccess.Write, FileShare.Read))
				serializer.Serialize (stream, baseType);
		}

		public static T Deserialize <T> (string filename) {
			T instance;
			var serializer = new XmlSerializer (typeof(T));
			if (!File.Exists (filename)) {
				string ShortDirectory = (filename.Length > 30) ? (filename.Substring (0, 30) + "...") : (filename);
				throw new FileNotFoundException (string.Format ("Directory \"{0}\" not found", ShortDirectory));
			}
			using (var stream = new FileStream (filename, FileMode.Open, FileAccess.Read, FileShare.Read))
				instance = (T) serializer.Deserialize (stream);
			return instance;
		}
	}
}

