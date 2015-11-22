using System;
using Iodine.Runtime;
using nginz.Common;
using OpenTK.Graphics.OpenGL4;

namespace nginz.Interop.Iodine.nginzcore
{
	public class BufferTargetType : IodineObject
	{
		readonly public static IodineTypeDefinition typeDef;

		static BufferTargetType () {
			typeDef = new BufferTargetTypeDefinition ("BufferTarget");
		}

		class BufferTargetTypeDefinition : IodineTypeDefinition {
			public BufferTargetTypeDefinition (string name) : base (name) {
				this.AutoimplementEnum<BufferTarget> ();
			}
		}

		public BufferTargetType () : base (typeDef) {
		}
	}
}

