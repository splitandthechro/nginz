using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nginz.Graphics {
	[Flags]
	public enum FboAttachment {
		DiffuseAttachment = 1,
		NormalAttachment = 1 << 2,
		SpecularAttachment = 1 << 3,
		DepthAttachment = 1 << 4,
	}
}
