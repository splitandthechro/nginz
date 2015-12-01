using System;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;

namespace nginz.Interop.IronPython
{
	public class PythonVM
	{
		readonly Game Game;
		readonly ScriptEngine Engine;
		readonly ScriptScope Scope;
		CompiledCode CompiledSource;

		public dynamic this [string name] {
			get { return Call (name); }
		}

		public PythonVM (Game game) {
			Game = game;
			Game.Content.RegisterAssetProvider<PythonScript> (typeof(PythonScriptProvider));
			Engine = Python.CreateEngine ();
			Scope = Engine.CreateScope ();
			SetupGlobals ();
		}

		public void Load (PythonScript script) {
			var source = Engine.CreateScriptSourceFromString (script.Source);
			CompiledSource = source.Compile ();
			CompiledSource.Execute (Scope);
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
			ImportFrom ("nginz", "*");
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

