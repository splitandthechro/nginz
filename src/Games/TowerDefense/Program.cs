using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using nginz;

namespace TowerDefense {
	class Program {
		static void Main (string[] args) {
			int tilesW = 26;
			int tilesH = 18;
			int UITilesW = 4;
			int UITilesH = 4;
			int sizeTilesW = 16;
			int sizeTilesH = 16;
			int scale = 2;

			var conf = new GameConfiguration {
				Width = ((tilesW * sizeTilesW) + (UITilesW * sizeTilesW)) * scale,
				Height = ((tilesH * sizeTilesH) + (UITilesH * sizeTilesH)) * scale,
				WindowTitle = "Tower Defense Game",
				FixedWindow = true,
				Vsync = VsyncMode.Off,
				FixedFramerate = false,
				TargetFramerate = 60,
			};

			new MainGame (conf).Run ();
		}
	}
}
