using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MoonSharp.Interpreter;
using nginz.Common;
using NginzScript = nginz.Common.Script;
using HostedLuaScript = MoonSharp.Interpreter.Script;

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
		readonly HostedLuaScript Script;

		readonly ScriptReloader Reloader;

		/// <summary>
		/// Initializes a new instance of the <see cref="nginz.Interop.Lua.LuaVM"/> class.
		/// </summary>
		/// <param name="game">Game.</param>
		public LuaVM (Game game) {
			Game = game;
			Game.Content.RegisterAssetProvider<LuaScript> (typeof(LuaScriptProvider));
			Script = new HostedLuaScript ();
			Reloader = new ScriptReloader ("*.lua");
			Reloader.LoadScript = Load;
			Reloader.PauseGame = Game.Pause;
			Reloader.ResumeGame = Game.Resume;
			RegisterGlobals ();
		}

		public void Load (NginzScript script) {
			try {
				Script.DoString (script.Source);
				ScriptEvents.Load (script);
			} catch (InvalidOperationException) {
			} catch (Exception e) {
				this.Log ("Syntax error: {0}", e.Message);
			}
		}

		/// <summary>
		/// Load the specified lua script.
		/// </summary>
		/// <param name="script">Script.</param>
		public LuaVM LoadLive (NginzScript script) {
			Load (script);
			if (script.HasValidPath) {
				this.Log ("Observing {0}", Path.GetFileName (script.FilePath));
				Reloader.WatchFile (script);
			} else if (!script.HasValidPath)
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

