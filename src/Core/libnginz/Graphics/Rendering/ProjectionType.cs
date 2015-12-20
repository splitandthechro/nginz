using System;

namespace nginz {

	/// <summary>
	/// The camera projection type.
	/// </summary>
	public enum ProjectionType {

		/// <summary>
		/// Perspective camera for 3D stuff.
		/// </summary>
		Perspective = 1 << 1,

		/// <summary>
		/// Orthographic camera for 2D stuff.
		/// </summary>
		Orthographic = 1 << 2,
	}
}
