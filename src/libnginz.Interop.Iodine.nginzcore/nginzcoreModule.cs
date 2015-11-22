using System;
using Iodine.Runtime;

namespace nginz.Interop.Iodine.nginzcore
{
	[IodineBuiltinModule ("nginz")]
	public class nginzcoreModule : IodineModule
	{
		public nginzcoreModule () : base ("nginz") {
			var glBufferCallback = new InternalMethodCallback (createGlBuffer, this);
			SetAttribute ("GLBuffer", glBufferCallback);
		}

		static IodineObject createGlBuffer (VirtualMachine vm, IodineObject self, IodineObject[] args) {
			return new GLBufferType ();
		}
	}
}

