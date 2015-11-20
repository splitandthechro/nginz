using System;
using System.Collections.Generic;
using Iodine;
using Iodine.Runtime;
using nginz.Common;

namespace nginz.Interop.Iodine
{
	/// <summary>
	/// Iodine VM.
	/// </summary>
	public class IodineVM : ICanLog, ICanThrow
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
		/// Module for exposed functions.
		/// </summary>
		IodineModule exposedModule;

		/// <summary>
		/// Initializes a new instance of the <see cref="nginz.Interop.Iodine.IodineVM"/> class.
		/// </summary>
		public IodineVM () {
			log = new ErrorLog ();
			vm = new VirtualMachine (new IodineConfiguration ());
			modules = new List<IodineModule> ();
			exposedModule = new IodineModule ("nginz");
			vm.Globals.Add ("nginz", exposedModule);
		}

		/// <summary>
		/// Expose a function to the iodine vm.
		/// </summary>
		/// <param name="name">Name.</param>
		/// <param name="callback">Callback.</param>
		public void ExposeFunction (string name, IodineMethodCallback callback) {
			exposedModule.SetAttribute (vm, name, new InternalMethodCallback (callback, exposedModule));
		}

		/// <summary>
		/// Expose a function to the iodine vm.
		/// </summary>
		/// <param name="name">Name.</param>
		/// <param name="callback">Callback.</param>
		public void ExposeFunction (string name, Func<IodineObject[], IodineObject> callback) {
			ExposeFunction (name, new IodineMethodCallback ((vm, self, args) => callback (args)));
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

			return null;
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
