using System;
using System.IO;
using System.Runtime.InteropServices;

namespace nginz.StbInterop
{
	public static class Stb
	{
		public static IntPtr Load(string filename, ref int x, ref int y, ref int n, int req_comp)
		{
			switch (Environment.OSVersion.Platform) {
			case PlatformID.Win32Windows:
			case PlatformID.Win32NT:
			case PlatformID.Win32S:
				if (IntPtr.Size == 8)
					return Win64.stbi_load (filename, ref x, ref y, ref n, req_comp);
				else
					return Win32.stbi_load (filename, ref x, ref y, ref n, req_comp);
			default:
				if(Directory.Exists("/Library") &&
					Directory.Exists("/System"))
					return OSX.stbi_load(filename, ref x, ref y, ref n, req_comp);
				if (IntPtr.Size == 8)
					return Linux64.stbi_load (filename, ref x, ref y, ref n, req_comp);
				else
					return Linux32.stbi_load(filename, ref x, ref y, ref n, req_comp);
			}
		}
		public static void Free(IntPtr data)
		{
			switch (Environment.OSVersion.Platform) {
			case PlatformID.Win32Windows:
			case PlatformID.Win32NT:
			case PlatformID.Win32S:
				if (IntPtr.Size == 8)
					Win64.stbi_image_free (data);
				else
					Win32.stbi_image_free (data);
				break;
			default:
				if (Directory.Exists ("/Library") &&
				   Directory.Exists ("/System")) {
					OSX.stbi_image_free (data);
				} else if (IntPtr.Size == 8)
					Linux64.stbi_image_free (data);
				else
					Linux32.stbi_image_free (data);
				break;
			}
		}
		class Win64
		{
			[DllImport("stb_image-win64.dll")]
			public static extern IntPtr stbi_load (string filename, ref int x, ref int y, ref int n, int req_comp);
			[DllImport("stb_image-win64.dll")]
			public static extern void stbi_image_free (IntPtr data);
		}
		class Win32
		{
			[DllImport ("stb_image-win32.dll", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr stbi_load (string filename, ref int x, ref int y, ref int n, int req_comp);
			[DllImport ("stb_image-win32.dll", CallingConvention = CallingConvention.Cdecl)]
			public static extern void stbi_image_free (IntPtr data);
		}
		class Linux64
		{
			[DllImport("stb_image-x86_64.so")]
			public static extern IntPtr stbi_load (string filename, ref int x, ref int y, ref int n, int req_comp);
			[DllImport("stb_image-x86_64.so")]
			public static extern void stbi_image_free (IntPtr data);
		}
		class Linux32
		{
			[DllImport("stb_image-i686.so")]
			public static extern IntPtr stbi_load (string filename, ref int x, ref int y, ref int n, int req_comp);
			[DllImport("stb_image-i686.so")]
			public static extern void stbi_image_free (IntPtr data);
		}
		class OSX
		{
			[DllImport("stb_image.dylib")]
			public static extern IntPtr stbi_load (string filename, ref int x, ref int y, ref int n, int req_comp);
			[DllImport("stb_image.dylib")]
			public static extern void stbi_image_free (IntPtr data);
		}
	}
}

