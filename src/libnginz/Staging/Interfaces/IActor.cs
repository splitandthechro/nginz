using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using nginz.Common;

namespace nginz.Staging.Interfaces {
	public interface IActor : IAction, IDraw {
		dynamic Stage { get; set; }

		void Initialize (ContentManager content);
	}
}
