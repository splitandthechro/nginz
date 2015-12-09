using System;

namespace nginz
{

	/// <summary>
	/// Drawable interface.
	/// </summary>
	public interface IDrawable
	{

		/// <summary>
		/// Draw.
		/// </summary>
		/// <param name="time">Time.</param>
		void Draw (GameTime time);
	}
}

