using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using nginz.Common;

namespace nginz.Staging.Interfaces {
	public interface IActor : IAct, IDraw {
		void Initialize (ContentManager content);
	}
}
