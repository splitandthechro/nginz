using System;
using Iodine.Runtime;
using nginz.Common;
using OpenTK.Graphics.OpenGL4;

namespace nginz.Interop.Iodine.nginzcore
{
	public class BufferTargetType : IodineObject, ICanLog
	{
		readonly static IodineTypeDefinition typeDef;

		static BufferTargetType () {
			typeDef = new IodineTypeDefinition ("GLBuffer");
		}

		public BufferTargetType () : base (typeDef) {
			var names = Enum.GetNames (typeof(BufferTarget));
			foreach (var name in names) {
				this.Log ("Registering {{{0} => {1}}}", name, BufferTargetValue (name));
				SetAttribute (name, BufferTargetValue (name));
			}
		}

		static IodineInteger BufferTargetValue (string name) {
			return new IodineInteger ((long)Enum.Parse (typeof (BufferTarget), name));
		}
	}
}

