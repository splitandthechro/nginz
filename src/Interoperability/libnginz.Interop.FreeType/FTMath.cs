using System;

namespace nginz.Interop.FreeType
{
	public static class FTMath
	{
		//FT26Dot6 Conversions to/from floating point
		public static float From26Dot6(IntPtr ft)
		{
			return (int)ft / 64f;
		}
		public static IntPtr To26Dot6(float input)
		{
			return new IntPtr ((int)(input * 64));
		}
	}
}

