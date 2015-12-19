using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using nginz;

namespace AnotherTestGame {
	class Program {
		static void Main (string[] args) {
			var config = new GameConfiguration {
				Width = 640,
				Height = 480,
				Fullscreen = false,
				FixedFramerate = false,
				FixedWindow = true,
				WindowTitle = "Another Test Game",
				TargetFramerate = 60,
				Vsync = VsyncMode.Off	
			};

			new MainGame (config).Run ();
		}
	}
}
