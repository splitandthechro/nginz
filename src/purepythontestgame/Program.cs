using System;
using nginz;
using nginz.Interop.IronPython;

namespace purepythontestgame
{
	class MainClass
	{
		public static void Main (string[] args) {
			var conf = new GameConfiguration {
				FixedFramerate = true,
				TargetFramerate = 60,
				FixedWindow = true,
				Fullscreen = false,
				Width = 1280,
				Height = 720,
				Vsync = VsyncMode.Off,
				WindowTitle = "nginz :: Game"
			};
			PythonGame
				.Create ("../../assets")
				.Load ("game")
				.Run ("MainGame", conf);
		}
	}
}
