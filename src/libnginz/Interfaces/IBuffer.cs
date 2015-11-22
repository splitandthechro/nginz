using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using nginz.Common;

namespace nginz {
	public interface IBuffer<T> : IBind, IPointTo<T> {
	}
}
