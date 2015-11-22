using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
		public VirtualMachine vm;

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
		/// Module that contains the nginz core.
		/// </summary>
		IodineModule nginzcore;

		/// <summary>
		/// The livereload watchers.
		/// </summary>
		List<FileSystemWatcher> livereloadWatchers;

		/// <summary>
		/// The livereload files.
		/// </summary>
		List<string> livereloadFiles;

		/// <summary>
		/// The livereload temp files.
		/// </summary>
		List<string> livereloadTempFiles;

		/// <summary>
		/// Initializes a new instance of the <see cref="nginz.Interop.Iodine.IodineVM"/> class.
		/// </summary>
		public IodineVM () {

			// Create the vm log
			log = new ErrorLog ();

			// Create the vm
			vm = new VirtualMachine (new IodineConfiguration ());

			// Initialize lists
			modules = new List<IodineModule> ();
			livereloadFiles = new List<string> ();
			livereloadWatchers = new List<FileSystemWatcher> ();
			livereloadTempFiles = new List<string> ();

			// Create a module to hold the functions exposed to the vm
			exposedModule = new IodineModule ("nginzgame");

			// Load nginz core module
			const string nginzcore_path = "libnginz.Interop.Iodine.nginzcore.dll";
			nginzcore = IodineModule.LoadExtensionModule ("nginz", nginzcore_path);

			// Add the module that holds the exposed functions to the globals
			vm.Globals.Add ("nginzgame", exposedModule);
			vm.Globals.Add ("nginz", nginzcore);

			if (nginzcore == null)
				this.Log ("Error loading nginzcore");

			if (nginzcore != null) {
				foreach (var attrib in nginzcore.Attributes) {
					this.Log ("Attrib: nginz/{0}", attrib.Key);
				}
			}
		}

		/// <summary>
		/// Hot-reload the vm.
		/// </summary>
		public void Hotreload () {

			// Clear all modules
			ClearModules ();

			// Create a new vm
			vm = new VirtualMachine (new IodineConfiguration ());

			// Add the module that holds the exposed functions to the globals
			vm.Globals.Add ("nginzgame", exposedModule);
			vm.Globals.Add ("nginz", nginzcore);
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
		/// Watch the specified directory.
		/// </summary>
		/// <param name="directory">Directory.</param>
		public void Watch (string directory) {

			// The full path of the directory
			directory = directory.EndsWith ("\\") ? directory : directory + "\\";
			var directorypath = Path.GetFullPath (directory);

			// Throw if the directory doesn't exist
			if (!Directory.Exists (directorypath))
				this.Throw ("Directory {0} doesn't exist.", directorypath);

			// Check if the directory isn't being watched currently
			if (livereloadWatchers.All (watcher => Path.GetFullPath (watcher.Path) != directorypath)) {

				// Create the watcher
				this.Log (directorypath);
				var watcher = CreateFilesystemWatcher (directorypath);

				// Add the watcher to the list of watchers
				livereloadWatchers.Add (watcher);

				// Log the directory name
				var directoryname = Directory.GetParent (directorypath).Name;
				this.Log ("Added {0} directory to live-reload watch.", directoryname);
			}
		}

		/// <summary>
		/// Observe the specified file.
		/// </summary>
		/// <param name="file">File.</param>
		public void Observe (string file) {

			// The full path of the file
			var filepath = Path.GetFullPath (file);

			// Get the directory of the path
			var directory = Path.GetFullPath (Path.GetDirectoryName (filepath));

			// Observe the directory
			Watch (directory);

			// Throw if the file doesn't exist
			if (!File.Exists (filepath))
				this.Throw ("File does not exist: {0}", filepath);

			// Delete cached file if it exists
			var tempfile = string.Format ("{0}.live", Path.GetFileNameWithoutExtension (filepath));
			if (File.Exists (tempfile))
				File.Delete (tempfile);

			// Add the file to the livereload files if it
			// isn't yet in the livereload list
			if (!livereloadFiles.Contains (filepath))
				livereloadFiles.Add (filepath);
		}

		/// <summary>
		/// Observe the specified files.
		/// </summary>
		/// <param name="files">Files.</param>
		public void Observe (params string[] files) {

			// Observe specified files
			foreach (var file in files)
				Observe (file);
		}

		/// <summary>
		/// Load an iodine module.
		/// </summary>
		/// <param name="path">Path.</param>
		public void LoadModule (string path) {

			// Load module from file
			IodineModule module;
			try {
				module = IodineModule.LoadModule (log, path);
			} catch (Exception ex) {
				this.Log (ex.Message);
				return;
			}

			// Throw if module is null
			if (module == null)
				throw new Exception (string.Format ("Could not load module: '{0}'", path));

			// Invoke module
			module.Invoke (vm, new IodineObject[] { });

			// Add module to module list
			modules.Add (module);
		}

		/// <summary>
		/// Loads specified iodine modules.
		/// </summary>
		/// <param name="paths">Paths.</param>
		public void LoadModules (params string[] paths) {

			// Load all specified modules
			foreach (var path in paths)
				LoadModule (path);
		}

		/// <summary>
		/// Create a filesystem watcher for a specific directory.
		/// </summary>
		/// <returns>The filesystem watcher.</returns>
		/// <param name="directory">Directory.</param>
		FileSystemWatcher CreateFilesystemWatcher (string directory) {

			// Create the watcher
			var fsw = new FileSystemWatcher (directory, "*.id");

			// Set the notify filters
			fsw.NotifyFilter = 0x0 // dummy for alignment
			| NotifyFilters.CreationTime
			| NotifyFilters.LastWrite
			| NotifyFilters.Size;

			// Subscribe to the changed event
			fsw.Changed += (sender, e) => {

				// Return if the changed file isn't in the list
				// of watched files
				if (!livereloadFiles.Contains (e.FullPath))
					return;

				// Create a temporary filename
				var filename = Path.GetFileNameWithoutExtension (e.FullPath);
				var tempfile = string.Format ("{0}.live", filename);

				// Add the temprary filename to the list of temprary files
				livereloadTempFiles.Add (Path.GetFullPath (tempfile));

				// Delete the temporary file with the same name,
				// if it already exists
				if (File.Exists (tempfile))
					File.Delete (tempfile);

				// Copy the new file to the temporary location
				File.Copy (e.FullPath, tempfile);

				// Reload the vm
				Hotreload ();

				// Reload the cached module
				LoadModule (tempfile);
			};

			// Enable raising events
			fsw.EnableRaisingEvents = true;

			// Return the watcher
			return fsw;
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
			foreach (var module in modules) {
				if (module.HasAttribute (function)) {
					try {
						return module.GetAttribute (function).Invoke (vm, args);
					} catch (UnhandledIodineExceptionException ex) {
						var originalException = (IodineException) ex.OriginalException;
						this.Log ("! {0}: {1}", function, originalException.Message);
					}
				}
			}

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

		/// <summary>
		/// Clears all modules.
		/// </summary>
		public void ClearModules () {

			// Iterate over all modules
			foreach (var module in modules) {

				// Clear all attributes
				module.Attributes.Clear ();
				module.ConstantPool.Clear ();
				module.Name = "";
			}

			// Clear modules
			modules.Clear ();
		}

		/// <summary>
		/// Cleans the trash up.
		/// </summary>
		public void Cleanup () {

			// Get rid of the vm
			// This is a really dirty way of doing it,
			// but currently there's no other way of doing it.
			vm = null;

			// Delete temporary files
			// Analysis disable once EmptyGeneralCatchClause
			foreach (var tmp in livereloadTempFiles) {
				try {
					File.Delete (tmp);
				} catch { }
			}
		}

		/// <summary>
		/// Releases unmanaged resources and performs other cleanup operations before the
		/// <see cref="nginz.Interop.Iodine.IodineVM"/> is reclaimed by garbage collection.
		/// </summary>
		~IodineVM () {

			// Clean everything up
			Cleanup ();
		}
	}
}
