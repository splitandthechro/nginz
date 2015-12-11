using System;
using nginz;

namespace FlameThrowah
{
	class MainClass
	{
		public static void Main (string[] args) {
			var conf = new GameConfiguration {
				Width = 480,
				Height = 480,
				FixedFramerate = false,
				FixedWindow = true,
				WindowTitle = "nginz :: FlameThrowah"
			};
			using (var game = new MainGame (conf))
				game.Run ();
		}
	}
}
