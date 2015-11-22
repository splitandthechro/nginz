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

		class BufferUsageHintTypeDefinition : IodineTypeDefinition, ICanLog {
			public BufferUsageHintTypeDefinition (string name) : base (name) {
				this.AutoimplementEnum<BufferUsageHint> ();
			}
		}

		public BufferUsageHintType () : base (typeDef) {
		}
	}
}

