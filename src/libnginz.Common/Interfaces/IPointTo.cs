using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nginz.Common {
	public interface IPointTo<T> {
		void PointTo (T where);
		void PointTo (T where, T offset);
		void PointTo (T where, params T[] other);
	}
}
