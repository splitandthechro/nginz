using System;
using nginz.Common;

namespace nginz {

	/// <summary>
	/// Buffer interface.
	/// </summary>
	public interface IBuffer<T> : IBind, IPointTo<T> {
		int BufferSize { get; set; }
	}
}
