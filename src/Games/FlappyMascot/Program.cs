using System;
using nginz;

namespace FlappyMascot
{
	class MainClass
	{
		public static void Main (string[] args) {
			var conf = new GameConfiguration {
				Width = 640,
				Height = 480,
				FixedWindow = true,
				FixedFramerate = false,
				Vsync = VsyncMode.Off,
				Fullscreen = false,
				WindowTitle = "nginz :: Flappy Mascot",
				ContentRoot = "../../assets"
			};
			using (var game = new MainGame (conf))
				game.Run ();
		}
	}
}
