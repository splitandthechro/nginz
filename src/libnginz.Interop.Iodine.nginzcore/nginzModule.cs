using System;
using Iodine.Runtime;

namespace nginz.Interop.Iodine.nginzcore
{
	[IodineBuiltinModule ("nginz")]
	public class NginzModule : IodineModule
	{
		public NginzModule () : base ("nginz") {
			SetAttribute ("GLBuffer", GLBufferType.typeDef);
			SetAttribute ("BufferTarget", BufferTargetType.typeDef);
			SetAttribute ("BufferUsageHint", BufferUsageHintType.typeDef);
		}
	}
}

