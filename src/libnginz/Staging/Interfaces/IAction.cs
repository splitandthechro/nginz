using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nginz.Staging.Interfaces {
	public interface IAction {
		void Action (GameTime time, KeyboardBuffer keyboard, MouseBuffer mouse);
	}
}
