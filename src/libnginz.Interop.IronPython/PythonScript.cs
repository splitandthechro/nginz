using System;
using System.IO;
using nginz.Common;

namespace nginz.Interop.IronPython
{
	public class PythonScript : Script
	{
		public PythonScript (string path, string source)
			: base (path, source) { }
	}
}

