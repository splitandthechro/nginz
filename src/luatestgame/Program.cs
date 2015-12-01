using System;
using nginz;

namespace luatestgame
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
				WindowTitle = "nginz :: Testgame (Lua scripting)",
			};
			var game = new MainGame (config);
			game.Run ();
		}
	}
}
