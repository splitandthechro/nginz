using System;
using nginz;

namespace pythontestgame
{
	class MainClass
	{
		public static void Main (string[] args) {
			var config = new GameConfiguration {
				FixedFramerate = false,
				FixedWindow = true,
				Fullscreen = false,
				Width = 640,
				Height = 480,
				Vsync = VsyncMode.Off,
				WindowTitle = "nginz :: Testgame (Python scripting)",
			};
			var game = new MainGame (config);
			game.Run ();
		}
	}
}
