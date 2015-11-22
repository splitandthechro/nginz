using System;
using System.Collections.Generic;
using Iodine.Runtime;
using nginz.Common;
using OpenTK.Graphics.OpenGL4;

namespace nginz.Interop.Iodine.nginzcore
{
	public class GLType : IodineObject, ICanLog
	{
		readonly public static IodineTypeDefinition typeDef;

		static GLType () {
			typeDef = new GLTypeDefinition ("GL");
		}

		public GLType () : base (typeDef) { }

		class GLTypeDefinition : IodineTypeDefinition {
			public GLTypeDefinition (string name) : base (name) {
				SetMultiple (new Dictionary<string, InternalMethodCallback> {
					{
						"ClearColor",
						Callback ((vm, self, args) => {
							if (args.Length < 3) {
								this.IodineError ("GLClearColor: Expected at least three parameters");
								return null;
							}
							var colors = new float[4];
							for (var i = 0; i < (args.Length < 4 ? 3 : 4); i++) {
								TypeSwitch.On (args [i])
									.Case ((IodineFloat x) => colors [i] = (float)x.Value / 10f)
									.Case ((IodineInteger x) => colors [i] = x.Value)
									.Default (x => colors [i] = 1.0f);
							}
							GL.ClearColor (
								red: colors [0],
								green: colors [1],
								blue: colors [2],
								alpha: colors [3]
							);
							return null;
						})
					},
					{
						"Clear",
						Callback ((vm, self, args) => {
							if (args.Length == 0) {
								this.IodineError ("GLClear: Expected one parameter");
								return null;
							}
							var maskVal = args [0] as IodineInteger;
							var mask = (ClearBufferMask) maskVal.Value;
							GL.Clear (mask);
							return null;
						})
					}
				});
			}

			void SetMultiple (IDictionary<string, InternalMethodCallback> values) {
				foreach (var kvp in values)
					SetAttribute (kvp.Key, kvp.Value);
			}

			InternalMethodCallback Callback (
				Func<VirtualMachine,
				IodineObject,
				IodineObject[],
				IodineObject> func) {
				return new InternalMethodCallback (new IodineMethodCallback (func), this);
			}
		}
	}
}

