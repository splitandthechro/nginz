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
								this.IodineError ("GLClearColor: Expected at least three arguments");
								return null;
							}
							this.IodineInfo ("Type: {0}", args[0].GetType ().Name);
							var fr = args[0] as IodineFloat;
							var fg = args[1] as IodineFloat;
							var fb = args[2] as IodineFloat;
							var r = (float)fr.Value;
							var g = (float)fg.Value;
							var b = (float)fb.Value;
							var a = 1.0f;
							if (args.Length == 4)
								a = (float)(args[3] as IodineFloat).Value;
							this.IodineInfo ("IodineColor: {{R:{0:F},G:{1:F},B:{2:F}}}", fr.Value, fg.Value, fb.Value);
							this.IodineInfo ("Color: {{R:{0},G:{1},B:{2},A:{3}}}", r, g, b, a);
							GL.ClearColor (r / 10, g / 10, b / 10, a / 10);
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

