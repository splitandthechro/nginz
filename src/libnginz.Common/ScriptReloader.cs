using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace nginz.Common
{
	public class ScriptReloader : ICanLog, ICanThrow
	{
		public Action<Script> LoadScript;
		public Action PauseGame;
		public Action ResumeGame;

		readonly List<FileSystemWatcher> LivereloadWatchers;
		readonly List<Script> LivereloadFiles;
		readonly string ScriptFilter;

		public ScriptReloader (string scriptFilter) {
			ScriptFilter = scriptFilter;
			LivereloadWatchers = new List<FileSystemWatcher> ();
			LivereloadFiles = new List<Script> ();
		}

		public static ScriptReloader CreateFor (string scriptFilter) {
			return new ScriptReloader (scriptFilter);
		}

		/// <summary>
		/// Observe the specified file.
		/// </summary>
		/// <param name="script">Lua script.</param>
		public void WatchFile (Script script) {

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
			if (!LivereloadFiles.Contains (script))
				LivereloadFiles.Add (script);
		}

		/// <summary>
		/// Watch the specified directory.
		/// </summary>
		/// <param name="directory">Directory.</param>
		public void WatchDirectory (string directory) {

			// The full path of the directory
			directory = directory.EndsWith ("\\") ? directory : directory + "\\";
			var directorypath = Path.GetFullPath (directory);

			// Throw if the directory doesn't exist
			if (!Directory.Exists (directorypath))
				this.Throw ("Directory {0} doesn't exist.", directorypath);

			// Check if the directory isn't being watched currently
			if (LivereloadWatchers.All (watcher => Path.GetFullPath (watcher.Path) != directorypath)) {

				// Create the watcher
				var watcher = CreateFilesystemWatcher (directorypath);

				// Add the watcher to the list of watchers
				LivereloadWatchers.Add (watcher);

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
			var fsw = new FileSystemWatcher (directory, ScriptFilter);

			// Set the notify filters
			fsw.NotifyFilter = 0x0 // dummy for alignment
				| NotifyFilters.CreationTime
				| NotifyFilters.LastWrite
				| NotifyFilters.Size;

			// Subscribe to the changed event
			fsw.Changed += (sender, e) => {

				// Return if the changed file isn't in the list
				// of watched files
				if (LivereloadFiles.All (s => s.FilePath != e.FullPath))
					return;

				// Pause the game
				if (PauseGame != null)
					PauseGame ();

				// Unload the script before reloading it
				ScriptEvents.Unload (LivereloadFiles.First (s => s.FilePath == e.FullPath));

				// Reload the module
				if (LoadScript != null)
					LoadScript (Script.FromFile (e.FullPath));

				// Resume the game
				if (ResumeGame != null)
					ResumeGame ();
			};

			// Enable raising events
			fsw.EnableRaisingEvents = true;

			// Return the watcher
			return fsw;
		}
	}
}

