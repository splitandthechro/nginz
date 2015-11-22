using System;
using Iodine.Runtime;
using nginz.Common;
using OpenTK.Graphics.OpenGL4;

namespace nginz.Interop.Iodine.nginzcore
{
	public class ClearBufferMaskType : IodineObject, ICanLog
	{
		readonly public static IodineTypeDefinition typeDef;

		static ClearBufferMaskType () {
			typeDef = new ClearBufferMaskTypeDefinition ("ClearBufferMask");
		}

		public ClearBufferMask Value;

		class ClearBufferMaskTypeDefinition : IodineTypeDefinition, ICanLog {
			public ClearBufferMaskTypeDefinition (string name) : base (name) {
				this.AutoimplementEnum<ClearBufferMask> ();
			}
		}

		public ClearBufferMaskType () : base (typeDef) {
		}
	}
}

