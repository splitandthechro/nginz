using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nginz.Common {
	public interface IBind<T> : IPointTo<T> {
		void Bind ();
		void Unbind ();
	}
}
