using System;
using System.Collections.Generic;
using Iodine;
using Iodine.Compiler;
using Iodine.Runtime;

namespace splitandthechro.nginz.Interop.Iodine
{
	/// <summary>
	/// Iodine VM.
	/// </summary>
	public class IodineVM
	{
		/// <summary>
		/// The virtual machine.
		/// </summary>
		readonly public VirtualMachine vm;

		/// <summary>
		/// The error log.
		/// </summary>
		readonly public ErrorLog log;

		/// <summary>
		/// Loaded iodine modules.
		/// </summary>
		readonly List<IodineModule> modules;

		/// <summary>
		/// Initializes a new instance of the <see cref="splitandthechro.nginz.Interop.Iodine.IodineVM"/> class.
		/// </summary>
		public IodineVM () {
			log = new ErrorLog ();
			vm = new VirtualMachine (new IodineConfiguration ());
			modules = new List<IodineModule> ();
		}

		/// <summary>
		/// Load an iodine module.
		/// </summary>
		/// <param name="path">Path.</param>
		public void LoadModule (string path) {

			// Load module from file
			var module = IodineModule.LoadModule (log, path);

			// Throw if module is null
			if (module == null)
				throw new Exception (string.Format ("Could not load module: '{0}'", path));

			// Invoke module
			module.Invoke (vm, new IodineObject[] { });

			// Add module to module list
			modules.Add (module);
		}

		/// <summary>
		/// Invoke the specified function.
		/// </summary>
		/// <returns>The result of the function.</returns>
		/// <param name="function">The function.</param>
		/// <param name="args">The function arguments.</param>
		public IodineObject Invoke (string function, params IodineObject[] args) {

			// Count how many times the function is defined
			int count = 0;
			foreach (var module in modules)
				if (module.HasAttribute (function))
					count++;

			// Throw if the function is undefined
			if (count == 0)
				throw new Exception (string.Format ("Undefined attribute: '{0}'!", function));

			// Throw if the function is defined more than once
			if (count > 1)
				throw new Exception (string.Format ("Attribute '{0}' is defined more than once!", function));

			// Invoke the function
			foreach (var module in modules)
				if (module.HasAttribute (function))
					return module.GetAttribute (function).Invoke (vm, args);

			return (IodineObject)null;
		}

		/// <summary>
		/// Invoke the specified function as the specified type.
		/// </summary>
		/// <returns>The result of the function.</returns>
		/// <param name="function">The function.</param>
		/// <param name="args">The function arguments.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public T InvokeAs<T> (string function, params IodineObject[] args) where T : class {
			return Invoke (function, args) as T;
		}
	}
}
