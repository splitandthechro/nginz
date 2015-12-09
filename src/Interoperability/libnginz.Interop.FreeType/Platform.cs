using System;
using System.IO;
using System.Runtime.InteropServices;

namespace nginz.Interop.FreeType
{
	enum Platforms {
		Windows,
		OSX,
		Linux
	}
	static class Platform {
		public static Platforms CurrentPlatform;
		static Platform()
		{
			if(Path.DirectorySeparatorChar == '/') {
				if(Directory.Exists("/System") &&
					Directory.Exists("/Library")) {
					CurrentPlatform = Platforms.OSX;
				} else {
					CurrentPlatform = Platforms.Linux;
				}
			} else {
				CurrentPlatform = Platforms.Windows;
			}

		}
		public static DllLoader GetDllLoader()
		{
			switch(CurrentPlatform) {
			case Platforms.OSX:
				return new DllMac();
			case Platforms.Linux:
				return new DllLinux();
			case Platforms.Windows:
				return new DllWindows ();
			default:
				throw new NotImplementedException();
			}
		}
	}
	interface DllLoader
	{
		IntPtr LoadLibrary(string filename);
		void FreeLibrary(IntPtr handle);
		IntPtr GetProcAddress(IntPtr dllHandle, string name);
	}
	class DllWindows : DllLoader
	{
		[DllImport("kernel32.dll")]
		private static extern IntPtr LoadLibrary(string dllToLoad);

		[DllImport("kernel32.dll")]
		private static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);

		[DllImport("kernel32.dll")]
		private static extern bool FreeLibrary(IntPtr hModule);

		IntPtr DllLoader.LoadLibrary(string fileName) {
			return LoadLibrary (fileName);
		}
		IntPtr DllLoader.GetProcAddress(IntPtr dllHandle, string name) {
			return GetProcAddress (dllHandle, name);
		}
		void DllLoader.FreeLibrary(IntPtr handle) {
			FreeLibrary (handle);
		}
	}
	class DllLinux : DllLoader
	{
		const int RTLD_NOW = 2;
		[DllImport("libdl.so")]
		private static extern IntPtr dlopen(string filename, int flags);

		[DllImport("libdl.so")]
		private static extern IntPtr dlsym(IntPtr handle, string symbol);

		[DllImport("libdl.so")]
		private static extern IntPtr dlclose(IntPtr handle);

		[DllImport("libdl.so")]
		private static extern IntPtr dlerror();
		public IntPtr LoadLibrary(string fileName) {
			return dlopen(fileName, RTLD_NOW);
		}

		public void FreeLibrary(IntPtr handle) {
			dlclose(handle);
		}
		public IntPtr GetProcAddress(IntPtr dllHandle, string name) {
			// clear previous errors if any
			dlerror();
			var res = dlsym(dllHandle, name);
			var errPtr = dlerror();
			if (errPtr != IntPtr.Zero) {
				throw new Exception("dlsym: " + Marshal.PtrToStringAnsi(errPtr));
			}
			return res;
		}
	}
	class DllMac : DllLoader
	{
		const int RTLD_NOW = 2;
		[DllImport("libSystem.B.dylib")]
		private static extern IntPtr dlopen(string filename, int flags);

		[DllImport("libSystem.B.dylib")]
		private static extern IntPtr dlsym(IntPtr handle, string symbol);

		[DllImport("libSystem.B.dylib")]
		private static extern IntPtr dlclose(IntPtr handle);

		[DllImport("libSystem.B.dylib")]
		private static extern IntPtr dlerror();
		public IntPtr LoadLibrary(string fileName) {
			var ptr = dlopen(fileName, RTLD_NOW);
			var errPtr = dlerror ();
			if (errPtr != IntPtr.Zero) {
				throw new Exception ("dlopen: " + Marshal.PtrToStringAnsi (errPtr));
			}
			return ptr;
		}

		public void FreeLibrary(IntPtr handle) {
			dlclose(handle);
		}
		public IntPtr GetProcAddress(IntPtr dllHandle, string name) {
			// clear previous errors if any
			dlerror();
			var res = dlsym(dllHandle, name);
			var errPtr = dlerror();
			if (errPtr != IntPtr.Zero) {
				throw new Exception("dlsym: " + Marshal.PtrToStringAnsi(errPtr));
			}
			return res;
		}
	}
}