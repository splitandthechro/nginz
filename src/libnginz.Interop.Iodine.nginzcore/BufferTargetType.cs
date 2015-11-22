using System;
using Iodine.Runtime;
using nginz.Common;
using OpenTK.Graphics.OpenGL4;

namespace nginz.Interop.Iodine.nginzcore
{
	public class BufferTargetType : IodineObject, ICanLog
	{
		readonly public static IodineTypeDefinition typeDef;

		static BufferTargetType () {
			typeDef = new BufferTargetTypeDefinition ("BufferTarget");
		}

		public BufferTarget Value;

		class BufferTargetTypeDefinition : IodineTypeDefinition, ICanLog {
			public BufferTargetTypeDefinition (string name) : base (name) { }

			// arguments [0]: IodineString
			public override IodineObject Invoke (VirtualMachine vm, IodineObject[] arguments) {

				// Read the enum name
				var name = arguments [0] as IodineString;

				// Return the BufferTarget
				return new BufferTargetType (name.Value);
			}
		}

		public BufferTargetType (string name) : base (typeDef) {

			// Try to parse the BufferTargetType
			if (!Enum.TryParse<BufferTarget> (name, out Value))
				this.IodineError ("Invalid BufferTarget: {0}", name);
		}
	}
}

