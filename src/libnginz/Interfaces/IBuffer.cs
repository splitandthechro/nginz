using System;
using nginz.Common;

namespace nginz {

	/// <summary>
	/// Buffer interface.
	/// </summary>
	[CLSCompliant (false)]
	public interface IBuffer<T> : IBind, IPointTo<T> {
		int BufferSize { get; set; }
	}
}
