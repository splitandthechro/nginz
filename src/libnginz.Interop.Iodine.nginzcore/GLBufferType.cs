using System;
using Iodine.Runtime;
using nginz.Common;

namespace nginz.Interop.Iodine.nginzcore
{
	public class GLBufferType : IodineObject, ICanLog
	{
		readonly public static IodineTypeDefinition typeDef;

		static GLBufferType () {
			typeDef = new GLBufferTypeDefinition ("GLBuffer");
		}

		class GLBufferTypeDefinition : IodineTypeDefinition, ICanLog {
			public GLBufferTypeDefinition (string name) : base (name) { }

			// arguments [0]: BufferTargetType
			public override IodineObject Invoke (VirtualMachine vm, IodineObject[] arguments) {
				if (arguments.Length < 3)
					return null;
				
				var targetType = arguments [0] as BufferTargetType;
				this.Log ("Target type: {0}", targetType);
				return null;
			}
		}

		public GLBufferType () : base (typeDef) {
		}
	}
}

