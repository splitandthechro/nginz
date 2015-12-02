using System;
using nginz;
using nginz.Interop.IronPython;

namespace purepythontestgame
{
	class MainClass
	{
		public static void Main (string[] args) {
			var conf = new GameConfiguration {
				FixedFramerate = false,
				FixedWindow = true,
				Fullscreen = false,
				Width = 640,
				Height = 480,
				Vsync = VsyncMode.Off,
				WindowTitle = "nginz :: Pure python testgame"
			};
			PythonGame
				.Create ("../../assets")
				.Load ("game")
				.Run ("MainGame", conf);
		}
	}
}
