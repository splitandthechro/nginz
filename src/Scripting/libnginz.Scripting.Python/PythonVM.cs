using System;
using System.IO;
using IronPython.Runtime;
using IronPython.Runtime.Operations;
using Microsoft.Scripting.Hosting;
using nginz.Common;
using PythonHost = IronPython.Hosting.Python;

namespace nginz.Scripting.Python
{
	public class PythonVM : ICanLog, ICanThrow
	{
		readonly public ScriptEngine Engine;
		readonly public ScriptScope Scope;
		readonly public ScriptReloader Reloader;

		readonly Game Game;

		string currentError;

		public dynamic this [string name] {
			get { return CallDynamic (name); }
		}

		public PythonVM (Game game = null) {
			Game = game;
			Engine = PythonHost.CreateEngine ();
			Scope = Engine.CreateScope ();
			Reloader = new ScriptReloader ("*.py");
			Reloader.LoadScript = Load;
			if (Game != null) {
				Reloader.PauseGame = Game.Pause;
				Reloader.ResumeGame = Game.Resume;
				Game.Content.RegisterAssetHandler<PythonScript> (typeof(PythonScriptProvider));
			}
			currentError = string.Empty;
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

		public dynamic CallDynamic (string name) {
			return Scope.GetVariable (name);
		}

		public object Call (string name, params object[] args) {
			LogExtensions.IsRunningInScriptedEnvironment = true;
			object retVal = null;
			try {
				var func = Scope.GetVariable<PythonFunction> (name);
				retVal = PythonCalls.Call (func, args);
				currentError = string.Empty;
			} catch (Exception e) {
				if (currentError != e.Message)
					this.Log (e.Message);
				currentError = e.Message;
			}
			LogExtensions.IsRunningInScriptedEnvironment = false;
			return retVal;
		}

		public object CallInstance (object instance, string name, params object[] args) {
			try {
				var result = instance.GetType ().InvokeMember (name, System.Reflection.BindingFlags.InvokeMethod, null, instance, args);
				currentError = string.Empty;
				return result;
			} catch (Exception e) {
				if (currentError != e.Message)
					this.Log (e.Message);
				currentError = e.Message;
			}
			return null;
		}

		public T Call<T> (string name, params object[] args) {
			return (T) Call (name, args);
		}

		public T CallInstance<T> (object instance, string name, params object[] args) {
			return (T) CallInstance (instance, name, args);
		}

		public string GetLastError () {
			return currentError;
		}

		public dynamic CreateInstance (string name, params object[] args) {
			if (!Scope.ContainsVariable (name))
				this.Throw ("Cannot find type: {0}", name);
			return Engine.Operations.CreateInstance (name, args);
		}

		public void Shutdown () {
			Engine.Operations.Engine.Runtime.Shutdown ();
		}

		void SetupGlobals () {
			Import ("clr");
			AddReference ("System.Drawing");
			AddReference ("OpenTK");
			AddReference ("libnginz");
			AddReference ("libnginz.Common");
			AddReference ("libnginz.Scripting.Python");
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

