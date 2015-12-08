using System;

namespace Iodine
{
	public class IodinePackage : IodineObject
	{
		public IodineMethod EntryPoint
		{
			set;
			get;
		}

		public IodinePackage () 
			: base (null) {

		}

		public void AddModule (IodineModule module)
		{
			this.SetAttribute (module.Name, module);
		}

	}
}

