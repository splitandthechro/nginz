using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace nginz
{
	static class InteropTools
	{
		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern bool SetDllDirectory(string path);

		public static void DetectArchitecture()
		{
			if (Environment.OSVersion.Platform == PlatformID.Win32Windows ||
			   Environment.OSVersion.Platform == PlatformID.Win32NT ||
			   Environment.OSVersion.Platform == PlatformID.Win32S) {
				var path = Path.GetDirectoryName (Assembly.GetEntryAssembly ().Location);
				path = Path.Combine (path, IntPtr.Size == 8 ? "x64" : "x86");
				bool ok = SetDllDirectory (path);
				if (!ok) throw new System.ComponentModel.Win32Exception ();
			}
		}
	}
}

