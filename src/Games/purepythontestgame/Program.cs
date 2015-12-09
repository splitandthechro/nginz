using System;
using nginz;
using nginz.Scripting.Python;

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
				Width = 640,
				Height = 480,
				Vsync = VsyncMode.Off,
				WindowTitle = "nginz :: Game"
			};
			PythonGame
				.Create ("../../assets")
				.Load ("game2")
				.Run ("MainGame", conf);
		}
	}
}
