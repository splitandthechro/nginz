using System;
using System.IO;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using nginz.Common;

namespace nginz.Interop.IronPython
{
	public class PythonVM : ICanLog, ICanThrow
	{
		readonly public ScriptEngine Engine;
		readonly public ScriptScope Scope;

		readonly Game Game;
		readonly ScriptReloader Reloader;

		public dynamic this [string name] {
			get { return Call (name); }
		}

		public PythonVM (Game game = null) {
			Game = game;
			Engine = Python.CreateEngine ();
			Scope = Engine.CreateScope ();
			Reloader = new ScriptReloader ("*.py");
			Reloader.LoadScript = Load;
			if (Game != null) {
				Reloader.PauseGame = Game.Pause;
				Reloader.ResumeGame = Game.Resume;
				Game.Content.RegisterAssetProvider<PythonScript> (typeof(PythonScriptProvider));
			}
			SetupGlobals ();
		}

		public void Load (Script script) {
			var source = Engine.CreateScriptSourceFromString (script.Source);
			try {
				var compiled = source.Compile ();
				compiled.Execute (Scope);
				ScriptEvents.Load (script);
			} catch (Exception e) {
				this.Log (e.Message);
			}
		}

		public PythonVM LoadLive (Script script) {
			Load (script);
			if (script.HasValidPath) {
				this.Log ("Observing {0}", Path.GetFileName (script.FilePath));
				Reloader.WatchFile (script);
			} else if (!script.HasValidPath)
				this.Log ("Live reload not possible: No path information available");
			return this;
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
			AddReference ("libnginz.Interop.IronPython");
			if (Game != null)
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

