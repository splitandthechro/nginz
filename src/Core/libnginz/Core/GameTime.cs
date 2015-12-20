using System;

namespace nginz
{
	/// <summary>
	/// Gametime.
	/// Holds total time and delta time.
	/// </summary>
	public class GameTime
	{
		/// <summary>
		/// A GameTime object representing
		/// a total and elapsed timespan of zero time.
		/// </summary>
		public static GameTime ZeroTime;

		/// <summary>
		/// Initializes the <see cref="nginz.GameTime"/> class.
		/// </summary>
		static GameTime () {
			var zero = TimeSpan.Zero;
			ZeroTime = new GameTime (zero, zero);
		}

		/// <summary>
		/// Total amount of time the game has been running for.
		/// </summary>
		readonly public TimeSpan Total;

		/// <summary>
		/// The amount of time that elapsed since the last update.
		/// Use that for time-based calculations.
		/// </summary>
		readonly public TimeSpan Elapsed;

		/// <summary>
		/// Initializes a new instance of the <see cref="nginz.GameTime"/> class.
		/// </summary>
		/// <param name="total">Total.</param>
		/// <param name="elapsed">Elapsed.</param>
		public GameTime (TimeSpan total, TimeSpan elapsed) {
			Total = total;
			Elapsed = elapsed;
		}
	}
}

