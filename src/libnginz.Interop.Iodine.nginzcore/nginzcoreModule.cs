using System;
using Iodine.Runtime;

namespace nginz.Interop.Iodine.nginzcore
{
	[IodineBuiltinModule ("nginz")]
	public class nginzcoreModule : IodineModule
	{
		public nginzcoreModule () : base ("nginz") {
			SetAttribute ("GLBuffer", GLBufferType.typeDef);
			SetAttribute ("BufferTarget", BufferTargetType.typeDef);
		}
	}
}

