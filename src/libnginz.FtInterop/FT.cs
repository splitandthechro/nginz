using System;
using System.Runtime.InteropServices;
using System.IO;
using System.Reflection;

namespace nginz.FtInterop
{
	public static class FT
	{
		public const int FT_RENDER_MODE_NORMAL = 0;
		public const int FT_LOAD_DEFAULT = 0;
		//define FT_LOAD_TARGET_( x ) ( (FT_Int32)( (x) & 15) << 16 )
		//define FT_LOAD_TARGET_NORMAL = FT_LOAD_TARGET_(FT_RENDER_MODE_NORMAL)
		public const int FT_LOAD_TARGET_NORMAL = (FT_RENDER_MODE_NORMAL & 15) << 16;

		public const long FT_FACE_FLAG_KERNING = (1L << 6);
		//The fields do get assigned to, just through reflection.
		#pragma warning disable 0649

		public static bool Loaded = false;

		[UnmanagedFunctionPointer (CallingConvention.Cdecl)]
		public delegate int Init_FreeType (out IntPtr alibrary);

		public static Init_FreeType FT_Init_FreeType;

		[UnmanagedFunctionPointer (CallingConvention.Cdecl)]
		public delegate int Done_FreeType (IntPtr alibrary);

		public static Done_FreeType FT_Done_FreeType;

		[UnmanagedFunctionPointer (CallingConvention.Cdecl)]
		public delegate int New_Face (IntPtr library,string filepathname,int face_index,out IntPtr aface);

		public static New_Face FT_New_Face;

		[UnmanagedFunctionPointer (CallingConvention.Cdecl)]
		public delegate int Set_Char_Size (IntPtr face,IntPtr char_width,IntPtr char_height,uint horz_resolution,uint vert_resolution);

		public static Set_Char_Size FT_Set_Char_Size;

		[UnmanagedFunctionPointer (CallingConvention.Cdecl)]
		public delegate uint Get_Char_Index (IntPtr face,uint charcode);

		public static Get_Char_Index FT_Get_Char_Index;

		[UnmanagedFunctionPointer (CallingConvention.Cdecl)]
		public delegate int Load_Glyph (IntPtr face,uint glyph_index,int load_flags);

		public static Load_Glyph FT_Load_Glyph;

		[UnmanagedFunctionPointer (CallingConvention.Cdecl)]
		public delegate int Render_Glyph (IntPtr slot,int render_mode);

		public static Render_Glyph FT_Render_Glyph;

		[UnmanagedFunctionPointer (CallingConvention.Cdecl)]
		public delegate int Get_Kerning (IntPtr face,uint left_glyph,uint right_glyph,uint kern_mode,out FT_Vector akerning);

		public static Get_Kerning FT_Get_Kerning;

		[StructLayout (LayoutKind.Sequential)]
		public struct FT_Vector
		{
			//signed long FT_Pos -> IntPtr
			public IntPtr x;
			public IntPtr y;
		}

		static string ResolvePath(string path)
		{
			if (Path.IsPathRooted (path))
				return path;
			var myDir = Path.GetDirectoryName (Assembly.GetExecutingAssembly ().Location);
			if (File.Exists (Path.Combine (myDir, path)))
				return Path.Combine (myDir, path);
			return path;
		}

		public static void Load () {
			var loader = Platform.GetDllLoader ();
			IntPtr library = IntPtr.Zero;
			string libPath = "";
			switch (Platform.CurrentPlatform) {
			case Platforms.Linux:
				libPath = "libfreetype.so.6";
				break;
			case Platforms.Windows:
				if (Environment.Is64BitProcess) {
					libPath = "freetype6-win64.dll";
				} else {
					libPath = "freetype6-win32.dll";
				}
				break;
			case Platforms.OSX:
				libPath = "libfreetype.6.dylib";
				break;
			}
			library = loader.LoadLibrary (ResolvePath (libPath));
			LoadFunctions ((x) => loader.GetProcAddress (library, x));
			Loaded = true;
		}
		static void LoadFunctions(Func<string, IntPtr> func)
		{
			foreach (var f in typeof(FT).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)) {
				if (f.FieldType.BaseType == typeof(MulticastDelegate) ||
					f.FieldType.BaseType == typeof(Delegate)) {
					var ptr = func (f.Name);
					if (f.Name.Contains ("$") || f.Name.Contains ("<") || f.Name.Contains (">"))
						continue; //For some reason this reflection stuff catches compiler-internal static variables
					var del = Marshal.GetDelegateForFunctionPointer (ptr, f.FieldType);
					f.SetValue (null, del);
				}
			}
		}

		[StructLayout (LayoutKind.Sequential)]
		public class FaceRec
		{
			public IntPtr num_faces;
			public IntPtr face_index;

			public IntPtr face_flags;
			public IntPtr style_flags;

			public IntPtr num_glyphs;

			public IntPtr family_name;
			public IntPtr style_name;

			public int num_fixed_sizes;
			public IntPtr available_sizes;

			public int num_charmaps;
			public IntPtr charmaps;

			public GenericRec generic;

			public BBox bbox;

			public ushort units_per_EM;
			public short ascender;
			public short descender;
			public short height;

			public short max_advance_width;
			public short max_advance_height;

			public short underline_position;
			public short underline_thickness;

			public IntPtr glyph;
			public IntPtr size;
			public IntPtr charmap;

			private IntPtr driver;
			private IntPtr memory;
			private IntPtr stream;

			private IntPtr sizes_list;
			private GenericRec autohint;
			private IntPtr extensions;

			private IntPtr @public;

			public static int SizeInBytes { get { return Marshal.SizeOf (typeof(FaceRec)); } }
		}

		[StructLayout (LayoutKind.Sequential)]
		public struct GenericRec
		{
			public IntPtr data;
			public IntPtr finalizer;
		}

		[StructLayout (LayoutKind.Sequential)]
		public struct BBox
		{
			IntPtr xMin, yMin;
			IntPtr xMax, yMax;
		}

		[StructLayout (LayoutKind.Sequential)]
		public class GlyphSlotRec
		{
			public IntPtr library;
			public IntPtr face;
			public IntPtr next;
			public uint reserved;
			public GenericRec generic;

			public GlyphMetricsRec metrics;
			public IntPtr linearHoriAdvance;
			public IntPtr linearVertAdvance;
			public FT_Vector advance;

			public uint format;

			public BitmapRec bitmap;
			public int bitmap_left;
			public int bitmap_top;

			public OutlineRec outline;

			public uint num_subglyphs;
			public IntPtr subglyphs;

			public IntPtr control_data;
			public IntPtr control_len;

			public IntPtr lsb_delta;
			public IntPtr rsb_delta;

			public IntPtr other;

			private IntPtr @public;
		}

		[StructLayout (LayoutKind.Sequential)]
		public struct GlyphMetricsRec
		{
			public IntPtr width;
			public IntPtr height;

			public IntPtr horiBearingX;
			public IntPtr horiBearingY;
			public IntPtr horiAdvance;

			public IntPtr vertBearingX;
			public IntPtr vertBearingY;
			public IntPtr vertAdvance;
		}

		[StructLayout (LayoutKind.Sequential)]
		public struct BitmapRec
		{
			public int rows;
			public int width;
			public int pitch;
			public IntPtr buffer;
			public short num_grays;
			public byte pixel_mode;
			public byte palette_mode;
			public IntPtr palette;
		}

		[StructLayout (LayoutKind.Sequential)]
		public struct OutlineRec
		{
			public short n_contours;
			public short n_points;

			public IntPtr points;
			public IntPtr tags;
			public IntPtr contours;

			public int flags;
		}

		[StructLayout (LayoutKind.Sequential)]
		public class SizeRec
		{
			public IntPtr face;
			public GenericRec generic;
			public SizeMetricsRec metrics;
			private IntPtr @public;
		}

		[StructLayout (LayoutKind.Sequential)]
		public struct SizeMetricsRec
		{
			public ushort x_ppem;
			public ushort y_ppem;

			public IntPtr x_scale;
			public IntPtr y_scale;
			public IntPtr ascender;
			public IntPtr descender;
			public IntPtr height;
			public IntPtr max_advance;
		}
	}
}