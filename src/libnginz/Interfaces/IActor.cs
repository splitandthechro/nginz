using System;
using nginz.Common;

namespace nginz
{

	[CLSCompliant (false)]
	public interface IActor : IAction, IDrawable2D {
		dynamic Stage { get; set; }

		void Initialize (ContentManager content);
	}
}
