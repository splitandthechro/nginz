using System;
using nginz;

namespace FlappyMascot
{
	class MainClass
	{
		public static void Main (string[] args) {
			var conf = GameConfiguration.Default;
			using (var game = new MainGame (conf))
				game.Run ();
		}
	}
}
