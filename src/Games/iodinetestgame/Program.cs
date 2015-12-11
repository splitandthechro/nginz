using System;
using nginz;

namespace iodinetestgame
{
	class MainClass
	{
		public static void Main (string[] args) {
			var conf = new GameConfiguration {
				Width = 640,
				Height = 480,
				FixedFramerate = false,
				Fullscreen = false,
				FixedWindow = true,
				Vsync = VsyncMode.Off,
				WindowTitle = "nginz :: Iodine Test Game"
			};
			using (var game = new MainGame (conf))
				game.Run ();
		}
	}
}
