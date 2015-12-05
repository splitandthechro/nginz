using System;
using OpenTK;
using SharpFont;
namespace nginz
{
	public class Font
	{
		//Size of font textures (in pixels)
		const int TEXTURE_SIZE = 512;

		/// <summary>
		/// Global FT_Library instance.
		/// </summary>
		static Library freetype;
		static Font()
		{
			freetype = new Library ();
		}

		Face face;
		public Font (string filename, float emSize) 
		{
			face = new Face (freetype, filename);
			face.SetCharSize (new Fixed26Dot6 (emSize), new Fixed26Dot6(0), 96, 96);
		}
	}
}

