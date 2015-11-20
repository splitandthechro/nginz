using System;
using Iodine.Runtime;

namespace ModuleReflection
{
	public class IodineMethodBuilder : IodineObject
	{
		public static IodineTypeDefinition MethodBuilderTypeDef = new IodineTypeDefinition ("MethodBuilder");


		public IodineMethodBuilder (IodineMethod method)
			: base (MethodBuilderTypeDef)
		{
			//this.internalValue = method;
		}

		private IodineObject emit (VirtualMachine vm, IodineObject self, IodineObject[] args)
		{
			return null;
		}

		private IodineObject createLabel (VirtualMachine vm, IodineObject self, IodineObject[] args)
		{
			return null;
		}


	}
}

