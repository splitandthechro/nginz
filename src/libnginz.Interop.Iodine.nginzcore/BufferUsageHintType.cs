using System;
using Iodine.Runtime;
using nginz.Common;
using OpenTK.Graphics.OpenGL4;

namespace nginz.Interop.Iodine.nginzcore
{
	public class BufferUsageHintType : IodineObject, ICanLog
	{
		readonly public static IodineTypeDefinition typeDef;

		static BufferUsageHintType () {
			typeDef = new BufferUsageHintTypeDefinition ("BufferUsageHint");
		}

		public BufferUsageHint Value;

		class BufferUsageHintTypeDefinition : IodineTypeDefinition, ICanLog {
			public BufferUsageHintTypeDefinition (string name) : base (name) { }

			// arguments [0]: IodineString
			public override IodineObject Invoke (VirtualMachine vm, IodineObject[] arguments) {

				// Read the enum name
				var name = arguments [0] as IodineString;

				// Return the BufferUsageHint
				return new BufferUsageHintType (name.Value);
			}
		}

		public BufferUsageHintType (string name) : base (typeDef) {

			// Try to parse the BufferUsageHint
			if (!Enum.TryParse<BufferUsageHint> (name, out Value))
				this.IodineError ("Invalid BufferUsageHint: {0}", name);
		}
	}
}

