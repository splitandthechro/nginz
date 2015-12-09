using System;

namespace nginz
{

	/// <summary>
	/// Updatable interface.
	/// </summary>
	public interface IUpdatable
	{

		/// <summary>
		/// Update.
		/// </summary>
		/// <param name="time">Time.</param>
		void Update (GameTime time);
	}
}

