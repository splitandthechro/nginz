using System;
using System.Linq;

namespace FlappyMascot
{
	class TubeInstance {
		public Tube[] Tubes;
		public bool IsInView () {
			return Tubes.All (t => t.IsInView ());
		}
	}
}

