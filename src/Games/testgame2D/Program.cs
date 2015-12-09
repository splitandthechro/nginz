using System;
using nginz;

namespace testgame2D {
	class Program {
		static void Main (string[] args) {
			var conf = new GameConfiguration {
				Width = 640,
				Height = 480,
				WindowTitle = "2D Game",
				FixedWindow = false,
				Vsync = VsyncMode.Off,
				FixedFramerate = false,
				TargetFramerate = 60,
			};
			var game = new MainGame (conf);
			game.Run ();
		}
	}
}
