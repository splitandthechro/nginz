using System;

namespace nginz
{

	/// <summary>
	/// Sprite effects.
	/// </summary>
	[Flags]
	public enum SpriteEffects {

		/// <summary>
		/// No sprite effects.
		/// </summary>
		None = 0 << 1,

		/// <summary>
		/// Flip the texture horizontally.
		/// </summary>
		FlipHorizontal = 1 << 1,

		/// <summary>
		/// Flip the texture vertically.
		/// </summary>
		FlipVertical = 2 << 1
	}
}

