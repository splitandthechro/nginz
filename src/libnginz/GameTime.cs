using System;

namespace nginz
{
	/// <summary>
	/// Gametime.
	/// Holds total time and delta time.
	/// </summary>
	public class GameTime
	{
		public static GameTime ZeroTime;

		static GameTime () {
			var zero = TimeSpan.Zero;
			ZeroTime = new GameTime (zero, zero);
		}

		readonly public TimeSpan Total;
		readonly public TimeSpan Elapsed;

		public GameTime (TimeSpan total, TimeSpan elapsed) {
			Total = total;
			Elapsed = elapsed;
		}
	}
}

