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
		/// Initializes a new instance of the <see cref="nginz.Resolution"/> struct.
		/// </summary>
		/// <param name="width">Width.</param>
		/// <param name="height">Height.</param>
		public Resolution (int width, int height) : this () {
			Width = width;
			Height = height;
		}

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

