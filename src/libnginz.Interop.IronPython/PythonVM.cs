using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using nginz.Common;

namespace nginz.Interop.IronPython
{
	public class PythonVM : ICanLog, ICanThrow
	{
		readonly Game Game;
		readonly ScriptEngine Engine;
		readonly ScriptScope Scope;
		readonly List<FileSystemWatcher> livereloadWatchers;
		readonly List<Script> livereloadFiles;

		CompiledCode CompiledSource;

		public dynamic this [string name] {
			get { return Call (name); }
		}

		public PythonVM (Game game) {
			Game = game;
			Game.Content.RegisterAssetProvider<PythonScript> (typeof(PythonScriptProvider));
			Engine = Python.CreateEngine ();
			Scope = Engine.CreateScope ();
			livereloadWatchers = new List<FileSystemWatcher> ();
			livereloadFiles = new List<Script> ();
			SetupGlobals ();
		}

		public PythonVM Load (Script script, bool liveReload = false) {
			var source = Engine.CreateScriptSourceFromString (script.Source);
			try {
				CompiledSource = source.Compile ();
				CompiledSource.Execute (Scope);
				ScriptEvents.Load (script);
			} catch (Exception e) {
				this.Log (e.Message);
			}
			if (script.HasValidPath && liveReload) {
				this.Log ("Observing {0}", Path.GetFileName (script.FilePath));
				WatchFile (script);
			} else if (!script.HasValidPath && liveReload)
				this.Log ("Live reload not possible: No path information available");
			return this;
		}

		/// <summary>
		/// Observe the specified file.
		/// </summary>
		/// <param name="script">Lua script.</param>
		void WatchFile (Script script) {

			// The full path of the file
			var filepath = Path.GetFullPath (script.FilePath);

			// Get the directory of the path
			var directory = Path.GetFullPath (Path.GetDirectoryName (filepath));

			// Observe the directory
			WatchDirectory (directory);

			// Throw if the file doesn't exist
			if (!File.Exists (filepath))
				this.Throw ("File does not exist: {0}", filepath);

			// Add the file to the livereload files if it
			// isn't yet in the livereload list
			if (!livereloadFiles.Contains (script))
				livereloadFiles.Add (script);
		}

		/// <summary>
		/// Watch the specified directory.
		/// </summary>
		/// <param name="directory">Directory.</param>
		void WatchDirectory (string directory) {

			// The full path of the directory
			directory = directory.EndsWith ("\\") ? directory : directory + "\\";
			var directorypath = Path.GetFullPath (directory);

			// Throw if the directory doesn't exist
			if (!Directory.Exists (directorypath))
				this.Throw ("Directory {0} doesn't exist.", directorypath);

			// Check if the directory isn't being watched currently
			if (livereloadWatchers.All (watcher => Path.GetFullPath (watcher.Path) != directorypath)) {

				// Create the watcher
				var watcher = CreateFilesystemWatcher (directorypath);

				// Add the watcher to the list of watchers
				livereloadWatchers.Add (watcher);

				// Log the directory name
				var directoryname = Directory.GetParent (directorypath).Name;
				this.Log ("Observing {0} directory", directoryname);
			}
		}

		/// <summary>
		/// Create a filesystem watcher for a specific directory.
		/// </summary>
		/// <returns>The filesystem watcher.</returns>
		/// <param name="directory">Directory.</param>
		FileSystemWatcher CreateFilesystemWatcher (string directory) {

			// Create the watcher
			var fsw = new FileSystemWatcher (directory, "*.py");

			// Set the notify filters
			fsw.NotifyFilter = 0x0 // dummy for alignment
				| NotifyFilters.CreationTime
				| NotifyFilters.LastWrite
				| NotifyFilters.Size;

			// Subscribe to the changed event
			fsw.Changed += (sender, e) => {

				// Return if the changed file isn't in the list
				// of watched files
				if (livereloadFiles.All (s => s.FilePath != e.FullPath))
					return;

				// Pause the game
				Game.Pause ();

				// Unload the script before reloading it
				ScriptEvents.Unload (livereloadFiles.First (s => s.FilePath == e.FullPath));

				// Reload the module
				Load (Script.FromFile (e.FullPath));

				// Resume the game
				Game.Resume ();
			};

			// Enable raising events
			fsw.EnableRaisingEvents = true;

			// Return the watcher
			return fsw;
		}


		public dynamic Call (string name) {
			var func = Scope.GetVariable (name);
			return func;
		}

		void SetupGlobals () {
			Import ("clr");
			AddReference ("System.Drawing");
			AddReference ("OpenTK");
			AddReference ("libnginz");
			AddReference ("libnginz.Common");
			Scope.SetVariable ("game", Game);
		}

		void Import (string name) {
			Engine.Execute (string.Format ("import {0}", name), Scope);
		}

		void ImportFrom (string source, string name) {
			Engine.Execute (string.Format ("from {0} import {1}", source, name));
		}

		void AddReference (string name) {
			Engine.Execute (string.Format ("clr.AddReference (\"{0}\")", name), Scope);
		}
	}
}

