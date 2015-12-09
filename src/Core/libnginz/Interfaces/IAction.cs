using System;

namespace nginz
{
	public interface IAction
	{
		void Action (GameTime time, KeyboardBuffer keyboard, MouseBuffer mouse);
	}
}
