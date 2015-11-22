using System;
using Iodine.Runtime;
using nginz.Common;

namespace nginz.Interop.Iodine.nginzcore
{
	public class GLBufferType : IodineObject, ICanLog
	{
		readonly static IodineTypeDefinition typeDef;

		static GLBufferType () {
			typeDef = new GLBufferTypeDefinition ("GLBuffer");
		}

		class GLBufferTypeDefinition : IodineTypeDefinition, ICanLog {
			public GLBufferTypeDefinition (string name) : base (name) { }
			public override IodineObject Invoke (VirtualMachine vm, IodineObject[] arguments) {
				this.Log ("Initializing GLBuffer");
				return null;
			}
		}

		public GLBufferType () : base (typeDef) {
			SetAttribute ("init", new InternalMethodCallback (init, this));
		}

		// args [0]: BufferTargetType
		// args [1]: IodineArray
		IodineObject init (VirtualMachine vm, IodineObject self, IodineObject[] args) {
			this.Log ("Initializing GLBuffer");
			//var iodinetarget = args [0] as BufferTargetType;
			return null;
		}
	}
}

