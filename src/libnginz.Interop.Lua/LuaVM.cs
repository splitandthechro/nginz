using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MoonSharp.Interpreter;
using nginz.Common;

namespace nginz.Interop.Lua
{

	/// <summary>
	/// Lua VM.
	/// </summary>
	public class LuaVM : ICanLog, ICanThrow
	{

		/// <summary>
		/// The game.
		/// </summary>
		readonly Game Game;

		/// <summary>
		/// The lua script.
		/// </summary>
		readonly Script Script;

		/// <summary>
		/// The livereload watchers.
		/// </summary>
		List<FileSystemWatcher> livereloadWatchers;

		/// <summary>
		/// The livereload files.
		/// </summary>
		List<LuaScript> livereloadFiles;

		/// <summary>
		/// Script state changed event arguments.
		/// </summary>
		public delegate void ScriptStateChangedEventArgs (LuaScript script);

		/// <summary>
		/// Occurs when a script gets loaded.
		/// </summary>
		public event ScriptStateChangedEventArgs LoadScript;

		/// <summary>
		/// Occurs when a script gets unloaded.
		/// </summary>
		public event ScriptStateChangedEventArgs UnloadScript;

		/// <summary>
		/// Initializes a new instance of the <see cref="nginz.Interop.Lua.LuaVM"/> class.
		/// </summary>
		/// <param name="game">Game.</param>
		public LuaVM (Game game) {
			Game = game;
			Script = new Script ();
			livereloadFiles = new List<LuaScript> ();
			livereloadWatchers = new List<FileSystemWatcher> ();
			RegisterGlobals ();
			for (var i = 0; i < Script.Globals.Length; i++)
				this.Log ("Lua global: {0}", Script.Globals [i].ToString ());
			LoadScript += delegate { };
			UnloadScript += delegate { };
		}

		/// <summary>
		/// Load the specified lua script.
		/// </summary>
		/// <param name="script">Script.</param>
		/// <param name="liveReload">Whether the script should live-reload on changes.</param>
		public LuaVM Load (LuaScript script, bool liveReload = false) {
			try {
				Script.DoString (script.Source);
				LoadScript (script);
			} catch (InvalidOperationException) {
			} catch (Exception e) {
				this.Log ("Syntax error: {0}", e.Message);
			}
			if (script.HasValidPath && liveReload) {
				this.Log ("Observing {0}", Path.GetFileName (script.FilePath));
				WatchFile (script);
			} else if (!script.HasValidPath && liveReload)
				this.Log ("Live reload not possible: No path information available");
			return this;
		}

		/// <summary>
		/// Call the specified function with the specified args.
		/// </summary>
		/// <param name="function">Function.</param>
		/// <param name="args">Arguments.</param>
		public object Call (string function, params object[] args) {
			object retVal = new object ();
			try {
				retVal = Script.Call (Script.Globals [function], args);
			} catch (InvalidOperationException) {
			} catch (ScriptRuntimeException e) {
				this.Log (e.Message);
			} catch (Exception e) {
				this.Log (e.Message);
			}
			return retVal;
		}

		/// <summary>
		/// Call the specified function with the specified args.
		/// </summary>
		/// <param name="function">Function.</param>
		/// <param name="args">Arguments.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public T Call<T> (string function, params object[] args) {
			return ((DynValue) Call (function, args)).ToObject<T> ();
		}

		/// <summary>
		/// Register a class.
		/// </summary>
		/// <param name="name">Name.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public void RegisterClass<T> (string name) where T : class, new() {
			UserData.RegisterType<T> ();
			Script.Globals [name] = new T ();
		}

		public void RegisterClass<T> (string name, params object[] args) where T : class {
			UserData.RegisterType<T> ();
			Script.Globals [name] = Activator.CreateInstance (typeof(T), args);
		}

		/// <summary>
		/// Observe the specified file.
		/// </summary>
		/// <param name="script">Lua script.</param>
		void WatchFile (LuaScript script) {

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
			var fsw = new FileSystemWatcher (directory, "*.lua");

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
				UnloadScript (livereloadFiles.First (s => s.FilePath == e.FullPath));

				// Reload the module
				Load (LuaScript.FromFile (e.FullPath));

				// Resume the game
				Game.Resume ();
			};

			// Enable raising events
			fsw.EnableRaisingEvents = true;

			// Return the watcher
			return fsw;
		}

		/// <summary>
		/// Register a class with a specific casing.
		/// </summary>
		/// <param name="nameCase">Name case.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		void RegisterClassWithCasing<T> (ClassNameCasing nameCase = ClassNameCasing.Default)
			where T : class, new() {
			UserData.RegisterType<T> ();
			var instance = new T ();
			var name = instance.GetType ().Name;
			if (nameCase.HasFlag (ClassNameCasing.Default))
				Script.Globals [name] = instance;
			if (nameCase.HasFlag (ClassNameCasing.Lowercase))
				Script.Globals [name.ToLowerInvariant ()] = instance;
			if (nameCase.HasFlag (ClassNameCasing.Uppercase))
				Script.Globals [name.ToUpperInvariant ()] = instance;
		}

		/// <summary>
		/// Register globals.
		/// </summary>
		void RegisterGlobals () {

			// OpenTK GL4 wrapper
			// Register uppercase and lowercase versions
			RegisterClassWithCasing<GL> ((ClassNameCasing) 12);

			// OpenTK Vector2 wrapper
			RegisterClass<Vector2> ("vec2");

			// OpenTK Vector3 wrapper
			RegisterClass<Vector3> ("vec3");

			// OpenTK Color4 wrapper
			RegisterClass<Color> ("color");

			// Nginz Game wrapper
			RegisterClass<Nginz> ("game", Game);

			// Nginz Common.ContentManager wrapper
			RegisterClass<Content> ("content", Game.Content);

			// Nginz Texture2D wrapper
			RegisterClass<Texture2D> ("tex2");

			// Nginz SpriteBatch wrapper
			RegisterClass<SpriteBatch> ("spriteBatch", Game.SpriteBatch);
		}

		/// <summary>
		/// Class name casing.
		/// </summary>
		enum ClassNameCasing {
			Default = 1 << 1,
			Uppercase = 1 << 2,
			Lowercase = 1 << 3,
		}
	}
}

