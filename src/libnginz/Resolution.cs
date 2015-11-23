using System;

namespace nginz
{
	/// <summary>
	/// Resolution.
	/// </summary>
	public struct Resolution
	{
		/// <summary>
		/// The width.
		/// </summary>
		public int Width;

		/// <summary>
		/// The height.
		/// </summary>
		public int Height;

		/// <summary>
		/// Gets the aspect ratio.
		/// </summary>
		/// <value>The aspect ratio.</value>
		public float AspectRatio {
			get {

				// Calculate the aspect ratio
				return (float) Width / (float) Height;
			}
		}
	}
}

