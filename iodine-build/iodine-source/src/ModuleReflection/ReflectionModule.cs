using System;
using System.IO;
using Iodine.Runtime;
using Iodine;

namespace ModuleReflection
{
	[IodineBuiltinModule ("reflection")]
	public class ReflectionModule : IodineModule
	{
		public ReflectionModule ()
			: base ("reflection")
		{
			SetAttribute ("getBytecode", new InternalMethodCallback (getBytecode, this));
			SetAttribute ("hasAttribute", new InternalMethodCallback (hasAttribute, this));
			SetAttribute ("setAttribute", new InternalMethodCallback (setAttribute, this));
			SetAttribute ("getAttributes", new InternalMethodCallback (getAttributes, this));
			SetAttribute ("getInterfaces", new InternalMethodCallback (getInterfaces, this));
			SetAttribute ("loadModule", new InternalMethodCallback (loadModule, this));
			SetAttribute ("compileModule", new InternalMethodCallback (compileModule, this));
			SetAttribute ("MethodBuilder", new InternalMethodCallback (loadModule, this));
			SetAttribute ("Opcode", IodineOpcode.OpcodeTypeDef);
		}

		private IodineObject hasAttribute (VirtualMachine vm, IodineObject self, IodineObject[] args)
		{
			if (args.Length < 2) {
				vm.RaiseException (new IodineArgumentException (2));
				return null;
			}
			IodineObject o1 = args [0];
			IodineString str = args [1] as IodineString;
			if (str == null) {
				vm.RaiseException (new IodineTypeException ("Str"));
				return null;
			}
			return IodineBool.Create (o1.HasAttribute (str.Value));
		}

		private IodineObject getAttributes (VirtualMachine vm, IodineObject self, IodineObject[] args)
		{
			if (args.Length < 1) {
				vm.RaiseException (new IodineArgumentException (1));
				return null;
			}
			IodineObject o1 = args [0];
			IodineHashMap map = new IodineHashMap ();
			foreach (string key in o1.Attributes.Keys) {
				map.Set (new IodineString (key), o1.Attributes [key]);
			}
			return map;
		}

		private IodineObject getInterfaces (VirtualMachine vm, IodineObject self, IodineObject[] args)
		{
			if (args.Length < 1) {
				vm.RaiseException (new IodineArgumentException (1));
				return null;
			}
			IodineObject o1 = args [0];
			IodineList list = new IodineList (o1.Interfaces.ToArray ());
			return list;
		}

		private IodineObject setAttribute (VirtualMachine vm, IodineObject self, IodineObject[] args)
		{
			if (args.Length < 3) {
				vm.RaiseException (new IodineArgumentException (2));
				return null;
			}
			IodineObject o1 = args [0];
			IodineString str = args [1] as IodineString;
			if (str == null) {
				vm.RaiseException (new IodineTypeException ("Str"));
				return null;
			}
			o1.SetAttribute (str.Value, args [2]);
			return null;
		}

		private IodineObject loadModule (VirtualMachine vm, IodineObject self, IodineObject[] args)
		{
			IodineString pathStr = args [0] as IodineString;
			IodineModule module = IodineModule.LoadModule (new ErrorLog (), pathStr.Value);
			module.Initializer.Invoke (vm, new IodineObject[] { });
			return module;
		}

		private IodineObject compileModule (VirtualMachine vm, IodineObject self, IodineObject[] args)
		{
			IodineString pathStr = args [0] as IodineString;
			return IodineModule.CompileModuleFromSource (new ErrorLog (), pathStr.Value);
		}

		private IodineObject getBytecode (VirtualMachine vm, IodineObject self, IodineObject[] args)
		{
			
			IodineMethod method = args [0] as IodineMethod;

			if (method == null && args [0] is IodineClosure) {
				method = ((IodineClosure)args [0]).Target;
			}

			IodineList ret = new IodineList (new IodineObject[] { });

			foreach (Instruction ins in method.Body) {
				ret.Add (new IodineInstruction (method, ins));
			}

			return ret;
		}

		private IodineObject methodBuilder (VirtualMachine vm, IodineObject self, IodineObject[] args)
		{
			return null;
		}
	}
}

