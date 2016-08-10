using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using nginz;

namespace particlestest {
	class Program {
		static void Main (string[] args) {
			var config = new GameConfiguration {
				Width = 800,
				Height = 600,
				WindowTitle = "Particles Test",
				FixedWindow = true,
				Vsync = VsyncMode.Off,
				FixedFramerate = false,
				Fullscreen = false,
				TargetFramerate = 60,
                ContentRoot = "../../assets",
            };

			new MainGame (config).Run ();
		}
	}
}
