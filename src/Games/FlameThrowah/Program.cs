using System;
using nginz;

namespace FlameThrowah
{
	class MainClass
	{
		public static void Main (string[] args) {
			var conf = new GameConfiguration {
				Width = 1000,
				Height = 1000,
				FixedFramerate = false,
				FixedWindow = true,
				WindowTitle = "nginz :: FlameThrowah"
			};
			using (var game = new MainGame (conf))
				game.Run ();
		}
	}
}
