using System;
using OpenTK.Input;

namespace nginz
{

	/// <summary>
	/// Key state.
	/// </summary>
	public class KeyState
	{
		public Key Key;
		public bool IsDown;
		public bool IsFrozen;

		public static KeyState FromKey (Key key) {
			return new KeyState {
				Key = key,
				IsDown = false,
				IsFrozen = false,
			};
		}
	}
}

